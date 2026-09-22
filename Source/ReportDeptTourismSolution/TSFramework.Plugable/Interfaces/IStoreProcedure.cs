using System;
using System.Collections.Generic;
using System.Data;
using TiSun;
using TSFramework.Plugable.Models;

namespace TSFramework.Plugable.Interfaces
{
    public interface IStoreProcedure
    {
        Type ProviderType { get; }
        DataProvider SPProvider { get; set; }
        string SchemaName { get; set; }
        string ProviderName { get; set; }

        Dictionary<string, ProcedureModel> ListProcedures { get; }

        IStoreProcedure Instance(string providerName, Dictionary<string, string> mappingSP, string schemaName = "dbo");

        /// <summary>
        ///     Get list store procedure in db via data provider
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ProcedureModel> GetProcedures();

        /// <summary>
        ///     Get infomation of procedure via procedure name
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        ProcedureModel GetProcedureInfoByName(string spName);

        /// <summary>
        ///     Executes the stored procedure and returns the number of rows affected.
        /// </summary>
        /// <returns>int</returns>
        int? ExecuteProcedure(string spName, params object[] parameters);

        /// <summary>
        ///     Executes the stored procedure and returns the number of rows affected.
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable ExecuteProcedureDataTable(string spName, params object[] parameters);

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <returns>object</returns>
        object ExecuteProcedureScalar(string spName, params object[] parameters);

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>object</returns>
        object ExecuteProcedureScalar<T>(string spName, params object[] parameters);

        /// <summary>
        ///     Executes the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List<T></returns>
        List<T> ExecuteProcedureTypedList<T>(string spName, params object[] parameters) where T : new();

        /// <summary>
        ///     Executes query the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List<T></returns>
        List<T> ExecuteQueryTypedList<T>(string sQuery) where T : new();

        /// <summary>
        ///     Executes query the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>object</returns>
        object ExecuteQueryScalar<T>(string sQuery);
    }
}