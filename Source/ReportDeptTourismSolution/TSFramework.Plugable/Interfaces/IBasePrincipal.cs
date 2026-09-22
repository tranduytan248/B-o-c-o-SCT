using System.Security.Principal;

namespace TSFramework.Plugable.Interfaces
{
    public interface IBasePrincipal : IPrincipal
    {
        int UserId { get; set; }
        string UserName { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
    }
}