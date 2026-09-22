using System;

namespace TSFramework.Core.Members.Job
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobPluginAttribute : Attribute
    {
        public JobPluginAttribute(string pluginName, string description)
        {
            PluginName = pluginName;
            Description = description;
        }

        public string PluginName { get; }

        public string Description { get; }
    }
}