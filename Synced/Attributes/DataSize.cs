using System;
using System.Collections.Generic;
using System.Text;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DataSize: Attribute
    {
        public int Size { get; set; }
        public DataSize(int size)
        {
            Size = size;
        }
    }
}
