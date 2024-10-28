using Guardians.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Security.Claims;
using Guardians.Data;
using Microsoft.EntityFrameworkCore;

namespace Guardians.Services
{
    public interface IMemberService
    {
        Task<Member> ValidateUserAsync(string account, string password);
        Task<bool> CheckAccountAsync(string account);
        Task<List<Member>> GetAllAsync();
        Task<Member> GetByIdAsync(int id);
        Task SetAuthenTicket(Member member);
        Task CreateAsync(Member member);
        Task UpdateAsync(Member member, string newPassword);
        Task DeleteAsync(int id);
        Task SignInLogAsync(string account);
        Task SignOutLogAsync();


    }

    public class MemberService : IMemberService
    {
        protected readonly DBContext _db;
        private IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal _user;

        public MemberService(DBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
        }

        #region "將使用者資料寫入cookie,產生AuthenTicket"
        /// <summary>
        /// 將使用者資料寫入cookie,產生AuthenTicket
        /// </summary>
        public async Task SetAuthenTicket(Member member)
        {
            var strMember = JsonConvert.SerializeObject(member);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, member.Id.ToString(),ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, member.Account),
                new Claim(ClaimTypes.GivenName, member.Name),
                new Claim(ClaimTypes.Authentication, member.Permission,ClaimValueTypes.String),
                new Claim(ClaimTypes.UserData, strMember),
               
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
        }
        #endregion

        public async Task SignInLogAsync(string account)
        {
            LoginLog log = new LoginLog();
            log.Login = LoginType.登入;
            log.Poster = account;
            log.InitDate = DateTime.Now;
            _db.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task SignOutLogAsync()
        {
            LoginLog log = new LoginLog();
            log.Login = LoginType.登出;
            log.Poster = _user.Identity.Name;
            log.InitDate = DateTime.Now;
            _db.Add(log);
            await _db.SaveChangesAsync();
        }
        /// <summary>
        /// 帳密驗證
        /// </summary>
        /// <param name="account">使用者輸入的帳號</param>
        /// <param name="password">使用者輸入的密碼</param>
        /// <returns></returns>
        public async Task<Member> ValidateUserAsync(string account, string password)
        {
            //檢查是否有該使用者
            Member member = await _db.Members
                .Include(x => x.Roles)
                .Include(x => x.MyUnit)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Account == account);
            if (member == null)
            {
                return null;
            }
            //將該使用者的密碼用他自己的密碼鹽加密
            string saltPassword = Utility.GenerateHashWithSalt(password, member.PasswordSalt);
            //如果加密後的密碼跟資料庫密碼相同，則回傳該使用者的這筆資料
            return saltPassword == member.Password ? member : null;
        }

        public async Task<bool> CheckAccountAsync(string account)
        {
            //檢查是否有該使用者
            Member member = await _db.Members.FirstOrDefaultAsync(o => o.Account == account);
            if (member == null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 使用者列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Member>> GetAllAsync()
        {
            return await _db.Members.ToListAsync();
        }
        /// <summary>
        /// 個別使用者資料
        /// </summary>
        /// <returns></returns>
        public async Task<Member> GetByIdAsync(int id)
        {
            return await _db.Members.FindAsync(id);
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task CreateAsync(Member member)
        {
            member.PasswordSalt = Utility.CreateSalt();
            member.Password = Utility.GenerateHashWithSalt(member.Password, member.PasswordSalt);
            await member.Create(_db, _user);

        }
        /// <summary>
        /// 編輯使用者
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Member member, string newPassword)
        {
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                member.Password = Utility.GenerateHashWithSalt(newPassword, member.PasswordSalt);
            }
            await member.Update(_db, _user);
            //_db.Update(member);
            //await _db.SaveChangesAsync();
            //_db.Entry(member).State = EntityState.Modified;
            //await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _db.Members.FindAsync(id);
            await member.Delete(_db, _user);
        }

    }
}
