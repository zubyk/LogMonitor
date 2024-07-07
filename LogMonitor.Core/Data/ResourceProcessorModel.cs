using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMonitor.Models
{
    [Table("Processors", Schema = "dbo")]
    public class ResourceProcessorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public ResourceProcessorTypeEnum ProcessorType { get; set; }

        public string? Configuration { get; set; }

        [ForeignKey(nameof(PrevProcessor))]
        public int? PrevId { get; set; }

        public ResourceProcessorModel? PrevProcessor { get; set; }

        public virtual List<ResourceProcessorModel> NextProcessors { get; set; }

        public ResourceProcessorModel()
        {
            NextProcessors = new List<ResourceProcessorModel>();
        }
    }
}
