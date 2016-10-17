using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace PnPAutomationUI.Helpers
{
    public static class AuthorizationManager
    {

        public static bool IsAdmin(IPrincipal principal)
        {
            try
            {
                if (!principal.Identity.IsAuthenticated)
                {
                    return false;
                }

                using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
                {
                    var user = dc.UsersSets.Where(u => u.UPN.Equals(principal.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).First();
                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        return user.IsAdmin;
                    }
                }
            }
            catch (Exception ex)
            {
                //log error
                return false;
            }
        }

        public static bool IsCoreTeamMember(IPrincipal principal)
        {
            try
            {
                if (!principal.Identity.IsAuthenticated)
                {
                    return false;
                }

                using (PnPTestAutomationEntities dc = new PnPTestAutomationEntities())
                {
                    var user = dc.UsersSets.Where(u => u.UPN.Equals(principal.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).First();
                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        return user.IsCoreMember;
                    }
                }
            }
            catch (Exception ex)
            {
                //log error
                return false;
            }
        }
    }
}