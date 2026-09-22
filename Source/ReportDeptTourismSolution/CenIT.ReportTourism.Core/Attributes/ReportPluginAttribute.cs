using System;

namespace CenIT.ReportTourism.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportPluginAttribute : Attribute
    {
        public ReportPluginAttribute(string reportKey, string reportName, string description)
        {
            ReportKey = reportKey;
            ReportName = reportName;
            Description = description;
        }

        public string ReportKey { get; }

        public string ReportName { get; }

        public string Description { get; }
    }
}