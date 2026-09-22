using System.Web.Security;
using TSFramework.Plugable.Interfaces;

namespace TSFramework.Plugable.Abstracts
{
    public abstract class AMembership : MembershipProvider
    {
        protected IAuthority Auth;

        protected AMembership(IAuthority authority)
        {
            Auth = authority;
        }
    }
}