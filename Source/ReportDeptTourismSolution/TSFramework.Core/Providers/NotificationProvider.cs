using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TSFramework.Core.Enums;
using TSFramework.Core.Helpers;
using TSFramework.Core.Hubs;

namespace TSFramework.Core.Providers
{
    public class NotificationProvider
    {
        private static IHubContext _notifyHub;
        private static MessageProvider _message;

        public NotificationProvider(MessageProvider message)
        {
            _notifyHub = GlobalHost.ConnectionManager.GetHubContext<AppSignalRHub>();
            _message = message;
        }

        public string CreateMessage(string messageContent, EnumProcessType typeProcess, EnumMsgIcon icon,
            string sUrl = "", string sTarget = "")
        {
            var sType = string.Empty;
            var sIcon = string.Empty;
            var sMessage = string.Empty;

            switch (icon)
            {
                case EnumMsgIcon.Success:
                    sType = "success";
                    sIcon = "fa fa-check-circle";
                    break;
                case EnumMsgIcon.Error:
                    sType = "danger";
                    sIcon = "fa fa-exclamation-circle";
                    break;
                case EnumMsgIcon.Info:
                    sType = "info";
                    sIcon = "fa fa-info-circle";
                    break;
                case EnumMsgIcon.Warning:
                    sType = "warning";
                    sIcon = "fa fa-exclamation-triangle";
                    break;
            }

            switch (typeProcess)
            {
                case EnumProcessType.Add:
                    sMessage = _message.GetMessage("Common_Add" + icon);
                    break;
                case EnumProcessType.Edit:
                    sMessage = _message.GetMessage("Common_Update" + icon);
                    break;
                case EnumProcessType.Delete:
                    sMessage = _message.GetMessage("Common_Delete" + icon);
                    break;
                case EnumProcessType.DataExisted:
                    sMessage = _message.GetMessage("Common_DataExisted");
                    break;
                case EnumProcessType.DataNotExist:
                    sMessage = _message.GetMessage("Common_DataNotExist");
                    break;
                case EnumProcessType.NonFormat:
                    sMessage = _message.GetMessage("Common_NonFormat");
                    break;
            }

            var sTitle = _message.GetMessage($"{EnumHelper.GetDescription(icon)}_Title");
            return
                $"showNotify('{sTitle}', {string.Join(",", "'" + sIcon + "'", "'" + string.Format(sMessage, "<b>" + messageContent.Replace("'", @"\'") + "</b>") + "'", "'" + sUrl + "'", "'" + sTarget + "'", "'" + sType + "'")});";
        }

        public string CreateNotify(string messageContent, EnumProcessType typeProcess, EnumMsgIcon icon)
        {
            var sMessage = string.Empty;

            var sFunction = EnumHelper.GetDescription(icon);
            sFunction = sFunction?.ToLower();

            switch (typeProcess)
            {
                case EnumProcessType.Add:
                    sMessage = _message.GetMessage("Common_Add" + icon);
                    break;
                case EnumProcessType.Edit:
                    sMessage = _message.GetMessage("Common_Update" + icon);
                    break;
                case EnumProcessType.Delete:
                    sMessage = _message.GetMessage("Common_Delete" + icon);
                    break;
                case EnumProcessType.DataExisted:
                    sMessage = _message.GetMessage("Common_DataExisted");
                    break;
                case EnumProcessType.DataNotExist:
                    sMessage = _message.GetMessage("Common_DataNotExist");
                    break;
                case EnumProcessType.NonFormat:
                    sMessage = _message.GetMessage("Common_NonFormat");
                    break;
            }

            sMessage = string.Format(sMessage, "<b>" + messageContent.Replace("'", @"\'") + "</b>");
            return $"toastr.{sFunction}('{sMessage}');";
        }

        public void PushNotify(string sReceiver, string sMessage)
        {
            var sSender = "Sys";
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotify(string sReceiver, string sMessage, EnumProcessType typeProcess, EnumMsgIcon icon)
        {
            var sSender = "Sys";
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToGroup(string sSender, string sGroupName, string sMessage, EnumProcessType typeProcess,
            EnumMsgIcon icon)
        {
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.Group(sGroupName).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToGroup(string sSender, string sGroupName, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.Group(sGroupName).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToUser(string sSender, string sReceiver, string sMessage, EnumProcessType typeProcess,
            EnumMsgIcon icon)
        {
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToUser(string sSender, string sReceiver, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void BroadcastNotify(string sSender, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.Group("Authenticated").OnBroadcast(sSender, sMessage));
        }

        public void ForceLogout(string userName)
        {
            Task.Run(() => _notifyHub.Clients.User(userName).OnForceLogout(userName));
        }
    }
}