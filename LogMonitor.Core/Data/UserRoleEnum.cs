using System.ComponentModel.DataAnnotations;

namespace LogMonitor.Models
{
    [Flags]
    public enum UserRoleEnum: int
    {
        [Display(Name = "Пользователь")]
        User = 0,
        [Display(Name = "Дежурный")]
        Operator = 1,
        [Display(Name = "Технолог")]
        Supervisor = Operator * 2,
        [Display(Name = "Администратор")]
        Administrator = Supervisor * 2,
    }
}
