using System.Configuration;
using TSFramework.App.BaseApps;
using TSFramework.Core.Consts;
using TSFramework.Core.Providers;

namespace TSFramework.App.Processors
{
    public static class AppProcessor
    {
        private static StoreProcedureProvider _storeProceduror;
        private static AuthorityProvider _author;
        private static LogProvider _logger;
        private static JobSchedulerProvider _jobber;
        private static MessageProvider _messagor;
        private static NotificationProvider _notifider;
        private static MailProvider _mailer;

        public static StoreProcedureProvider ProcedureProvider =>
            _storeProceduror ?? (_storeProceduror = StoreProcedureProvider.Instance());

        public static AuthorityProvider Author => _author ?? (_author =
                                                      AuthorityProvider.Instance(
                                                          ProcedureProvider?.DicStoreProceduresProvider?[
                                                              ConfigurationManager.AppSettings[
                                                                  AppSettingConst.AppDefaultDataProviderKey]]));

        public static MessageProvider Messagor => _messagor ?? (_messagor =
                                                      MessageProvider.Instance(
                                                          ProcedureProvider.DicStoreProceduresProvider,
                                                          BaseAppContext.Current.CurrentLanguageCode));

        public static LogProvider Logger => _logger ?? (_logger = new LogProvider());
        public static JobSchedulerProvider Jobber => _jobber ?? (_jobber = new JobSchedulerProvider());
        public static NotificationProvider Notifider => _notifider ?? (_notifider = new NotificationProvider(Messagor));
        public static MailProvider Mailer => _mailer ?? (_mailer = MailProvider.Instance());
    }
}