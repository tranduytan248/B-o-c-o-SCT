using System;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using TSFramework.Core.Members.Job;

namespace TSFramework.Core.Providers
{
    public class JobSchedulerProvider
    {
        private static readonly ISchedulerFactory SchedFact = new StdSchedulerFactory();

        private static IScheduler _currentScheduler;

        private static IScheduler GetCurrentScheduler()
        {
            _currentScheduler = _currentScheduler ?? SchedFact.GetScheduler();
            return _currentScheduler;
        }

        public static IList<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            return _currentScheduler?.GetCurrentlyExecutingJobs();
        }

        private static IList<IJobDetail> GetAllJobs()
        {
            IList<IJobDetail> lstJobsInSchedule = new List<IJobDetail>();
            var jobGroups = _currentScheduler.GetJobGroupNames();

            foreach (var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = _currentScheduler.GetJobKeys(groupMatcher);
                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = _currentScheduler.GetJobDetail(jobKey);
                    lstJobsInSchedule.Add(jobDetail);
                }
            }

            return lstJobsInSchedule;
        }

        public static void RegisterJobScheduler(Type jobType, JobModel typeJob)
        {
            // construct a scheduler factory

            // get a scheduler
            _currentScheduler = _currentScheduler ?? SchedFact.GetScheduler();
            _currentScheduler?.Start();

            var job = JobBuilder.Create().OfType(jobType)
                .WithIdentity(typeJob.JobId, typeJob.GroupId)
                .WithDescription(typeJob.Description)
                .Build();

            var trigger = typeJob.Trigger.Build();

            if (_currentScheduler == null) return;
            if (_currentScheduler.CheckExists(job.Key)) _currentScheduler.DeleteJob(job.Key);
            _currentScheduler.ScheduleJob(job, trigger);
            if (!typeJob.IsActive) _currentScheduler.PauseJob(job.Key);
        }

        public static void RegisterCronScheduler(Type typeJob, string cronSchedule, string identityName,
            string identityGroupName, string jobDescription, string triggerId)
        {
            // Grab the Scheduler instance from the Factory 
            _currentScheduler = _currentScheduler ?? SchedFact.GetScheduler();
            _currentScheduler?.Start();

            //// define the job and tie it to our
            var jobDetail = JobBuilder.Create<IJob>()
                .WithIdentity(identityName, identityGroupName)
                .WithDescription(jobDescription)
                .OfType(typeJob)
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerId, identityGroupName)
                .StartNow()
                .WithCronSchedule(cronSchedule)
                .Build();

            if (_currentScheduler == null) return;
            if (_currentScheduler.CheckExists(jobDetail.Key)) _currentScheduler.DeleteJob(jobDetail.Key);
            _currentScheduler.ScheduleJob(jobDetail, trigger);
        }

        public static void PauseJob(string sJobId)
        {
            var curShedule = GetCurrentScheduler();
            if (curShedule == null) return;
            var lstExecutingJobs = GetAllJobs();

            foreach (var executingJob in lstExecutingJobs)
            {
                if (executingJob.Key.Name != sJobId) continue;
                if (!curShedule.InStandbyMode && curShedule.IsStarted)
                {
                    curShedule.PauseJob(executingJob.Key);
                    break;
                }

                Console.WriteLine($"Paused job {sJobId}");
            }
        }

        public static void ResumeJob(string sJobId)
        {
            var curShedule = GetCurrentScheduler();
            if (curShedule == null) return;
            var lstExecutingJobs = GetAllJobs();

            foreach (var executingJob in lstExecutingJobs)
            {
                if (executingJob.Key.Name != sJobId) continue;
                if (!curShedule.InStandbyMode && curShedule.IsStarted)
                {
                    curShedule.ResumeJob(executingJob.Key);
                    break;
                }

                Console.WriteLine($"Resumed job {sJobId}");
            }
        }

        public static void DeleteJob(string sJobId)
        {
            var curShedule = GetCurrentScheduler();
            if (curShedule == null) return;
            var lstExecutingJobs = GetAllJobs();

            foreach (var executingJob in lstExecutingJobs)
            {
                if (executingJob.Key.Name != sJobId) continue;
                if (!curShedule.InStandbyMode && curShedule.IsStarted)
                {
                    curShedule.DeleteJob(executingJob.Key);
                    break;
                }

                Console.WriteLine($"Deleted job {sJobId}");
            }
        }

        public static void UpdateCronExpression(string sJobId, string sCronExpression)
        {
            var curShedule = GetCurrentScheduler();
            if (curShedule == null) return;
            var lstExecutingJobs = GetAllJobs();
            foreach (var executingJob in lstExecutingJobs)
            {
                if (executingJob.Key.Name != sJobId) continue;
                var allTriggerKeys = curShedule.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
                foreach (var triggerKey in allTriggerKeys)
                {
                    var trigger = curShedule.GetTrigger(triggerKey);
                    var triggerBuilder = trigger.GetTriggerBuilder();
                    triggerBuilder.WithCronSchedule(sCronExpression);
                    trigger = triggerBuilder.Build();
                    if (_currentScheduler.CheckExists(executingJob.Key)) _currentScheduler.DeleteJob(executingJob.Key);
                    _currentScheduler.ScheduleJob(trigger);
                }
            }
        }
    }
}