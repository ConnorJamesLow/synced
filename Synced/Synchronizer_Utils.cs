﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Synced.Attributes;

namespace Synced
{
    public partial class Synchronizer
    {
        private bool CompareFlag(SyncFlags flag, SyncFlags compare) => ( flag & compare ) == compare;

        public bool TableExists(string tableName)
        {
            string sql = @"IF (EXISTS (
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME = @Table
))
SELECT 1 AS [Exists]
ELSE
SELECT 0 AS [Exists]";
            SqlCommand command = CreateCommand(sql);
            command.Parameters.AddWithValue("@Table", tableName);
            return ExecuteSingleReturn<int>(command) == 1;
        }

        private ColumnModel GetPrimaryKey(TableModel model) => model.Columns.FirstOrDefault(i => i.IsIdentity);

        public bool TableHasRows(string tableName)
        {
            string sql = @$"SELECT TOP 1 COUNT(*) FROM {tableName}";
            SqlCommand command = CreateCommand(sql);
            command.Parameters.AddWithValue("@Table", tableName);
            return ExecuteSingleReturn<int>(command) > 0;
        }

        private SqlCommand CreateCommand(string sql) => new SqlCommand(sql);

        private void ExecuteCommand(SqlCommand command)
        {
            command.Connection = new SqlConnection(_connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
            command.Connection.Close();
        }

        private T ExecuteSingleReturn<T>(SqlCommand command)
        {
            T result;
            try
            {
                command.Connection = new SqlConnection(_connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return default;
                }
                result = (T)reader[0];
            }
            finally
            {
                command.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Gets the best guess <see cref="ColumnType"/> related to a property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private ColumnType GetColumnType(PropertyInfo property)
        {
            // First, try to get the ColumnType from the attribute.
            DataType attribute = property.GetCustomAttribute<DataType>();
            if (attribute != null)
            {
                return attribute.ColumnType;
            }
            else if (DefaultTypeMapping.ContainsKey(property.PropertyType))
            {
                // If the attribute is not available, but a default mapping exists, use that.
                return DefaultTypeMapping[property.PropertyType];
            }
            else if (typeof(IEnumerable<byte>).IsAssignableFrom(property.PropertyType))
            {
                // Additionally, try for binary.
                return ColumnType.binary;
            }

            // Return varchar by default.
            return ColumnType.varchar;
        }

        /// <summary>
        /// For scalable data types, gets the scale (and precision in some cases).
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int[] GetSizeModifier(PropertyInfo property, ColumnType type)
        {
            int parameterCount = ColumnTypeHelpers.GetParameterCount(type);
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
            return parameterCount == 2 ? new int[] { 0, 0 } : new int[] { 0 };
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

            // this needs additional checks, e.g. in GetColumnType
            { typeof(IEnumerable<byte>), ColumnType.binary },
            { typeof(DateTime), ColumnType.datetime }
        };
    }
}
