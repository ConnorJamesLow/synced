namespace Synced
{
    public enum ColumnType
    {
        bit = 0,
        @int = 1,
        bigint = 2,
        smallint = 3,
        tinyint = 4,

        numeric = 10,
        smallmoney = 11,
        money = 12,

        @float = 20,
        real = 21,

        binary = 30,
        varbinary = 31,
        image = 32,

        @char = 100,
        varchar = 101,
        text = 102,
        nchar = 110,
        nvarchar = 111,
        ntext = 112,

        date = 200,
        datetime = 201,
        datetime2 = 202,
        time = 203,
        datetimeoffset = 204,
        smalldatetime = 205,

        xml = 300
    }

    public class ColumnTypeHelpers
    {
        /// <summary>
        /// Get the count of parameters acceptable for a column type.
        /// <para>
        /// e.g. <c>varchar(150)</c>
        /// </para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetParameterCount(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.numeric:
                    return 2;
                case ColumnType.binary:
                case ColumnType.@char:
                case ColumnType.nchar:
                case ColumnType.varchar:
                case ColumnType.nvarchar:
                case ColumnType.varbinary:
                case ColumnType.time:
                case ColumnType.datetimeoffset:
                case ColumnType.datetime2:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get whether a type accepts MAX as a parameter.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetParameterCanBeMax(ColumnType type) => type == ColumnType.nvarchar
            || type == ColumnType.varbinary
            || type == ColumnType.varchar;
    }
}
