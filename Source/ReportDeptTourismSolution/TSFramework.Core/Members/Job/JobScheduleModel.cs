namespace TSFramework.Core.Members.Job
{
    public class JobScheduleModel
    {
        public string JobName { get; set; }
        public string CronExpression { get; set; }
        public int TypePeriodic { get; set; }
    }
}