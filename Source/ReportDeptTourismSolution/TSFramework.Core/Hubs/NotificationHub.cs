using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace TSFramework.Core.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserNotificationModel> Users =
            new ConcurrentDictionary<string, UserNotificationModel>();

        #region Base Function

        /// <summary>
        ///     Event connection connect to hub
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var connectionId = Context.ConnectionId;

            #region Assign To Security Group

            if (Context.User?.Identity?.IsAuthenticated ?? false)
                Groups.Add(Context.ConnectionId, "Authenticated");
            else
                Groups.Add(Context.ConnectionId, "Anonymous");

            #endregion

            var user = Users.GetOrAdd(userName, _ => new UserNotificationModel
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1) Clients.Others.userConnected(userName);
            }

            return base.OnConnected();
        }

        /// <summary>
        ///     Event connection disconnect to hub
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var connectionId = Context.ConnectionId;

            UserNotificationModel user;
            Users.TryGetValue(userName, out user);

            if (user != null)
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserNotificationModel removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }

            #region Remove from security group

            Groups.Remove(Context.ConnectionId, "Authenticated");
            Groups.Remove(Context.ConnectionId, "Anonymous");

            #endregion

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region Hub Functions

        /// <summary>
        ///     User join group of Hub
        /// </summary>
        /// <param name="groupName">group name</param>
        public void JoinGroup(string groupName)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";

            UserNotificationModel user;
            Users.TryGetValue(userName, out user);
            if (user == null) return;
            lock (user.ConnectionIds)
            {
                if (user.ConnectionIds.Count <= 0) return;
                foreach (var connId in user.ConnectionIds)
                    Groups.Add(connId, groupName);
            }
        }

        /// <summary>
        ///     Send broadcast message to Authenticated User
        /// </summary>
        /// <param name="sMessage"></param>
        public void Broadcast(string sMessage)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Users.GetOrAdd(userName, _ => new UserNotificationModel
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });
            Clients.Group("Authenticated").OnMessage(userName, sMessage);
        }

        #endregion

        #region Extend Function

        /// <summary>
        ///     Send message to group user
        /// </summary>
        /// <param name="sGroupName"></param>
        /// <param name="sMessage"></param>
        [Authorize]
        public void SendToGroup(string sGroupName, string sMessage)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Clients.Group(sGroupName).OnMessage(userName, sMessage);
        }

        /// <summary>
        ///     Send message to specify user
        /// </summary>
        /// <param name="sReceiver"></param>
        /// <param name="sMessage"></param>
        [Authorize]
        public void SendToUser(string sReceiver, string sMessage)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Clients.User(sReceiver).OnMessage(userName, sMessage);
        }

        /// <summary>
        ///     Update status of task
        /// </summary>
        /// <param name="sForEmp"></param>
        /// <param name="sTaskId"></param>
        [Authorize]
        public void UpdateStatusTask(string sForEmp, string sTaskId)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Clients.User(sForEmp).OnUpdateTask(userName, sTaskId);
        }

        /// <summary>
        ///     Update status of task invoice
        /// </summary>
        /// <param name="sForEmp"></param>
        /// <param name="sFKey"></param>
        [Authorize]
        public void UpdateStatusTaskInv(string sForEmp, string sFKey)
        {
            var userName = Context.User?.Identity?.Name ?? "Anonymous";
            Clients.User(sForEmp).OnUpdateTaskInv(userName, sFKey);
        }

        #endregion
    }

    public class UserNotificationModel
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}