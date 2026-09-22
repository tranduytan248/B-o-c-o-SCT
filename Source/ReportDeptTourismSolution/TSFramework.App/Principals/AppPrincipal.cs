using System;
using System.Linq;
using System.Security.Principal;
using TSFramework.Plugable.Interfaces;

namespace TSFramework.App.Principals
{
    public class AppPrincipal : IBasePrincipal
    {
        public AppPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }

        public string[] Roles { get; set; }

        public string Token { get; set; }

        public DateTime? CreatedDate { get; set; }
        public IIdentity Identity { get; }

        public int UserId { get; set; }
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsInRole(string role)
        {
            return Roles.Any(r => role.Contains(r));
        }
    }
}