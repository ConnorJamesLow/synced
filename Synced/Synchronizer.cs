using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Synced.Attributes;

namespace Synced
{
    public class Synchronizer
    {
        private readonly string _connection;
        public Synchronizer(string connectionString)
        {
            _connection = connectionString;
        }

        public void SyncTable<T>()
        {
            Type type = typeof(T);
            if (type.FullName.StartsWith("System"))
            {
                throw new TypeAccessException($"{type.FullName} is not permitted as a Table Model");
            }

            // Define the table name
            TableModel schema = new TableModel
            {
                Name = type.Name
            };

            // Get the public properties of this type
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public);

            // Build the data for table columns
            List<ColumnModel> columns = new List<ColumnModel>();
            foreach (PropertyInfo property in properties)
            {
                Identity identity = property.GetCustomAttribute<Identity>();
                ColumnModel column = new ColumnModel
                {
                    Name = property.Name,
                    DataType = GetColumnType(property),
                    Parameters = GetSizeModifier(property),
                    AllowsNulls = property.GetCustomAttribute<AllowNulls>() != null,
                    Unique = property.GetCustomAttribute<Unique>() != null,
                    IsIdentity = identity != null
                };
                if (column.IsIdentity)
                {
                    column.Seed = identity.Seed;
                    column.Increment = identity.Increment;
                }

                columns.Add(column);

            }
            schema.Columns = columns;

            // Build out the sql
            System.Text.StringBuilder columnsSql = new System.Text.StringBuilder();
            schema.Columns.ToList().ForEach(c =>
            {
                columnsSql.Append(@$",
    [{c.Name}] [{c.DataType.ToString()}] {( c.IsIdentity ? $"IDENTITY({c.Seed},{c.Increment})" : "" )} {( c.AllowsNulls ? "" : "NOT" )} NULL");
            });
            string sql = @$"SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE {schema.Name} ({columnsSql.ToString().Substring(1)}
) ON [PRIMARY]
GO
    ";

            // Connect to the database and create the table
            SqlConnection connection = new SqlConnection(_connection);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Gets the best guess <see cref="ColumnType"/> related to a property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public ColumnType GetColumnType(PropertyInfo property)
        {
            DataType attribute = property.GetCustomAttribute<DataType>();
            if (attribute != null)
            {
                return attribute.ColumnType;
            }
            else if (DefaultTypeMapping.ContainsKey(property.GetType()))
            {
                return DefaultTypeMapping[property.GetType()];
            }
            return ColumnType.varchar;
        }

        public int[] GetSizeModifier(PropertyInfo property, int parameterCount = 0)
        {
            if (parameterCount == 0)
            {
                return new int[] { };
            }

            // First, try getting the value from DataSize.Size
            DataSize sizeAttribute = property.GetCustomAttribute<DataSize>();
            if (sizeAttribute != null)
            {
                if (parameterCount == 2)
                {
                    return new int[] { sizeAttribute.Param1, sizeAttribute.Param2 };
                }
                return new int[] { sizeAttribute.Param1 };
            }

            // Next, try getting the value from the DataType.Size
            DataType typeAttribute = property.GetCustomAttribute<DataType>();
            if (typeAttribute != null)
            {
                if (parameterCount == 2)
                {
                    return new int[] { typeAttribute.SizeModifier, typeAttribute.SizeModifier2 };
                }
                return new int[] { typeAttribute.SizeModifier };
            }
            return new int[] { 0 };
        }

        /// <summary>
        /// Provides some default mappings between C# data types and T-SQL data types.
        /// </summary>
        public Dictionary<Type, ColumnType> DefaultTypeMapping = new Dictionary<Type, ColumnType>
        {
            { typeof(bool), ColumnType.bit },
            { typeof(int), ColumnType.@int },
            { typeof(uint), ColumnType.@int },
            { typeof(short), ColumnType.smallint },
            { typeof(ushort), ColumnType.smallint },
            { typeof(long), ColumnType.bigint },
            { typeof(ulong), ColumnType.bigint },
            { typeof(double), ColumnType.numeric },
            { typeof(float), ColumnType.@float },
            { typeof(byte), ColumnType.tinyint },
            { typeof(string), ColumnType.varchar },
            { typeof(IEnumerable<byte>), ColumnType.binary },
            { typeof(DateTime), ColumnType.datetime }
        };
    }
}
