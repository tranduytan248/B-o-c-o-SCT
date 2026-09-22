using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TSFramework.Core.Members.Job;

namespace TSFramework.Core.Providers
{
    public class JobProvider
    {
        public static string GenerateJobId(int iLength = 10)
        {
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char) e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char) e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(iLength)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        public static string GenerateTriggerId(int iLength = 10)
        {
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char) e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char) e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(iLength)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        public static string GenerateGroupId(int iLength = 10)
        {
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char) e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char) e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(iLength)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        public static List<JobScheduleModel> ReadScheduleFromFileXml(string pathXmlFile)
        {
            var xmlDoc = XDocument.Load(pathXmlFile);
            if (xmlDoc.Root == null) return null;
            var lstSchedules = xmlDoc.Root.Elements()
                .Select(a => new JobScheduleModel
                {
                    JobName = (string) a.Attribute("name"),
                    CronExpression = (string) a.Attribute("value"),
                    TypePeriodic = int.Parse((string) a.Attribute("typePeriodic") ?? "0")
                }).ToList();
            return lstSchedules;
        }
    }
}