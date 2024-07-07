using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMonitor.Models
{
    [Table("Groups", Schema = "dbo")]
    public class ResourceGroupModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }


        [ForeignKey(nameof(Parent))]
        public int? ParentId { get; set; }

        public ResourceGroupModel? Parent { get; set; }

        public int SortOrder { get; set; }

        public ResourceGroupModel()
        {
        }
    }
}
