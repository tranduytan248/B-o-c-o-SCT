using System;
using System.Configuration;
using System.Net;
using System.Web.Hosting;
using Quartz;
using TSFramework.App.Processors;
using TSFramework.Core.Members.Job;

namespace CenIT.ReportTourism.Jobs.RefreshSite
{
    public class ScheduleExecuteJobModel : JobModel
    {
        public override void Execute(IJobExecutionContext context)
        {
            try
            {
                var hostProtocol = ConfigurationManager.AppSettings["Host_Protocol"] ?? "http://";
                AppProcessor.Logger.Message("========================================");
                AppProcessor.Logger.Message("==========Gọi Request tới site========");
                AppProcessor.Logger.Message("========================================");
                var urlHost = $"{hostProtocol}{HostingEnvironment.SiteName}/";
                using (var client = new WebClient())
                {
                    client.DownloadString(urlHost);
                }
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }
        }
    }
}