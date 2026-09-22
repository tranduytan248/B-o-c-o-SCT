using System.ComponentModel;
using TSFramework.App.Processors;

namespace TSFramework.App.Attributes
{
    public class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _resourceName;

        public CustomDisplayNameAttribute(string resourceName)
        {
            _resourceName = resourceName;
        }

        public override string DisplayName => AppProcessor.Messagor.GetMessage(_resourceName);
    }
}