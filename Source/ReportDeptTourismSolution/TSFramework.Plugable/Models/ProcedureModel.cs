using System.Collections.Generic;
using System.Data;

namespace TSFramework.Plugable.Models
{
    public class ProcedureModel
    {
        public string Name { get; set; }
        public List<ProcedureParamModel> Params { get; set; }
    }

    public class ProcedureParamModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public DbType DataType { get; set; }
        public byte? NumberPrecision { get; set; }
        public int? NumberScale { get; set; }
    }
}