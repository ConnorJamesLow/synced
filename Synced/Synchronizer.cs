using System;
using System.Collections.Generic;
using System.Reflection;

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

            // Get the public properties of this type
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public);


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
