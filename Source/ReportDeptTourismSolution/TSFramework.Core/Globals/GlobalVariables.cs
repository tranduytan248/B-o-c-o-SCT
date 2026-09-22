using System.Configuration;
using TSFramework.Core.Consts;

namespace TSFramework.Core.Globals
{
    public static class GlobalVariables
    {
        private static string _appLanguageCode = "en-US";
        private static int _cookieExpired = 7;
        private static bool _hasEncrypt;

        public static string LanguageCode
        {
            get
            {
                var sAppLanguageCode = ConfigurationManager.AppSettings[AppSettingConst.LanguageCodeKey];
                _appLanguageCode = string.IsNullOrEmpty(sAppLanguageCode) ? _appLanguageCode : sAppLanguageCode;

                return _appLanguageCode;
            }
            set { _appLanguageCode = value; }
        }

        public static int CookieExpired
        {
            get
            {
                var sCookieExpired = ConfigurationManager.AppSettings[AppSettingConst.CookieExpiredKey];
                _cookieExpired = string.IsNullOrEmpty(sCookieExpired) ? _cookieExpired : int.Parse(sCookieExpired);
                return _cookieExpired;
            }
        }

        public static bool HasEncrypt
        {
            get
            {
                var sHasEncrypt = ConfigurationManager.AppSettings[AppSettingConst.HasEncryptKey];
                if (string.IsNullOrEmpty(sHasEncrypt)) _hasEncrypt = false;

                if (sHasEncrypt != null && (sHasEncrypt == "1" || sHasEncrypt.ToLower() == "true")) _hasEncrypt = true;

                if (sHasEncrypt != null && (sHasEncrypt == "0" || sHasEncrypt.ToLower() == "false"))
                    _hasEncrypt = false;
                return _hasEncrypt;
            }
        }
    }
}