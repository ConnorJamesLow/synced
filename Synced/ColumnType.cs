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
    }
}
