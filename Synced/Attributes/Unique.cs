using System;
using System.Collections.Generic;
using System.Text;

namespace Synced.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class Unique: Attribute
    {
    }
}
