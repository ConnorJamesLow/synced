using System;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DataType : Attribute
    {

        public ColumnType ColumnType { get; set; }
        public int SizeModifier { get; set; }
        public int SizeModifier2 { get; set; } = 0;



        // This is a positional argument
        public DataType(ColumnType type)
        {
            ColumnType = type;
        }

        public DataType(ColumnType type, int size)
        {
            ColumnType = type;
            SizeModifier = size;
        }

        public DataType(ColumnType type, int precision, int scale)
        {
            ColumnType = type;
            SizeModifier = precision;
            SizeModifier2 = precision;
        }

    }
}
