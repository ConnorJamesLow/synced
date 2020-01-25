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

        public DataType(ColumnType type, int scale)
        {
            ColumnType = type;
            SizeModifier = scale;
        }

        public DataType(ColumnType type, int scale, int precision)
        {
            ColumnType = type;
            SizeModifier = scale;
            SizeModifier2 = precision;
        }

    }
}
