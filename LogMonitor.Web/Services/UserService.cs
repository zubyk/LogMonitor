using LogMonitor.Data;
using LogMonitor.Models;
using LogMonitor.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;

namespace LogMonitor.Services
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UserService> _logger;
        private readonly PrincipalContext _adContext;

        public UserService(ILogger<UserService> logger, DataContext context, PrincipalContext adContext)
        {
            _dataContext = context;
            _logger = logger;
            _adContext = adContext;
        }

        /// <summary>
        /// Обходной метод для поиска пользователя используя StringComparison.InvariantCultureIgnoreCase не зависящий от реализации поиска в БД
        /// </summary>
        /// <param name="name">Логин пользователя</param>
        /// <param name="users">Список UserModel из БД</param>
        /// <returns></returns>
        private async Task<UserViewModel> GetUserAsync(string name, IEnumerable<UserModel> users)
        {
            UserRoleEnum? roles = null;

            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall(args: new { name, users });

                try
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        roles = users.FirstOrDefault(u => string.Equals(u.UserName, name, StringComparison.InvariantCultureIgnoreCase))?.UserRole;

                        return await GetADPrincipal(name, roles);
                    }
                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                }
            }

            return new UserViewModel(name, roles ?? UserRoleEnum.User, roles.HasValue, null, false);

            Task<UserViewModel> GetADPrincipal(string name, UserRoleEnum? roles)
            {
                return Task.Factory.StartNew(() =>
                {
                    using var user = UserPrincipal.FindByIdentity(_adContext, name);
                    return new UserViewModel(name, roles ?? UserRoleEnum.User, roles.HasValue, user?.DisplayName, user != null);
                });
            }
        }

        public async Task<string?> GetUserMailAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall(args: name);

                try
                {
                    return await GetADPrincipalEmail(name);
                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                    return null;
                }
            }

            Task<string?> GetADPrincipalEmail(string name)
            {
                return Task.Factory.StartNew(() =>
                {
                    using var user = UserPrincipal.FindByIdentity(_adContext, name);
                    return user?.EmailAddress;
                });
            }
        }

        public async Task<UserViewModel> GetUserAsync(string name)
        {
            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall(args: name);

                try
                {
                    var users = await _dataContext.Users.ToArrayAsync();

                    return await GetUserAsync(name, users);
                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
        {
            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall();

                var users = await _dataContext.Users.ToArrayAsync();

                var result = new List<UserViewModel>();

                foreach (var user in users)
                {
                    result.Add(await GetUserAsync(user.UserName, users));
                }

                return result;
            }
        }

        public async Task<bool> IsAdminsExistsAsync()
        {
            using (_logger.BeginScopeFromCaller())
            {
                _logger.LogMethodCall();

                try
                {
                    return await _dataContext.Users.AnyAsync(u => u.UserRole.HasFlag(UserRoleEnum.Administrator));
                }
                catch (Exception e) when (_logger.FilterAndLogError(e))
                {
                    return true;
                }
            }
        }
    }
}
