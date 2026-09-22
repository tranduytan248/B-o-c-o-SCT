using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Hosting;
using CenIT.ReportTourism.Caches.Cate;
using CenIT.ReportTourism.Caches.Sys;
using CenIT.ReportTourism.Models.Cate;
using Quartz;
using TSFramework.App.Processors;
using TSFramework.Core.Members.Job;
using TSFramework.Core.Members.Mail;
using TSFramework.Core.Providers;

namespace CenIT.ReportTourism.Jobs.NotifyEnterprise
{
    public class ScheduleExecuteJobModel : JobModel
    {
        private readonly SysConfigsCache _configCache;
        private readonly CateEnterpriseCache _enterpriseCache;

        public ScheduleExecuteJobModel()
        {
            _enterpriseCache = new CateEnterpriseCache();
            _configCache = new SysConfigsCache();
        }

        public override void Execute(IJobExecutionContext context)
        {
            try
            {
                AppProcessor.Logger.Message("=================================================================");
                AppProcessor.Logger.Message("==========Gửi mail nhắc nhở doanh nghiệp chưa nộp báo cáo========");
                AppProcessor.Logger.Message("==================================================================");

                var hostProtocol = ConfigurationManager.AppSettings["Host_Protocol"] ?? "http://";
                var urlHost = $"{hostProtocol}{HostingEnvironment.SiteName}/";

                var dayDeadlineSendReport =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Report")?.ConfigValue ?? "0");
                var dayDeadlineSendReportLate =
                    int.Parse(_configCache.GetViaKey("Day_Deadline_Send_Late_Report")?.ConfigValue ?? "0");

                var lstEmails = new List<MailModel>();
                _enterpriseCache.GetNotSumitReportYet(DateTime.Now).ForEach(e =>
                {
                    var templateMailNotify = new TemplateNotifyEnterpise
                    {
                        EnterpriseName = e.BusinessName,
                        DayDeadlineSendReport = dayDeadlineSendReport,
                        DayDeadlineSendReportLate = dayDeadlineSendReportLate,
                        UrlSubmitReport = urlHost
                    };
                    var mailBodyHtml = RenderTemplateHtmlProvider.RenderStringHtml(
                        HostingEnvironment.MapPath(
                            @"~/Contents/Modules/Cate/Templates/_MailNotifyEnterpriseTemplate.cshtml"),
                        templateMailNotify);
                    lstEmails.Add(new MailModel
                    {
                        DicImgs = new Dictionary<string, byte[]>
                            {
                                { "LogoVNPT", System.IO.File.ReadAllBytes($"{HostingEnvironment.MapPath(ConfigurationManager.AppSettings["Mail_VNPTLogoPath"])}")},
                                { "LogoTourism", new WebClient().DownloadData(ConfigurationManager.AppSettings["Mail_TourismLogoPath"])}
                            },
                        Subject = string.Format(AppProcessor.Messagor.GetMessage("MailSubject_NotifyEnterprise_Message"), DateTime.Now.ToString("MM/yyyy")),
                        To = new List<string> { e.Email },
                        Body = mailBodyHtml,
                        IsBodyHtml = true,
                        DisplayNameFrom = AppProcessor.Messagor.GetMessage("App_Owner_DisplayName")
                    });
                });
                AppProcessor.Mailer.PushEmail(lstEmails);
            }
            catch (Exception ex)
            {
                AppProcessor.Logger.Error(ex);
            }
        }
    }
}