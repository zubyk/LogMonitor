using LogMonitor.Models;
using System.Security.Claims;

namespace LogMonitor.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; }
        public string DisplayName { get; }
        public string DisplayShortName { get; }
        public bool ExistsInAD { get; }
        public UserRoleEnum UserRole { get; }
        public bool IsAdministrator { get; }
        public bool IsSupervisor { get; }
        public bool IsOperator { get; }
        public bool ExistsInDB { get; }

        public UserViewModel(string userName, UserRoleEnum role, bool existsInDB = false, string displayName = null, bool existsInAD = false)
        {
            UserName = userName;

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                DisplayName = displayName;
                var names = displayName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Select(n => n.TrimEnd('.'));

                DisplayShortName = (names.First() + " " + string.Join(" ", names.Skip(1).Take(2).Select(n => n[0].ToString().ToUpper() + "."))).Trim();
            }

            ExistsInAD = existsInAD;
            ExistsInDB = existsInDB;

            UserRole = role;
            IsAdministrator = UserRole.HasFlag(UserRoleEnum.Administrator);
            IsSupervisor = UserRole.HasFlag(UserRoleEnum.Supervisor);
            IsOperator = UserRole.HasFlag(UserRoleEnum.Operator);
        }

        public UserViewModel(ClaimsPrincipal principal)
        {
            UserName = principal.FindFirst(ClaimTypes.Name)?.Value;
            DisplayName = principal.FindFirst(LogMonitorClaimTypes.UserDisplayName)?.Value;
            DisplayShortName = principal.FindFirst(LogMonitorClaimTypes.UserDisplayShortName)?.Value;

            ExistsInAD = !string.IsNullOrWhiteSpace(DisplayName);

            UserRole = UserRoleEnum.User;

            foreach (UserRoleEnum role in Enum.GetValues(typeof(UserRoleEnum)))
            {
                if (principal.IsInRole(role))
                {
                    UserRole |= role;
                    ExistsInDB = true;
                }
            }

            IsAdministrator = UserRole.HasFlag(UserRoleEnum.Administrator);
            IsSupervisor = UserRole.HasFlag(UserRoleEnum.Supervisor);
            IsOperator = UserRole.HasFlag(UserRoleEnum.Operator);
        }
    }
}
