using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysFunctionActionBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysFunctionActionGet = "Sys_FunctionAction_GetAll";

        private List<SysFunctionActionModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listFunctionActions = AppProcessor.ProcedureProvider.ExecuteTypedList<SysFunctionActionModel>(
                _sysFunctionActionGet, DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listFunctionActions != null && listFunctionActions.Count > 0)
                total = int.Parse(listFunctionActions.First()?.TotalRow.ToString() ?? "0");
            return listFunctionActions;
        }

        public List<SysFunctionActionModel> GetAll()
        {
            int total;
            var listFunctionActions = Get(out total, null);
            return listFunctionActions;
        }
    }
}