using System;
using System.Collections.Generic;
using System.Text;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class Identity : Attribute
    {
        public int Seed { get; set; } = 0;
        public int Increment { get; set; } = 1;

        public Identity ()
        {

        }

        public Identity(int seed, int increment)
        {
            Seed = seed;
            Increment = increment;
        }
    }
}
