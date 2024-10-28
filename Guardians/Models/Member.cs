using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Guardians.Models
{
    public class Member : BackendBase
    {

        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty]
        public int Id { get; set; }

        

        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(50)]
        [Display(Name = "帳號")]
        [JsonProperty]
        public string Account { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        [Newtonsoft.Json.JsonIgnore]
        public string Password { get; set; }

        [MaxLength(100)]
        [Display(Name = "密碼鹽")]
        [Newtonsoft.Json.JsonIgnore]
        public string? PasswordSalt { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(50)]
        [Display(Name = "使用者名稱")]
        [JsonProperty]
        public string Name { get; set; }

        [MaxLength(50)]
        [Display(Name = "聯絡人")]
        [Newtonsoft.Json.JsonIgnore]
        public string? CName { get; set; }

        [MaxLength(50)]
        [Display(Name = "公司電話")]
        [Newtonsoft.Json.JsonIgnore]
        public string? TEL { get; set; }

        [MaxLength(50)]
        [Display(Name = "手機")]
        [Newtonsoft.Json.JsonIgnore]
        public string? Mobile { get; set; }

        [MaxLength(500)]
        [Display(Name = "網址")]
        [Newtonsoft.Json.JsonIgnore]
        public string? URL { get; set; }


        [Display(Name = "備註")]
        [Newtonsoft.Json.JsonIgnore]
        public string? Memo { get; set; }


        [Display(Name = "性別")]
        [Newtonsoft.Json.JsonIgnore]
        public GenderType? Gender { get; set; }

        //[Required(ErrorMessage = "{0}必填")]
        [EmailAddress(ErrorMessage = "{0} 格式錯誤")]
        [MaxLength(200)]
        //[DataType(DataType.EmailAddress)]
        [Display(Name = "E-Mail")]
        [JsonProperty]
        public string? Email { get; set; }

        [MaxLength(50)] 
        [Display(Name = "照片")]
        [Newtonsoft.Json.JsonIgnore]
        public string? MyPic { get; set; }

        [MaxLength(50)]
        [Display(Name = "職稱")]
        [Newtonsoft.Json.JsonIgnore]
        public string? JobTitle { get; set; }

        //ForeignKey
        [Required(ErrorMessage = "{0}必填")] public int? UnitId { get; set; }

        [Display(Name = "所屬單位")]
        [ForeignKey("UnitId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Unit? MyUnit { get; set; }

        [Display(Name = "角色")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Role>? Roles { get; set; }

        [MaxLength(500)]
        [Display(Name = "權限")]
        [JsonProperty]
        public string? Permission { get; set; }
    }
}