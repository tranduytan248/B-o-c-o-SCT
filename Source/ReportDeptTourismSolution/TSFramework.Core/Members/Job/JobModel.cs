using System;
using Quartz;

namespace TSFramework.Core.Members.Job
{
    public abstract class JobModel : IJob
    {
        public bool IsActive = true;
        public string JobId { get; private set; }
        public string GroupId { get; private set; }
        public string Description { get; private set; }

        private DailyTimeIntervalScheduleBuilder DailyTimeIntervalSchedule { get; } =
            DailyTimeIntervalScheduleBuilder.Create();

        public TriggerBuilder Trigger { get; set; } = TriggerBuilder.Create();

        public virtual void Execute(IJobExecutionContext context)
        {
        }

        public JobModel SetJobId(string jobId)
        {
            JobId = jobId;
            return this;
        }

        public JobModel SetGroupId(string groupId)
        {
            GroupId = groupId;
            return this;
        }

        public JobModel SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public JobModel SetTriggerId(string sTriggerId)
        {
            Trigger?.WithIdentity(sTriggerId);
            return this;
        }

        public JobModel UsingJobData(JobDataMap jobDataMap)
        {
            Trigger?.UsingJobData(jobDataMap);
            return this;
        }

        public JobModel UsingJobData(string sKey, bool dataValue)
        {
            Trigger?.UsingJobData(sKey, dataValue);
            return this;
        }

        public JobModel UsingJobData(string sKey, decimal dataValue)
        {
            Trigger?.UsingJobData(sKey, dataValue);
            return this;
        }

        public JobModel UsingJobData(string sKey, double dataValue)
        {
            Trigger?.UsingJobData(sKey, dataValue);
            return this;
        }

        public JobModel UsingJobData(string sKey, float dataValue)
        {
            Trigger?.UsingJobData(sKey, dataValue);
            return this;
        }

        public JobModel UsingJobData(string sKey, string dataValue)
        {
            Trigger?.UsingJobData(sKey, dataValue);
            return this;
        }

        public JobModel WithCronSchedule(string cronExpression)
        {
            Trigger.WithCronSchedule(cronExpression);
            return this;
        }

        public JobModel EndAt(DateTime? dateTimeOffset)
        {
            if (dateTimeOffset != null) Trigger.EndAt(new DateTimeOffset(dateTimeOffset.Value));
            return this;
        }

        public JobModel StartAt(DateTime dateTimeOffset)
        {
            Trigger.StartAt(new DateTimeOffset(dateTimeOffset));
            return this;
        }

        public JobModel StartNow()
        {
            Trigger.StartNow();
            return this;
        }

        public JobModel WithDailyTimeSchedule_EndingDailyAfterCount(int iCount)
        {
            DailyTimeIntervalSchedule.EndingDailyAfterCount(iCount);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_IntervalInHours(int iHour)
        {
            DailyTimeIntervalSchedule.WithIntervalInHours(iHour);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_EndingDailyAt(TimeOfDay atTime)
        {
            DailyTimeIntervalSchedule.EndingDailyAt(atTime);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_OnEveryDay()
        {
            DailyTimeIntervalSchedule.OnEveryDay();
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_StartingDailyAt(TimeOfDay atTime)
        {
            DailyTimeIntervalSchedule.StartingDailyAt(atTime);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_OnSaturdayAndSunday()
        {
            DailyTimeIntervalSchedule.OnSaturdayAndSunday();
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_WithInterval(int iCount, IntervalUnit unitType)
        {
            DailyTimeIntervalSchedule.WithInterval(iCount, unitType);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_OnMondayThroughFriday()
        {
            DailyTimeIntervalSchedule.OnMondayThroughFriday();
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_OnDaysOfTheWeek(params DayOfWeek[] onDays)
        {
            DailyTimeIntervalSchedule.OnDaysOfTheWeek(onDays);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_WithIntervalInSeconds(int iSeconds)
        {
            DailyTimeIntervalSchedule.WithIntervalInSeconds(iSeconds);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public JobModel WithDailyTimeSchedule_WithIntervalInMinutes(int iMinutes)
        {
            DailyTimeIntervalSchedule.WithIntervalInMinutes(iMinutes);
            Trigger.WithSchedule(DailyTimeIntervalSchedule);
            return this;
        }

        public virtual void ExecuteNow(object data)
        {
        }
    }
}