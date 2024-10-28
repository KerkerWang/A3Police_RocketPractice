using Guardians.Models;
using Guardians.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Guardians.Controllers
{
    public class Account : Controller
    {
        private readonly IMemberService _service;
        //recaptcha網址
        private string apiAddress = "https://www.google.com/recaptcha/api/siteverify";

        //後端金鑰
        private string recaptchaSecret = "6LfxPa4bAAAAAM2j37HkOoLC3gz8dCjlbktVLxrA";

        //Google reCaptcha的回應類別
        public class CaptchaResponseViewModel
        {
            public bool Success { get; set; }

            [JsonProperty(PropertyName = "error-codes")]
            public IEnumerable<string> ErrorCodes { get; set; }

            [JsonProperty(PropertyName = "challenge_ts")]
            public DateTime ChallengeTime { get; set; }

            public string HostName { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
        }

        /// <summary>
        /// 處理token
        /// </summary>
        /// <param name="recaptchaToken"></param>
        /// <returns></returns>
        public string RecaptchaVerify(string recaptchaToken)
        {
            string url = $"{apiAddress}?secret={recaptchaSecret}&response={recaptchaToken}";
            using (var httpClient = new HttpClient())
            {
                try
                {

                    string responseString = httpClient.GetStringAsync(url).Result;
                    return responseString;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public Account(IMemberService service)
        {
            _service = service;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? ret)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }


            var member = await _service.ValidateUserAsync(loginViewModel.Account, loginViewModel.Password);
            if (member == null)
            {
                ModelState.AddModelError("Password", "帳號或密碼錯誤");
                return View(loginViewModel);
            }
            Utility.GetMemberPermission(member);
            await _service.SetAuthenTicket(member);
            await _service.SignInLogAsync(member.Account);
            if (!string.IsNullOrWhiteSpace(ret))
            {
                return LocalRedirect(ret);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _service.SignOutLogAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
