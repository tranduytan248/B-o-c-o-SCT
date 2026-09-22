using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.SessionState;
using TSFramework.Core.Consts;
using TSFramework.Core.Globals;
using TSFramework.Plugable.Interfaces;

namespace TSFramework.App.BaseApps
{
    public class BaseAppContext
    {
        #region Authorization Data

        public IBasePrincipal User { get; set; }

        #endregion

        #region Static Properties

        private static readonly string DataKey = "AppContextStore";

        [ThreadStatic] private static BaseAppContext _currentContext;

        /// <summary>
        ///     Returns the current instance of the CMSContext from the ThreadData Slot. If one is not found and a valid
        ///     HttpContext can be found,
        ///     it will be used. Otherwise, an exception will be thrown.
        /// </summary>
        public static BaseAppContext Current
        {
            get
            {
                var httpContext = HttpContext.Current;

                if (httpContext != null)
                {
                    if (httpContext.Items.Contains(DataKey)) return httpContext.Items[DataKey] as BaseAppContext;

                    var context = new BaseAppContext(httpContext, true);
                    SaveContextToStore(context);
                    return context;
                }

                if (_currentContext == null)
                    throw new Exception(
                        "No BaseAppContext exists in the Current Application. AutoCreate fails since HttpContext.Current is not accessible.");
                return _currentContext;
            }
        }

        #endregion

        #region Static Function

        private static void SaveContextToStore(BaseAppContext context)
        {
            if (context.IsWebRequest)
                context.Context.Items[DataKey] = context;
            else
                _currentContext = context;
        }

        /// <summary>
        ///     Remove current context out of memmory
        /// </summary>
        public static void Unload()
        {
            _currentContext = null;
        }

        #endregion

        #region Properties

        #region Session context

        public HttpSessionState Session => Context.Session;

        #endregion

        private readonly HybridDictionary _items = new HybridDictionary();
        private Uri _currentUri;
        private string _hostPath;
        private string _rootPath;

        /// <summary>
        ///     Simulates Context.Items and provides a per request/instance storage bag
        /// </summary>
        private IDictionary Items => _items;

        /// <summary>
        ///     Provides direct access to the .Items property
        /// </summary>
        public object this[string key]
        {
            get { return Items[key]; }
            set { Items[key] = value; }
        }

        /// <summary>
        ///     Allows access to QueryString values
        /// </summary>
        private NameValueCollection QueryString { get; set; }

        private HttpContext Context { get; }

        /// <summary>
        ///     Quick check to see if we have a valid web reqeust. Returns false if HttpContext == null
        /// </summary>
        private bool IsWebRequest => Context != null;

        private string SiteUrl { get; set; }

        private Uri CurrentUri
        {
            get
            {
                if (_currentUri == null)
                    _currentUri = new Uri(@"/");

                return _currentUri;
            }
            set { _currentUri = value; }
        }

        /// <summary>
        /// </summary>
        public string HostPath
        {
            get
            {
                if (_hostPath != null) return _hostPath;
                var portInfo = CurrentUri.Port == 80 ? string.Empty : ":" + CurrentUri.Port;
                _hostPath = $"{CurrentUri.Scheme}://{CurrentUri.Host}{portInfo}";

                return _hostPath;
            }
        }

        private string RootPath()
        {
            if (_rootPath != null) return _rootPath;
            _rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var dirSep = Path.DirectorySeparatorChar.ToString();

            _rootPath = _rootPath.Replace("/", dirSep);

            return _rootPath;
        }

        #endregion

        #region Init function

        private void Initialize(NameValueCollection qs, Uri uri, string siteUrl)
        {
            QueryString = qs;
            SiteUrl = siteUrl;
            _currentUri = uri;
        }

        /// <summary>
        ///     cntr called when no HttpContext is available
        /// </summary>
        private BaseAppContext(Uri uri, string siteUrl)
        {
            Initialize(new NameValueCollection(), uri, siteUrl);
        }

        /// <summary>
        ///     cnst called when HttpContext is avaiable
        /// </summary>
        /// <param name="context"></param>
        private BaseAppContext(HttpContext context, bool includeQueryString)
        {
            Context = context;

            Initialize(includeQueryString ? new NameValueCollection(context.Request.QueryString) : null,
                context.Request.Url, GetSiteUrl());
        }

        #endregion

        #region URL context

        public string MapPath(string path)
        {
            return Context != null
                ? Context.Server.MapPath(path)
                : PhysicalPath(path.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("~", ""));
        }

        private string PhysicalPath(string path)
        {
            return string.Concat(RootPath().TrimEnd(Path.DirectorySeparatorChar),
                Path.DirectorySeparatorChar.ToString(), path.TrimStart(Path.DirectorySeparatorChar));
        }

        private string GetSiteUrl()
        {
            var hostName = Context.Request.Url.Host.Replace("www.", string.Empty);
            var applicationPath = Context.Request.ApplicationPath;

            if (applicationPath != null && applicationPath.EndsWith("/"))
                applicationPath = applicationPath.Remove(applicationPath.Length - 1, 1);
            return hostName + applicationPath;
        }

        #endregion

        #region Language Context

        private string _currentLanguageCode;

        /// <summary>
        ///     Get current language code
        /// </summary>

        public string CurrentLanguageCode
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentLanguageCode)) return _currentLanguageCode;
                //Try to get from cookie
                var cookie =
                    HttpContext.Current.Request.Cookies[AppSettingConst.ContextCurrentLanguageCodeConst];
                _currentLanguageCode = cookie != null ? cookie.Value : GlobalVariables.LanguageCode;

                return _currentLanguageCode;
            }
            set
            {
                _currentLanguageCode = value;
                WriteLanguageIdToCookie(_currentLanguageCode);
            }
        }

        private void WriteLanguageIdToCookie(string languaugeCode)
        {
            if (HttpContext.Current.Request.Cookies[AppSettingConst.ContextCurrentLanguageCodeConst] != null)
            {
                HttpContext.Current.Request.Cookies[AppSettingConst.ContextCurrentLanguageCodeConst].Value =
                    languaugeCode;
                HttpContext.Current.Response.Cookies.Set(
                    HttpContext.Current.Request.Cookies[AppSettingConst.ContextCurrentLanguageCodeConst]);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Set(
                    new HttpCookie(AppSettingConst.ContextCurrentLanguageCodeConst, languaugeCode));
            }

            //Cookie will be expired in 7 days
            HttpContext.Current.Response.Cookies[AppSettingConst.ContextCurrentLanguageCodeConst].Expires =
                DateTime.Now.AddDays(GlobalVariables.CookieExpired);
        }

        #endregion
    }
}