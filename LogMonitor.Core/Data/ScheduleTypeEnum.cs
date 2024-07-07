using System.ComponentModel.DataAnnotations;

namespace LogMonitor.Models
{
    public enum ScheduleTypeEnum : int
    {
        [Display(Name = "Каждые N минут")]
        EveryNMinutes = 0,
        [Display(Name = "Каждый час")]
        EveryHour = 1,
        [Display(Name = "Каждый день")]
        EveryDay = EveryHour * 2,
        [Display(Name = "Каждую неделю")]
        EveryWeek = EveryDay * 2,
    }
}
