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
        public string Name { get; set; }
        public IEnumerable<int> Parameters { get; set; }
        public bool ParameterCanBeMax
        {
            get
            {
                return ColumnTypeHelpers.GetParameterCanBeMax(DataType);
            }
        }

        public bool AllowsNulls { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool IsIdentity { get; set; } = false;
        public int Seed { get; set; } = 0;
        public int Increment { get; set; } = 0;
        public bool PrimaryKey { get; set; } = false;
        public ColumnType DataType { get; set; } = ColumnType.varchar;
    }
}
