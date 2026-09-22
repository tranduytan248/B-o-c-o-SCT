using CenIT.ReportTourism.Models.Sys;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Sys
{
    public class SysIpRequestBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _sysIpRequestGetByIp = "Sys_IPRequest_GetByIp";

        public SysIPRequestModel GetByIP(string sIp)
        {
            var modelIP =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysIPRequestModel>(_sysIpRequestGetByIp,
                    DATA_PROVIDER_NAME, sIp);
            return modelIP;
        }
    }
}