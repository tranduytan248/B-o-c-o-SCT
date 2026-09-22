using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TSFramework.Core.Members.Job;

namespace TSFramework.Core.Providers
{
    public class JobPlugableProvider
    {
        public static List<IJobPlugable> LoadPlugable(string folderPath)
        {
            return LibraryProvider<IJobPlugable>.LoadLibrary(folderPath).ToList();
        }

        public static IJobPlugable LoadPlugin(byte[] dataPluginBytes)
        {
            var pluginAssembly = Assembly.Load(dataPluginBytes);

            var availableTypes = new List<Type>();
            availableTypes.AddRange(pluginAssembly.GetTypes());

            // get a list of objects that implement the IJobPlugable interface AND have the RulePluginAttribute
            var plugableRuleList = availableTypes.FindAll(delegate(Type t)
            {
                var interfaceTypes = new List<Type>(t.GetInterfaces());
                var arr = t.GetCustomAttributes(typeof(JobPluginAttribute), true);
                return arr.Length != 0 && interfaceTypes.Contains(typeof(IJobPlugable));
            });

            // conver the list of Objects to an instantiated list of ICalculators
            return plugableRuleList.ConvertAll(t => Activator.CreateInstance(t) as IJobPlugable)?.First();
        }

        public static IJobPlugable GetPlugin(string assemblyPath)
        {
            //Assembly pluginAssembly = Assembly.LoadFrom(assemblyPath);
            var pluginAssembly = Assembly.Load(File.ReadAllBytes(assemblyPath));

            var availableTypes = new List<Type>();
            availableTypes.AddRange(pluginAssembly.GetTypes());

            // get a list of objects that implement the IJobPlugable interface AND have the RulePluginAttribute
            var plugableRuleList = availableTypes.FindAll(delegate(Type t)
            {
                var interfaceTypes = new List<Type>(t.GetInterfaces());
                var arr = t.GetCustomAttributes(typeof(JobPluginAttribute), true);
                return arr.Length != 0 && interfaceTypes.Contains(typeof(IJobPlugable));
            });

            // conver the list of Objects to an instantiated list of ICalculators
            return plugableRuleList.ConvertAll(t => Activator.CreateInstance(t) as IJobPlugable).First();
        }

        public static IJobPlugable GetJobPlugable(string assemblyPath)
        {
            var pluginAssembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            var pluginType = typeof(IJobPlugable);

            {
                var types = pluginAssembly.GetTypes();
                foreach (var type in types)
                    if (type.IsInterface || type.IsAbstract)
                    {
                    }
                    else
                    {
                        if (type.GetInterface(pluginType.FullName) == null) continue;
                        var plugin = (IJobPlugable) Activator.CreateInstance(type);
                        return plugin;
                    }
            }

            return default(IJobPlugable);
        }

        public static IJobPlugable LoadJobPlugable(byte[] dataPluginBytes)
        {
            var pluginAssembly = Assembly.Load(dataPluginBytes);
            var pluginType = typeof(IJobPlugable);

            {
                var types = pluginAssembly.GetTypes();
                foreach (var type in types)
                    if (type.IsInterface || type.IsAbstract)
                    {
                    }
                    else
                    {
                        if (type.GetInterface(pluginType.FullName) == null) continue;
                        var plugin = (IJobPlugable) Activator.CreateInstance(type);
                        return plugin;
                    }
            }

            return default(IJobPlugable);
        }

        public static List<IJobPlugable> GetPlugIns(List<Assembly> assemblies)
        {
            var availableTypes = new List<Type>();

            foreach (var currentAssembly in assemblies) availableTypes.AddRange(currentAssembly.GetTypes());

            // get a list of objects that implement the IJobPlugable interface AND 
            // have the RulePluginAttribute
            var plugableRuleList = availableTypes.FindAll(delegate(Type t)
            {
                var interfaceTypes = new List<Type>(t.GetInterfaces());
                var arr = t.GetCustomAttributes(typeof(JobPluginAttribute), true);
                return arr.Length != 0 && interfaceTypes.Contains(typeof(IJobPlugable));
            });

            // conver the list of Objects to an instantiated list of ICalculators
            return plugableRuleList.ConvertAll(t => Activator.CreateInstance(t) as IJobPlugable);
        }
    }
}