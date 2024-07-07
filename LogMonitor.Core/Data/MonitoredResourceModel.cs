using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMonitor.Models
{
    [Table("Resources", Schema = "dbo")]
    public class MonitoredResourceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [ForeignKey(nameof(ResourceGroup))]
        public int ResourceGroupId { get; set; }

        public ResourceGroupModel ResourceGroup { get; set; }

        public int SortOrder { get; set; }

        [Required]
        public MonitoredResourceTypeEnum ResourceType { get; set; }

        public bool IsEnabled { get; set; }

        public virtual List<ScheduleModel> Schedules { get; set; }

        public MonitoredResourceModel()
        {
            Schedules = new List<ScheduleModel>();
        }
    }
}
