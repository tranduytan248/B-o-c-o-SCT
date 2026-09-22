using System;
using TSFramework.Core.Members.Job;
using TSFramework.Core.Providers;

namespace CenIT.ReportTourism.Jobs.NotifyEnterprise
{
    [JobPlugin("JobNotifyEnterprise", "Job tự động gửi mail nhắc nhở doanh nghiệp nộp báo cáo")]
    public class JobNotifyEnterprise : IJobPlugable
    {
        public string PluginName
        {
            get { return "Job tự động gửi mail nhắc nhở doanh nghiệp nộp báo cáo"; }
            set { }
        }

        public Type JobType { get; set; }

        public JobModel MainJob { get; set; }

        public void ExecuteJobNow(string sJobId, string sDescription, params object[] dataObjects)
        {
            var sPreTriggerId = JobProvider.GenerateTriggerId(5); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(6); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var kpiScheduleJob = new ScheduleExecuteJobModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .StartNow();

            var dataMailContent = dataObjects?[0];
            kpiScheduleJob.ExecuteNow(dataMailContent);
        }

        public IJobPlugable BuildJob(string sJobId, string sDescription, string sCronExpression,
            params object[] dataObjects)
        {
            //sCronExpression = "0 0/17 * * * ? *";
            var sPreTriggerId = JobProvider.GenerateTriggerId(5); // Can custom
            var sPreGroupId = JobProvider.GenerateGroupId(6); // Can custom
            var sTriggerId = $"{sPreTriggerId}-{sJobId}";
            var sGroupId = $"{sPreGroupId}-{sJobId}";

            var kpiScheduleJob = new ScheduleExecuteJobModel()
                .SetJobId(sJobId)
                .SetGroupId(sGroupId)
                .SetDescription(sDescription)
                .SetTriggerId(sTriggerId)
                .WithCronSchedule(sCronExpression);

            return new JobNotifyEnterprise {MainJob = kpiScheduleJob, JobType = typeof(ScheduleExecuteJobModel)};
        }
    }
}