using System.Collections.Generic;

namespace TSFramework.Core.Members.Procedure
{
    public class StoreProcedure
    {
        public string Name { get; set; }
        public List<ProcedureParam> Params { get; set; }
    }
}