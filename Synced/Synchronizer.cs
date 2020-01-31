using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Synced.Attributes;

namespace Synced
{
    public partial class Synchronizer
    {
        private readonly string _connection;
        public Synchronizer(string connectionString)
        {
            _connection = connectionString;
        }

        public void SyncTable<T>(SyncFlags flags = SyncFlags.None)
        {

            Type type = typeof(T);
            string tableName = type.Name;
            if (TableExists(tableName))
            {
                if (TableHasRows(tableName))
                {
                    if (!CompareFlag(flags, SyncFlags.ForceDrop))
                    {
                        // TODO Try Update table
                        return;
                    }
                    DeleteRecords(tableName);
                }
                DropTable(tableName);
            }
            CreateTable(type);
        }

        public void CreateTable(Type type)
        {
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
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Build the data for table columns
            List<ColumnModel> columns = new List<ColumnModel>();
            foreach (PropertyInfo property in properties)
            {
                Identity identity = property.GetCustomAttribute<Identity>();
                ColumnType columnType = GetColumnType(property);
                ColumnModel column = new ColumnModel
                {
                    Name = property.Name,
                    DataType = columnType,
                    Parameters = GetSizeModifier(property, columnType),
                    AllowsNulls = property.GetCustomAttribute<AllowNulls>() != null,
                    Unique = property.GetCustomAttribute<Unique>() != null,
                    IsIdentity = identity != null
                };

                // Set Identity attributes
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
                // Get size modifier
                int[] parameters = c.Parameters.ToArray();
                string sizeString = "";
                if (parameters.Length == 1)
                {
                    string value = parameters[0] > 0
                        ? parameters[0].ToString()
                        : ( c.ParameterCanBeMax ? "MAX" : "1" );
                    sizeString = $"({value})";
                }
                else if (parameters.Length > 1)
                {
                    sizeString = $"({parameters[0]},{parameters[1]})";
                }

                // Create column sql
                columnsSql.Append(@$",
    [{c.Name}] [{c.DataType.ToString()}]{sizeString} {( c.IsIdentity ? $"IDENTITY({c.Seed},{c.Increment})" : "" )} {( c.AllowsNulls ? "" : "NOT" )} NULL");
            });

            // Set the primary key
            ColumnModel keyColumn = GetPrimaryKey(schema);
            if (keyColumn != default)
            {
                columnsSql.Append(@$",
    CONSTRAINT [PK_{schema.Name}] PRIMARY KEY CLUSTERED (
		[{keyColumn.Name}] ASC
	) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]");
            }

            // Create the CREATE TABLE Sql.
            string sql = @$"SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE {schema.Name} ({columnsSql.ToString().Substring(1)}
) ON [PRIMARY]";

            // Connect to the database and create the table
            SqlCommand command = CreateCommand(sql);
            ExecuteCommand(command);
        }

        /// <summary>
        /// DROP a table.
        /// </summary>
        /// <param name="tableName"></param>
        public void DropTable(string tableName) => ExecuteCommand(CreateCommand($@"DROP TABLE [dbo].{tableName}"));

        /// <summary>
        /// Delete all records from a table.
        /// </summary>
        /// <param name="tableName"></param>
        public void DeleteRecords(string tableName) => ExecuteCommand(CreateCommand($@"DELETE FROM [dbo].{tableName}"));
    }
}
