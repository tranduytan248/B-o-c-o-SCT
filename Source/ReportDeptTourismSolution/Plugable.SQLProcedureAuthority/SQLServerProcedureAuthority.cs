using System;
using System.Data;
using System.Linq;
using TSFramework.Plugable.Interfaces;

namespace Plugable.SQLProcedureAuthority
{
    public class PermissionModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
    }

    public class SQLServerProcedureAuthority : IAuthority
    {
        public IStoreProcedure ProcedureProvider { get; set; }

        public bool IsAllow(string userName, string areaName, string controllerName, string actionName)
        {
            var spCheckPermission = "Sys_Permission_IsAllow";

            var permissionObj = ProcedureProvider?.ExecuteProcedureTypedList<PermissionModel>(spCheckPermission, userName,
                areaName, controllerName, actionName);

            return permissionObj != null && permissionObj.Count > 0;
        }

        public bool IsValidUser(params object[] paramLogin)
        {
            var spCheckLogin = "Sys_User_Login";
            var permissionObj = ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spCheckLogin, paramLogin.ToArray());

            return permissionObj != null && permissionObj.Count > 0;
        }

        public AuthorizeData Login(params object[] paramLogin)
        {
            var spCheckLogin = "Sys_User_Login";
            var authDatas =
                ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spCheckLogin, paramLogin.ToArray());
            return authDatas?[0];
        }

        public AuthorizeData Register(params object[] paramRegister)
        {
            throw new NotImplementedException();
        }

        public AuthorizeData Token(params object[] paramLogin)
        {
            var spCheckToken = "Sys_User_Token";

            var authDatas =
                ProcedureProvider.ExecuteProcedureTypedList<AuthorizeData>(spCheckToken, paramLogin.ToArray());
            return authDatas?[0];
        }
    }
}