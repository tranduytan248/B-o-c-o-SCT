using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysContentPanelBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysContentPanelDelete = "SysContentPanel_Delete";
        private readonly string _sysContentPanelGet = "SysContentPanel_Get";
        private readonly string _sysContentPanelGetById = "SysContentPanel_GetById";
        private readonly string _sysContentPanelGetByLayoutId = "SysContentPanel_GetByLayoutId";
        private readonly string _sysContentPanelGetByLayoutName = "SysContentPanel_GetByLayoutName";
        private readonly string _sysContentPanelSave = "SysContentPanel_Save";

        private List<SysContentPanelModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listContentPanels = AppProcessor.ProcedureProvider.ExecuteTypedList<SysContentPanelModel>(
                _sysContentPanelGet, DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listContentPanels != null && listContentPanels.Count > 0)
                total = int.Parse(listContentPanels.First()?.TotalRow.ToString() ?? "0");
            return listContentPanels;
        }

        private SysContentPanelModel LoadDetail(int contentPanelId)
        {
            var dataContentPanel =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysContentPanelModel>(_sysContentPanelGetById,
                    DATA_PROVIDER_NAME, contentPanelId);
            return dataContentPanel;
        }

        public bool Delete(SysContentPanelModel model)
        {
            var result =
                AppProcessor.ProcedureProvider.Execute(_sysContentPanelDelete, DATA_PROVIDER_NAME,
                    model.ContentPanelId);
            return result == model.ContentPanelId;
        }

        public List<SysContentPanelModel> GetAll()
        {
            int total;
            var listContentPanels = Get(out total, null);
            return listContentPanels;
        }

        public SysContentPanelModel GetById(int contentPanelId)
        {
            var contentPanel = LoadDetail(contentPanelId);
            return contentPanel;
        }

        public List<SysContentPanelModel> GetList(out int total, SysSearchModel search = null)
        {
            var listContentPanels = Get(out total, search);
            return listContentPanels;
        }

        public List<SysContentPanelModel> GetByLayoutName(string layoutName)
        {
            var dataContentPanel =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysContentPanelModel>(_sysContentPanelGetByLayoutName,
                    DATA_PROVIDER_NAME, layoutName);
            return dataContentPanel;
        }

        public List<SysContentPanelModel> GetByLayoutId(int layoutId)
        {
            var dataContentPanel =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysContentPanelModel>(_sysContentPanelGetByLayoutId,
                    DATA_PROVIDER_NAME, layoutId);
            return dataContentPanel;
        }

        public int Save(SysContentPanelModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysContentPanelSave, DATA_PROVIDER_NAME,
                model.ContentPanelId,
                model.ContentPanelName,
                model.LayoutId,
                model.Note
            );

            return result.GetValueOrDefault(0);
        }
    }
}