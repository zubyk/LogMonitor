using System.ComponentModel.DataAnnotations;

namespace LogMonitor.Models
{
    public enum MonitoredResourceTypeEnum: int
    {
        [Display(Name = "Текстовый файл")]
        TextFile = 0,
    }
}
