using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Guardians.Models
{
    public class LoginLog
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "行為")]
        [MaxLength(500)]
        [Display(Name = "行為")]
        public LoginType Login { get; set; }

        [MaxLength(50)]
        [Display(Name = "發布者")]
        public string Poster { get; set; }

        [Display(Name = "發布時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? InitDate { get; set; }



    }
}
