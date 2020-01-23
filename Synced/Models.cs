using System.Collections.Generic;

namespace Synced
{


    internal class TableModel
    {
        public string Name { get; set; }
        public IEnumerable<ColumnModel> Columns { get; set; }
    }

    internal class ColumnModel
    {
        [Attributes.Identity]
        public string Name { get; set; }
        public int SizeModifier { get; set; }
        public bool AllowsNulls { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool IsIdentity { get; set; } = false;
        public ColumnType DataType { get; set; } = ColumnType.varchar;
    }
}
