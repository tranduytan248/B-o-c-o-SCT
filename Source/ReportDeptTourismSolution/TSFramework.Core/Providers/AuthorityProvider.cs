using System.Configuration;
using System.Linq;
using TSFramework.Core.Consts;
using TSFramework.Plugable.Interfaces;

namespace TSFramework.Core.Providers
{
    public class AuthorityProvider
    {
        private readonly IAuthority _authenticator;

        public AuthorityProvider(IStoreProcedure storeProceduror)
        {
            _authenticator = LoadAuthoriryLibs();
            _authenticator.ProcedureProvider = storeProceduror;
        }

        public static AuthorityProvider Instance(IStoreProcedure storeProceduror)
        {
            return new AuthorityProvider(storeProceduror);
        }

        private static IAuthority LoadAuthoriryLibs()
        {
            var authType = ConfigurationManager.AppSettings[AppSettingConst.AuthorityTypeKey];
            var libsAuthor =
                LibraryProvider<IAuthority>.LoadLibrary(
                    ConfigurationManager.AppSettings[AppSettingConst.LibraryAuthorKey] ?? "Authority");
            return libsAuthor.FirstOrDefault(lib => lib.GetType().Name == authType);
        }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            return _authenticator.IsAllow(userName, areaName, controllerName, actionName);
        }

        public bool IsValidUser(string userName, string passWord)
        {
            return _authenticator.IsValidUser(userName, passWord);
        }

        public AuthorizeData Login(string userName, string passWord)
        {
            return _authenticator.Login(userName, passWord);
        }

        public AuthorizeData Token(string userName, string passWord)
        {
            return _authenticator.Token(userName, passWord);
        }
    }
}