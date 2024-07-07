using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMonitor.Models
{
    [Table("Users", Schema = "usr")]
    public class UserModel
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        public UserRoleEnum UserRole { get; set; }

        public UserModel()
        {
            UserRole = UserRoleEnum.User;
        }
    }
}
