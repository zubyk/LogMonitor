using System.ComponentModel.DataAnnotations;

namespace LogMonitor.Models
{
    public enum ResourceProcessorTypeEnum : int
    {
        [Display(Name = "Обработчик событий")]
        CoreEventProcessor = 0,
        [Display(Name = "Загрузчик файлов по протоколу Samba")]
        SambaFileDownload = 1,
        [Display(Name = "Regexp-обработчик текстовых логов")]
        RegexpEventExtractor = SambaFileDownload * 2,

    }
}
