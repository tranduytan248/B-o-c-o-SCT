using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace TSFramework.Core.Providers
{
    public static class LibraryProvider<T>
    {
        public static T LoadLibraryByPath(string path)
        {
            path = HostingEnvironment.MapPath(path);

            if (path == null) return default(T);
            var assembly = Assembly.Load(File.ReadAllBytes(path));
            var pluginType = typeof(T);
            var types = assembly.GetTypes();
            foreach (var type in types)
                if (type.IsInterface || type.IsAbstract)
                {
                }
                else
                {
                    if (type.GetInterface(pluginType.FullName) == null) continue;
                    var plugin = (T) Activator.CreateInstance(type);
                    return plugin;
                }

            return default(T);
        }

        public static ICollection<T> LoadLibrary(string path)
        {
            path = HostingEnvironment.MapPath("/" + path);

            if (!Directory.Exists(path)) return null;
            var dllFileNames = Directory.GetFiles(path, "*.dll");

            var pluginType = typeof(T);
            ICollection<Type> pluginTypes = new List<Type>();

            foreach (var dllFile in dllFileNames)
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllFile));
                {
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                        if (type.IsInterface || type.IsAbstract)
                        {
                        }
                        else
                        {
                            if (type.GetInterface(pluginType.FullName) != null) pluginTypes.Add(type);
                        }
                }
            }

            ICollection<T> plugins = new List<T>(pluginTypes.Count);
            foreach (var type in pluginTypes)
            {
                var plugin = (T) Activator.CreateInstance(type);
                plugins.Add(plugin);
            }

            return plugins;
        }
    }
}