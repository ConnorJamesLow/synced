using System;
using System.Collections.Generic;
using System.Text;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DataSize: Attribute
    {
        public int Param1 { get; set; }
        public int Param2 { get; set; } = 0;
        public DataSize(int size)
        {
            Param1 = size;
        }

        public DataSize(int precision, int scale)
        {
            Param1 = precision;
            Param2 = scale;
        }
    }
}
