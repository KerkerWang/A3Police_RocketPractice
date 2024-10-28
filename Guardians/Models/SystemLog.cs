using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Guardians.Models
{
    public class SystemLog
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Id")]
        public int? Index { get; set; }

        [Required]
        [Display(Name = "欄位資料")]
        public string Subject { get; set; }

        [MaxLength(50)]
        [Display(Name = "發布者")]
        public string? Poster { get; set; }

        [Display(Name = "發布時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? InitDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "資料表")]
        public string? DataTable { get; set; }


        [Display(Name = "行為")]
        public string? UpdateMessage { get; set; }
    }
}
