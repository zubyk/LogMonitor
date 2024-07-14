using LogMonitor.Models;

namespace LogMonitor
{
    public static class UserRoleEnumExtension
    {
        private static readonly string _adminRoleName;
        private static readonly string _operatorRoleName;
        private static readonly string _supervisorRoleName;
        private static readonly string _userRoleName;

        private static readonly string _adminPolicyName;
        private static readonly string _operatorPolicyName;
        private static readonly string _supervisorPolicyName;
        private static readonly string _userPolicyName;

        static UserRoleEnumExtension()
        {
            _adminRoleName = UserRoleEnum.Administrator.ToString();
            _operatorRoleName = UserRoleEnum.Operator.ToString();
            _supervisorRoleName = UserRoleEnum.Supervisor.ToString();
            _userRoleName = UserRoleEnum.User.ToString();

            _adminPolicyName = $"{_adminRoleName}Policy";
            _operatorPolicyName = $"{_operatorRoleName}Policy";
            _supervisorPolicyName = $"{_supervisorPolicyName}Policy";
            _userPolicyName = $"{_userPolicyName}Policy";
        }

        public static string ToRoleName(this UserRoleEnum self)
        {
            switch (self)
            {
                case UserRoleEnum.Administrator:
                    return _adminRoleName;
                case UserRoleEnum.Operator:
                    return _operatorRoleName;
                case UserRoleEnum.Supervisor:
                    return _supervisorRoleName;
                default:
                    return _userRoleName;
            }
        }

        public static string ToPolicyName(this UserRoleEnum self)
        {
            switch (self)
            {
                case UserRoleEnum.Administrator:
                    return _adminPolicyName;
                case UserRoleEnum.Operator:
                    return _operatorPolicyName;
                case UserRoleEnum.Supervisor:
                    return _supervisorPolicyName;
                default:
                    return _userPolicyName;
            }
        }
    }
}
