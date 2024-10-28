using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Guardians.Models
{
    public class Role:BackendBase
    {
        public Role()
        {
            Members = new List<Member>();
        } 

        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "群組名稱必填")]
        [MaxLength(100)]
        [Display(Name = "群組名稱")]
        public string? Subject { get; set; }

        [MaxLength(6000)]
        [Display(Name = "權限")]
        public string? Permission { get; set; }


        [Display(Name = "隸屬成員")]
        [JsonProperty]
        public virtual  ICollection<Member>? Members { get; set; }


    }
}
