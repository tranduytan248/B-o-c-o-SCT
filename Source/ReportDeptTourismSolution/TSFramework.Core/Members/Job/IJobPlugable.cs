using System;

namespace TSFramework.Core.Members.Job
{
    public interface IJobPlugable
    {
        string PluginName { get; set; }
        Type JobType { get; set; }
        JobModel MainJob { get; set; }
        IJobPlugable BuildJob(string sJobId, string sDescription, string sCronExpression, params object[] dataObjects);
        void ExecuteJobNow(string sJobId, string sDescription, params object[] dataObjects);
    }
}