using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using CenIT.ReportTourism.Core.Attributes;
using CenIT.ReportTourism.Core.Interfaces;
using TSFramework.Core.Providers;

namespace CenIT.ReportTourism.Modules.ReportModule.Providers
{
    public static class ReportPlugableProvider
    {
        public static List<IPlugableReport> LoadReports()
        {
            return LibraryProvider<IPlugableReport>
                .LoadLibrary(ConfigurationManager.AppSettings["App_ReportPlugin_Folder"] ??
                             "/Contents/Modules/Report/Reports")
                .ToList();
        }

        public static IPlugableReport GetReportByKey(string reportKey)
        {
            var lstReports = LoadReports();
            var reportMatch = lstReports.Where(p => p.ReportKey == reportKey).ToList();
            return reportMatch.Count > 0 ? reportMatch.First() : null;
        }

        public static IPlugableReport GetReport(string assemblyPath)
        {
            //Assembly reportAssembly = Assembly.LoadFrom(assemblyPath);
            var reportAssembly = Assembly.Load(File.ReadAllBytes(assemblyPath));

            var availableTypes = new List<Type>();
            availableTypes.AddRange(reportAssembly.GetTypes());

            // get a list of objects that implement the IPlugableReport interface AND have the ReportReportAttribute
            var plugableReportList = availableTypes.FindAll(delegate(Type t)
            {
                var interfaceTypes = new List<Type>(t.GetInterfaces());
                var arr = t.GetCustomAttributes(typeof(ReportPluginAttribute), true);
                return arr.Length != 0 && interfaceTypes.Contains(typeof(IPlugableReport));
            });

            // conver the list of Objects to an instantiated list of ICalculators
            return plugableReportList.ConvertAll(t => Activator.CreateInstance(t) as IPlugableReport).First();
        }
    }
}