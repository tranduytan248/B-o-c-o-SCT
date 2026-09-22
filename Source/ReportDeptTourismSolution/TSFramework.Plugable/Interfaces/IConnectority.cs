using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace TSFramework.Plugable.Interfaces
{
    public interface IConnectority
    {
        string Name { get; }
        string ConnectionString { get; set; }
        IConnectority Instance();

        Dictionary<string, ProcedureModel> GetListProcedureInDB();
        ProcedureModel GetProcedureInDBByName(string spName);
        object Connect();
        bool Close(object conn);
        DbParameter CreateParameter(string fieldName, object value);
        bool ExceNonQuery(string queryString, DbParameter[] parameters);
        object ExceQuery(string queryString, DbParameter[] parameters);
        object ExceStoreProcedure(string nameStore, DbParameter[] parameters);
        object ExceStoreProcedureWithReturn(string nameStore, DbParameter[] parameters);
    }

    
}
