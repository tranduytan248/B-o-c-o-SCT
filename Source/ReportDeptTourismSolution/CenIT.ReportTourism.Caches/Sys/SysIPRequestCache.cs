using System.ComponentModel;
using CenIT.ReportTourism.Biz.Sys;
using CenIT.ReportTourism.Models.Sys;
using TSFramework.Core.Members.Caching;

namespace CenIT.ReportTourism.Caches.Sys
{
    [DataObject]
    public class SysIpRequestCache : CacheLayer
    {
        private SysIpRequestBiz _ipRequestApi;

        protected override string[] MasterCacheKeyArray => new[] { "SysIPRequestCache", "CENIT.Application.Cache" };

        private SysIpRequestBiz Api => _ipRequestApi ?? (_ipRequestApi = new SysIpRequestBiz());


        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public SysIPRequestModel GetByIp(string sIpRequest)
        {
            if (string.IsNullOrEmpty(sIpRequest)) return null;

            // See if the item is in the cache
            // Item not found in cache - retrieve it and insert it into the cache
            var ipRequest = Api.GetByIP(sIpRequest);

            return ipRequest;
        }
    }
}