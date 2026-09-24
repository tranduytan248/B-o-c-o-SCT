using System.Collections.Generic;
using CenIT.ReportTourism.Models.Cate;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateBusinessProductBiz
    {
        private const string DataProviderName = "SysProvider";
        private const string GetViaEnterpriseProcedure = "Cate_BusinessProducts_GetViaEnterprise";
        private const string GetByPrefixProcedure = "Cate_BusinessProducts_GetByPrefix";
        private const string GetUnconfiguredByPrefixProcedure = "Cate_BusinessProducts_GetUnconfiguredByPrefix";

        public List<CateBusinessProductModel> GetViaEnterprise(int enterpriseId)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<CateBusinessProductModel>(
                GetViaEnterpriseProcedure, DataProviderName, enterpriseId);
        }

        public List<CateBusinessProductModel> GetByPrefix(int enterpriseId, string productCodePrefix)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<CateBusinessProductModel>(
                GetByPrefixProcedure, DataProviderName, enterpriseId, productCodePrefix);
        }

        public List<CateBusinessProductModel> GetUnconfiguredByPrefix(int enterpriseId, string productCodePrefix)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<CateBusinessProductModel>(
                GetUnconfiguredByPrefixProcedure, DataProviderName, enterpriseId, productCodePrefix);
        }
    }
}
