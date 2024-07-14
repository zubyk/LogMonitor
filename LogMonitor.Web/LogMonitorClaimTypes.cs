using LogMonitor.ViewModels;

namespace LogMonitor
{
    public static class LogMonitorClaimTypes
    {
        public static readonly string IsProcessed = $"LogMonitor/identity/claims/isprocessed";

        public static readonly string UserDisplayShortName = $"LogMonitor/identity/claims/{nameof(UserViewModel.DisplayShortName)}";
        public static readonly string UserDisplayName = $"LogMonitor/identity/claims/{nameof(UserViewModel.DisplayName)}";
        public static readonly string UserRole = $"LogMonitor/identity/claims/{nameof(UserViewModel.UserRole)}";
    }
}
