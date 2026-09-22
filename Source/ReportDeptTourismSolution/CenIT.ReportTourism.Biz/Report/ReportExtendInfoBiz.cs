using System;
using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Report;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Report
{
    public class ReportExtendInfoBiz
    {
        private const string DATA_PROVIDER_NAME = "ReportTourismProvider";
        private readonly string _reportExtendInfoGet = "Report_ExtendInfos_Get";
        private readonly string _reportExtendInfoGetById = "Report_ExtendInfos_GetByID";
        private readonly string _reportExtendInfoSave = "Report_ExtendInfos_Save";
        private readonly string _reportExtendInfosGetViaMonth = "Report_ExtendInfos_GetViaMonth";

        private List<ReportExtendInfoModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listExtendInfos = AppProcessor.ProcedureProvider.ExecuteTypedList<ReportExtendInfoModel>(
                _reportExtendInfoGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listExtendInfos != null && listExtendInfos.Count > 0)
                total = int.Parse(listExtendInfos.First()?.TotalRow.ToString() ?? "0");
            return listExtendInfos;
        }

        private ReportExtendInfoModel LoadDetail(int? extendInfoId)
        {
            var lstExtendInfos =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<ReportExtendInfoModel>(_reportExtendInfoGetById,
                    DATA_PROVIDER_NAME, extendInfoId);

            return lstExtendInfos;
        }

        public ReportExtendInfoModel GetOnMonth(DateTime? onMonth)
        {
            var lstExtendInfos =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<ReportExtendInfoModel>(_reportExtendInfosGetViaMonth,
                    DATA_PROVIDER_NAME, onMonth);

            return lstExtendInfos;
        }

        public List<ReportExtendInfoModel> GetAll()
        {
            int total;
            var listExtendInfos = Get(out total, null);
            return listExtendInfos;
        }

        public ReportExtendInfoModel GetById(int? extendInfoId)
        {
            var extendInfo = LoadDetail(extendInfoId);
            return extendInfo;
        }

        public List<ReportExtendInfoModel> GetList(out int total, SysSearchModel search = null)
        {
            var listExtendInfos = Get(out total, search);
            return listExtendInfos;
        }

        public int? Save(ReportExtendInfoModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_reportExtendInfoSave, DATA_PROVIDER_NAME, model.Id,
                model.ForMonth, model.TotalGuestViaShip, model.Reason, model.SavedBy);

            return result;
        }
    }
}