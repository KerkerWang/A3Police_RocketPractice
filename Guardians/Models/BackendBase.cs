using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Guardians.Data;

namespace Guardians.Models
{
    public class BackendBase
    {
        [MaxLength(20)]
        [Display(Name = "發布者")]
        [JsonIgnore]
        public string? Poster { get; set; }

        [Display(Name = "發布時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? InitDate { get; set; }

        [MaxLength(20)]
        [Display(Name = "更新者")]
        [JsonIgnore]
        public string? Updater { get; set; }


        [Display(Name = "最後更新時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }





        public async Task Create(DBContext db, ClaimsPrincipal user)
        {

            this.InitDate = DateTime.Now;
            this.Poster = user.Identity.Name;
            db.Add(this);
            await db.SaveChangesAsync();

            StringBuilder sb = new StringBuilder();
            var entry = db.Entry(this);
            int id = Convert.ToInt32(entry.CurrentValues["Id"]);

            foreach (var propName in entry.CurrentValues.Properties)
            {
                var current = entry.CurrentValues[propName];
                var item = $";{propName.Name}={current};";
                sb.Append(item);
            }

            SystemLog log = new SystemLog();
            log.Subject = sb.ToString();
            log.UpdateMessage = "新增";
            log.Index = id;
            log.Poster = user.Identity.Name;
            log.DataTable = this.GetType().Name.Replace("Proxy", "");
            log.InitDate = DateTime.Now;
            db.Add(log);
            await db.SaveChangesAsync();
        }
        public async Task Update(DBContext db, ClaimsPrincipal user)
        {

            this.UpdateDate = DateTime.Now;
            this.Updater = user.Identity.Name;
            StringBuilder sb = new StringBuilder();
            var entry = db.Entry(this);
            int id = Convert.ToInt32(entry.CurrentValues["Id"]);
            string tableName = this.GetType().Name.Replace("Proxy", "");
            var oldData = db.SystemLogs.Where(x => x.DataTable == tableName && x.Index == id)
                .OrderByDescending(x => x.Id).FirstOrDefault();
            if (oldData == null)
            {
                db.Entry(this).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return;
            }
            var oldSubject = oldData.Subject;
            StringBuilder updateMessage = new StringBuilder("修改：");

            db.Entry(this).State = EntityState.Modified;


            if (oldSubject != null)
            {
                foreach (var propName in entry.CurrentValues.Properties)
                {
                    var current = entry.CurrentValues[propName];
                    var item = $";{propName.Name}={current};";
                    sb.Append(item);
                    if (oldSubject.IndexOf(item) == -1 && propName.Name != "Updater" && propName.Name != "UpdateDate")
                    {
                        updateMessage.Append(item.TrimStart(';'));
                    }
                }

            }
            else
            {
                updateMessage.Append("無log資料");
            }


            SystemLog log = new SystemLog();
            log.Subject = sb.ToString();
            log.UpdateMessage = updateMessage.ToString();
            log.Index = id;
            log.Poster = user.Identity.Name;
            log.DataTable = tableName;
            log.InitDate = DateTime.Now;
            db.Add(log);
            await db.SaveChangesAsync();


        }
        public async Task Delete(DBContext db, ClaimsPrincipal user)
        {
            var entry = db.Entry(this);
            int id = Convert.ToInt32(entry.CurrentValues["Id"]);
            var subject = "";
            foreach (var propName in entry.CurrentValues.Properties)
            {
                var current = entry.CurrentValues[propName];
                if (current != null)
                {
                    if (propName.Name.IndexOf("UpImageUrl") > -1)
                    {
                        subject = current.ToString();
                    }
                    if (propName.Name.IndexOf("Subject") > -1)
                    {
                        subject = current.ToString();
                    }
                    if (propName.Name.IndexOf("Name") > -1)
                    {
                        subject = current.ToString();
                    }
                }
            }
            SystemLog log = new SystemLog();
            log.Index = id;
            log.UpdateMessage = "刪除";
            log.Subject = "刪除";
            log.InitDate = DateTime.Now;
            log.Poster = user.Identity.Name;
            log.DataTable = this.GetType().Name.Replace("Proxy", "");
            db.SystemLogs.Add(log);


            db.Remove(this);
            await db.SaveChangesAsync();


        }

    }
}
