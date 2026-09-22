using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.WebPages.Razor.Configuration;
using RazorEngine.Templating;

namespace TSFramework.Core.Providers
{
    public class RenderTemplateHtmlProvider
    {
        public static string RenderStringHtml(string fullPathTemplate, object model)
        {
            if (!File.Exists(fullPathTemplate)) return string.Empty;
            var contentHtmlTemplate = File.ReadAllText(fullPathTemplate);
            var templateService = new TemplateService();
            // Add the default namespaces that will be automatically imported in all template classes
            AddDefaultNamespacesFromWebConfig(templateService);
            var emailHtmlBody = templateService.Parse(contentHtmlTemplate, model, null, null);
            return emailHtmlBody;
        }

        /// <summary>
        ///     Add the namespaces found in the ASP.NET MVC configuration section of the Web.config file to the provided
        ///     TemplateService instance.
        /// </summary>
        private static void AddDefaultNamespacesFromWebConfig(ITemplateService templateService)
        {
            var webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");
            if (!File.Exists(webConfigPath))
                return;

            var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = webConfigPath};
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var razorConfig = configuration.GetSection("system.web.webPages.razor/pages") as RazorPagesSection;

            if (razorConfig == null)
                return;

            foreach (NamespaceInfo namespaceInfo in razorConfig.Namespaces)
                templateService.AddNamespace(namespaceInfo.Namespace);
        }
    }
}