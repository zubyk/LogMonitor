using LogMonitor.Models;
using System.Security.Claims;

namespace LogMonitor
{
    public static class ClaimsPrincipalExtension
    {
        public static bool IsInRole(this ClaimsPrincipal self, UserRoleEnum role)
        {
            var roleString = role.ToRoleName();

            try
            {
                foreach (var claim in self.FindAll(LogMonitorClaimTypes.UserRole))
                {
                    if (string.Equals(claim.Value, roleString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch { }


            return false;
        }

    }
}
