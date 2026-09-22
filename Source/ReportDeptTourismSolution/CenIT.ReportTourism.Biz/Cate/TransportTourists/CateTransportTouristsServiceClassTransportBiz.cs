using System.Collections.Generic;
using System.Linq;
using CenIT.ReportTourism.Models.Cate.TransportTourists;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate.TransportTourists
{
    public class CateTransportTouristsServiceClassTransportBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";

        private readonly string _cateTransportTouristsServicesInfrastructuresClassTransportDelete =
            "Cate_TransportTouristsServices_Infrastructures_ClassTransports_Delete";

        private readonly string _cateTransportTouristsServicesInfrastructuresClassTransportGet =
            "Cate_TransportTouristsServices_Infrastructures_ClassTransports_Get";

        private readonly string _cateTransportTouristsServicesInfrastructuresClassTransportGetById =
            "Cate_TransportTouristsServices_Infrastructures_ClassTransports_GetByID";

        private readonly string _cateTransportTouristsServicesInfrastructuresClassTransportSave =
            "Cate_TransportTouristsServices_Infrastructures_ClassTransports_Save";

        public List<CateTransportTouristsServiceInfrastructureClassTransportModel> Get(int? enterpriseId, out int total,
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
            var dataTransportTouristsServicesClassTransport =
                AppProcessor.ProcedureProvider
                    .ExecuteTypedList<CateTransportTouristsServiceInfrastructureClassTransportModel>(
                        _cateTransportTouristsServicesInfrastructuresClassTransportGet, DATA_PROVIDER_NAME,
                        enterpriseId,
                        search.Search,
                        search.Order,
                        search.OrderDir,
                        search.StartIndex,
                        search.PageSize);
            total = 0;
            if (dataTransportTouristsServicesClassTransport != null &&
                dataTransportTouristsServicesClassTransport.Count > 0)
                total = int.Parse(dataTransportTouristsServicesClassTransport.First().TotalRow.ToString());
            return dataTransportTouristsServicesClassTransport;
        }

        public List<CateTransportTouristsServiceInfrastructureClassTransportModel> GetAll(int? enterpriseId = null)
        {
            int total;
            var listpublicCates = Get(enterpriseId, out total, null);
            return listpublicCates;
        }

        public CateTransportTouristsServiceInfrastructureClassTransportModel GetById(long? publicCateId)
        {
            var publicCate =
                AppProcessor.ProcedureProvider
                    .ExecuteScalarObject<CateTransportTouristsServiceInfrastructureClassTransportModel>(
                        _cateTransportTouristsServicesInfrastructuresClassTransportGetById, DATA_PROVIDER_NAME,
                        publicCateId);
            return publicCate;
        }

        public int Save(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _cateTransportTouristsServicesInfrastructuresClassTransportSave, DATA_PROVIDER_NAME,
                model.InfrastructureClassTransportId,
                model.EnterpriseId,
                model.TypeTransport,
                model.TypeTransportName,
                model.ClassTransportName,
                model.TotalTransport,
                model.Reason,
                model.SavedBy);
            return result.GetValueOrDefault(0);
        }

        public bool Delete(CateTransportTouristsServiceInfrastructureClassTransportModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _cateTransportTouristsServicesInfrastructuresClassTransportDelete, DATA_PROVIDER_NAME,
                model.InfrastructureClassTransportId, model.Reason, model.SavedBy);
            return result == model.InfrastructureClassTransportId;
        }
    }
}