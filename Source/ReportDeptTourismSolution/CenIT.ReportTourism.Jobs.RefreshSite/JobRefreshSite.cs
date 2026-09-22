using System;
using TSFramework.Core.Members.Job;
using TSFramework.Core.Providers;

namespace CenIT.ReportTourism.Jobs.RefreshSite
{
    [JobPlugin("JobRefreshSite", "Job tự động gọi request site")]
    public class JobRefreshSite : IJobPlugable
    {
        public string PluginName
        {
            get { return "Job tự động gọi request site"; }
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

            return new JobRefreshSite {MainJob = kpiScheduleJob, JobType = typeof(ScheduleExecuteJobModel)};
        }
    }
}