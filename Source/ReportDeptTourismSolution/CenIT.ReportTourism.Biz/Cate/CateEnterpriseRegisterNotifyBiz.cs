using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateEnterpriseRegisterNotifyBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateEnterpriseRegisterNotifiesGet = "Cate_EnterpriseRegisterNotifies_Get";
        private readonly string _cateEnterpriseRegisterNotifiesGetByID = "Cate_EnterpriseRegisterNotifies_GetByID";
        private readonly string _cateEnterpriseRegisterNotifiesSave = "Cate_EnterpriseRegisterNotifies_Save";

        public List<CateEnterpriseRegisterNotifyModel> Get(out int total,
            SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };
            var dataEnterpriseRegisterNotify =
                AppProcessor.ProcedureProvider.ExecuteTypedList<CateEnterpriseRegisterNotifyModel>(
                    _cateEnterpriseRegisterNotifiesGet, DATA_PROVIDER_NAME,
                    search.Search,
                    search.Order,
                    search.OrderDir,
                    search.StartIndex,
                    search.PageSize);
            total = 0;
            if (dataEnterpriseRegisterNotify != null && dataEnterpriseRegisterNotify.Count > 0)
                total = int.Parse(dataEnterpriseRegisterNotify.First().TotalRow.ToString());
            return dataEnterpriseRegisterNotify;
        }

        public List<CateEnterpriseRegisterNotifyModel> GetAll()
        {
            var listenterprises = Get(out _, null);
            return listenterprises;
        }

        public CateEnterpriseRegisterNotifyModel GetById(int? enterpriseId)
        {
            var enterprise =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateEnterpriseRegisterNotifyModel>(
                    _cateEnterpriseRegisterNotifiesGetByID,
                    DATA_PROVIDER_NAME, enterpriseId);
            return enterprise;
        }

        public int Save(CateEnterpriseRegisterNotifyModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateEnterpriseRegisterNotifiesSave, DATA_PROVIDER_NAME,
                model.EnterpriseID,
                model.HasProcessed,
                model.IsConfirm,
                model.CompletedBy
            );
            return result.GetValueOrDefault(0);
        }
    }
}