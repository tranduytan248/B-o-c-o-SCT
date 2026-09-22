using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysLayoutBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysLayoutActivated = "Sys_Layout_Activated";
        private readonly string _sysLayoutDelete = "Sys_Layout_Delete";
        private readonly string _sysLayoutGet = "Sys_Layout_Get";
        private readonly string _sysLayoutGetActivated = "Sys_Layout_GetActivated";
        private readonly string _sysLayoutGetById = "Sys_Layout_GetById";
        private readonly string _sysLayoutGetByName = "Sys_Layout_GetByName";
        private readonly string _sysLayoutSave = "Sys_Layout_Save";

        private List<SysLayoutModel> Get(out int total, SysSearchModel search)
        {
            search = search ?? new SysSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var listLayouts = AppProcessor.ProcedureProvider.ExecuteTypedList<SysLayoutModel>(_sysLayoutGet,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (listLayouts != null && listLayouts.Count > 0)
                total = int.Parse(listLayouts.First()?.TotalRow.ToString() ?? "0");
            return listLayouts;
        }

        private SysLayoutModel LoadDetail(int layoutId)
        {
            var dataLayout =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysLayoutModel>(_sysLayoutGetById,
                    DATA_PROVIDER_NAME, layoutId);
            return dataLayout;
        }

        public bool Delete(SysLayoutModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysLayoutDelete, DATA_PROVIDER_NAME, model.LayoutId);
            return result == model.LayoutId;
        }

        public List<SysLayoutModel> GetAll()
        {
            int total;
            var listLayouts = Get(out total, null);
            return listLayouts;
        }

        public SysLayoutModel GetById(int layoutId)
        {
            var layout = LoadDetail(layoutId);
            return layout;
        }

        public SysLayoutModel GetByName(string layoutName)
        {
            var dataLayout =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysLayoutModel>(_sysLayoutGetByName,
                    DATA_PROVIDER_NAME, layoutName);
            return dataLayout;
        }

        public List<SysLayoutModel> GetList(out int total, SysSearchModel search = null)
        {
            var listLayouts = Get(out total, search);
            return listLayouts;
        }

        public int Save(SysLayoutModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysLayoutSave, DATA_PROVIDER_NAME,
                model.LayoutId,
                model.LayoutName,
                model.LayoutView,
                model.Note,
                model.NumberContentPanel,
                model.NumberCol,
                model.Creator,
                model.Updater
            );

            return result.GetValueOrDefault(0);
        }

        public bool Activated(SysLayoutModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysLayoutActivated, DATA_PROVIDER_NAME,
                model.LayoutId,
                model.Updater
            );

            return result == model.LayoutId;
        }

        public SysLayoutModel GetActivatedLayout()
        {
            var dataLayout =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysLayoutModel>(_sysLayoutGetActivated,
                    DATA_PROVIDER_NAME);
            return dataLayout;
        }
    }
}