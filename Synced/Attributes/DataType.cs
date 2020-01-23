using System;
using System.Collections.Generic;
using System.Text;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    sealed class DataType : Attribute
    {

        public ColumnType ColumnType { get; set; }
        public int SizeModifier { get; set; }

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

    }
}
