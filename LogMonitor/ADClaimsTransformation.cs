using LogMonitor.Models;
using LogMonitor.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace LogMonitor
{
    public class ADClaimsTransformation : IClaimsTransformation
    {
        private readonly ILogger<ADClaimsTransformation> _logger;
        private readonly UserService _userService;

        public ADClaimsTransformation(ILogger<ADClaimsTransformation> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall(args: principal);

                try
                {
                    if (principal.Identity?.IsAuthenticated == true)
                    {
                        if (principal.FindFirst(LogMonitorClaimTypes.IsProcessed) is not null) return principal;

                        var identity = principal.Identity as ClaimsIdentity;

                        var userId = identity?.Name;

                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            var user = await _userService.GetUserAsync(userId);

                            if (user is not null)
                            {
                                if (identity is null)
                                {
                                    identity = new ClaimsIdentity();
                                }

                                foreach (UserRoleEnum role in Enum.GetValues(typeof(UserRoleEnum)))
                                {
                                    if (user.UserRole.HasFlag(role) && !principal.IsInRole(role))
                                    {
                                        identity.AddClaim(new Claim(LogMonitorClaimTypes.UserRole, role.ToRoleName()));
                                    }
                                    else if (!user.UserRole.HasFlag(role))
                                    {
                                        var roleString = role.ToRoleName();

                                        foreach (var claim in principal.FindAll(LogMonitorClaimTypes.UserRole).Where(c => string.Equals(c.Value, roleString, StringComparison.InvariantCultureIgnoreCase)).ToArray())
                                        {
                                            claim.Subject?.RemoveClaim(claim);
                                        }
                                    }
                                }

                                SetClaim(principal, identity, ClaimTypes.Email, await _userService.GetUserMailAsync(userId));
                                SetClaim(principal, identity, LogMonitorClaimTypes.UserDisplayName, user.DisplayName);
                                SetClaim(principal, identity, LogMonitorClaimTypes.UserDisplayShortName, user.DisplayShortName);

                                if (principal.Identity is not ClaimsIdentity)
                                {
                                    principal.AddIdentity(identity);
                                }

                                if (!user.IsAdministrator && !await _userService.IsAdminsExistsAsync())
                                {
                                    identity.AddClaim(new Claim(LogMonitorClaimTypes.UserRole, UserRoleEnum.Administrator.ToRoleName()));
                                }
                                else
                                {
                                    SetClaim(principal, identity, LogMonitorClaimTypes.IsProcessed, $"{true}");
                                }

                                return principal;
                            }
                        }
                    }

                    foreach (var claim in principal.FindAll(LogMonitorClaimTypes.UserRole).ToArray())
                    {
                        claim.Subject?.RemoveClaim(claim);
                    }

                    return principal;

                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                    throw;
                }
            }
        }

        private static void SetClaim(ClaimsPrincipal principal, ClaimsIdentity identity, string claimType, string claimValue)
        {
            var claim = principal.FindFirst(claimType);

            if (claim is null && !string.IsNullOrWhiteSpace(claimValue))
            {
                identity.AddClaim(new Claim(claimType, claimValue));
            }
            else if (claim is not null && !string.Equals(claim.Value, claimValue, StringComparison.InvariantCultureIgnoreCase))
            {
                claim.Subject?.RemoveClaim(claim);

                if (!string.IsNullOrWhiteSpace(claimValue))
                {
                    identity.AddClaim(new Claim(claimType, claimValue));
                }
            }
        }
    }
}

