using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMonitor.Models
{
    [Table("Schedules", Schema = "dbo")]
    public class ScheduleModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Description { get; set; }

        [Required]
        public ScheduleTypeEnum ScheduleType { get; set; }

        [Required]
        [Column("Period")]
        [Range(0, 7*24*60*60)]
        public int PeriodSeconds { get; set; }

        public TimeSpan Period => TimeSpan.FromSeconds(PeriodSeconds);

        [Required]
        [ForeignKey(nameof(Resource))]
        public int ResourceId { get; set; }

        public MonitoredResourceModel Resource { get; set; }

        public ScheduleModel()
        {
        }
    }
}
