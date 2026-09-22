using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using TiSun;
using TSFramework.Core.Consts;
using TSFramework.Core.Helpers;
using TSFramework.Core.Members.Procedure;
using TSFramework.Plugable.Interfaces;

namespace TSFramework.Core.Providers
{
    public class StoreProcedureProvider
    {
        private static string StoredProcedureConfigPath =>
            ConfigurationManager.AppSettings[AppSettingConst.AppProcedureConfigPathKey];

        private static string _providerName { get; set; }

        private static XmlProviders _xmlProviders { get; set; }

        private static XmlProviders XmlProviders
        {
            get
            {
                //if (_xmlProviders?.Procedures != null) return _xmlProviders;
                var xmlProcedureFullPath = HostingEnvironment.MapPath(StoredProcedureConfigPath);
                var allProcedureXmlFiles = Directory.GetFiles(xmlProcedureFullPath, "*", SearchOption.AllDirectories);
                foreach (var procedureXmlFile in allProcedureXmlFiles)
                    using (var reader = XmlReader.Create(procedureXmlFile))
                    {
                        var serializer = new XmlSerializer(typeof(XmlProviders));
                        var procedureXmlProvider = (XmlProviders) serializer.Deserialize(reader);
                        if (procedureXmlProvider == null || procedureXmlProvider.Procedures.Count == 0) continue;
                        if (_xmlProviders == null)
                        {
                            _xmlProviders = procedureXmlProvider;
                            continue;
                        }

                        procedureXmlProvider.Procedures.ForEach(p =>
                        {
                            var providerProcedure =
                                _xmlProviders.Procedures.FirstOrDefault(e => e.ProviderName == p.ProviderName);
                            if (providerProcedure == null)
                                _xmlProviders.Procedures.Add(new XmlProcedure
                                    {ProviderName = p.ProviderName, Procedures = p.Procedures.Clone()});
                            else
                                //p.Procedures.Where(sp => !providerProcedure.Procedures.Exists(pp => sp.Value == pp.Value && sp.Name == pp.Name)).ToList().ForEach(
                                //    sp =>
                                //    {
                                //        providerProcedure.Procedures.Add(sp);
                                //    });
                                p.Procedures.ForEach(sp =>
                                {
                                    if (providerProcedure.Procedures.Exists(pp =>
                                        sp.Value == pp.Value && sp.Name == pp.Name)) return;
                                    providerProcedure.Procedures.Add(sp);
                                });

                            //if (!_xmlProviders.Procedures.Exists(e => e.ProviderName == p.ProviderName)) return;
                            //{
                            //    var lstCurrentProcedure =
                            //        _xmlProviders.Procedures.FirstOrDefault(e => e.ProviderName == p.ProviderName);
                            //    if (lstCurrentProcedure == null) return;
                            //    var procedureNotExists = p.Procedures.Where(sp =>
                            //        !lstCurrentProcedure.Procedures.Exists(eSp => eSp.Name == sp.Name)).ToList();
                            //    if (procedureNotExists.Any())
                            //    {
                            //        lstCurrentProcedure.Procedures.AddRange(procedureNotExists.ToArray());
                            //    }
                            //}
                        });
                    }


                return _xmlProviders;
            }
        }

        public Dictionary<string, IStoreProcedure> DicStoreProceduresProvider { get; private set; }

        public static StoreProcedureProvider Instance()
        {
            var storeProceduresProvider = new Dictionary<string, IStoreProcedure>();
            var spProviders =
                LibraryProvider<IStoreProcedure>.LoadLibrary(
                    ConfigurationManager.AppSettings[AppSettingConst.LibraryProcedurorKey] ?? "SPProvider");

            if (spProviders != null)
                XmlProviders.Procedures.ForEach(xp =>
                {
                    var dProvider = DataService.GetInstance(xp.ProviderName);
                    var libSpProvider =
                        spProviders.ToList().FirstOrDefault(sp => sp.ProviderType == dProvider.GetType());
                    if (libSpProvider == null) return;
                    if (storeProceduresProvider.Keys.Contains(xp.ProviderName)) return;
                    storeProceduresProvider.Add(xp.ProviderName,
                        libSpProvider.Instance(xp.ProviderName,
                            xp.Procedures.Select(p => new {p.Name, p.Value}).Distinct()
                                .ToDictionary(d => d.Name, d => d.Value)));
                });

            //storeProceduresProvider.Add("UrencoProvider", new SQLStoreProcedure(
            //    "UrencoProvider", XmlProviders.Procedures
            //        .First(xp => xp.ProviderName == "UrencoProvider")?.Procedures
            //        .Select(p => new { p.Name, p.Value }).ToDictionary(d => d.Name, d => d.Value)
            //));

            var nSpProviders = new StoreProcedureProvider {DicStoreProceduresProvider = storeProceduresProvider};

            return nSpProviders;
        }

        public static StoreProcedureProvider Instance(string providerName)
        {
            _providerName = providerName;
            var storeProceduresProvider = new Dictionary<string, IStoreProcedure>();
            var spProviders =
                LibraryProvider<IStoreProcedure>.LoadLibrary(
                    ConfigurationManager.AppSettings[AppSettingConst.LibraryProcedurorKey] ?? "SPProvider");
            var dProvider = DataService.GetInstance(providerName);

            if (spProviders != null)
                XmlProviders.Procedures.Where(xp => xp.ProviderName == providerName).ToList()
                    .ForEach(xp =>
                    {
                        var libSpProvider =
                            spProviders.ToList().FirstOrDefault(sp => sp.ProviderType == dProvider.GetType());
                        if (libSpProvider == null) return;
                        if (storeProceduresProvider.Keys.Contains(xp.ProviderName)) return;
                        storeProceduresProvider.Add(xp.ProviderName,
                            libSpProvider.Instance(xp.ProviderName,
                                xp.Procedures.Select(p => new {p.Name, p.Value})
                                    .ToDictionary(d => d.Name, d => d.Value)));
                    });

            var nSpProviders = new StoreProcedureProvider {DicStoreProceduresProvider = storeProceduresProvider};

            return nSpProviders;
        }

        public int? Execute(string spName, string providerName, params object[] p)
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return null;
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            return storeProcedureProvider?.ExecuteProcedure(spName, p);
        }

        public DataTable ExecuteProcedure(string spName, string providerName, params object[] p)
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return null;
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            return storeProcedureProvider?.ExecuteProcedureDataTable(spName, p);
        }

        public object ExecuteScalar(string spName, string providerName, params object[] p)
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return null;
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            return storeProcedureProvider?.ExecuteProcedureScalar(spName, p);
        }

        public object ExecuteScalar<T>(string spName, string providerName, params object[] p)
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return null;
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            return storeProcedureProvider?.ExecuteProcedureScalar<T>(spName, p);
        }

        public T ExecuteScalarObject<T>(string spName, string providerName, params object[] p) where T : new()
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return default(T);
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            var lstDatas = storeProcedureProvider?.ExecuteProcedureTypedList<T>(spName, p);
            if (lstDatas == null || !lstDatas.Any()) return default(T);
            return lstDatas[0];
        }

        public List<T> ExecuteTypedList<T>(string spName, string providerName, params object[] p) where T : new()
        {
            if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(providerName)) return null;
            //var providerName = XmlProviders.Procedures.FirstOrDefault(sp => sp.Procedures.Exists(a => a.Name == spName))?.ProviderName ?? string.Empty;

            if (string.IsNullOrEmpty(providerName)) return null;

            var storeProcedureProvider = DicStoreProceduresProvider[providerName];
            return storeProcedureProvider?.ExecuteProcedureTypedList<T>(spName, p);
        }
    }
}