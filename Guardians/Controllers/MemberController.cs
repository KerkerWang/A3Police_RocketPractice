using Guardians.Data;
using Guardians.Filters;
using Guardians.Models;
using Guardians.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Guardians.Controllers
{
    [Authorize]
    [PermissionFilter]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IUnitService _unitService;
        protected readonly DBContext _db;

        private const int DefaultPageSize = 1;

        public MemberController(DBContext db, IMemberService memberService, IUnitService unitService)
        {
            _db = db;

            _memberService = memberService;
            _unitService = unitService;
        }
        public IActionResult Index(int? page)
        {
            page = page.HasValue ? page.Value : 1;
            return View(_db.Members.OrderBy(p => p.InitDate).ToPagedList(page.Value, DefaultPageSize));
        }

        public IActionResult Create()
        {
            
            ViewBag.Units = new SelectList(_unitService.GetAll(), "Id", "Subject");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member)
        {
            
            ViewBag.Units = new SelectList(_unitService.GetAll(), "Id", "Subject");
            if (!ModelState.IsValid)
            {
                return View();
            }
            //TODO:檢查帳號
            var checkAccount = await _memberService.CheckAccountAsync(member.Account);
            if (!checkAccount)
            {
                ModelState.AddModelError("Account", "帳號已被使用");
                return View();
            }
            //上傳圖檔
            //if (uploadImage != null)
            //{
            //    if (!Utility.CheckImageType(uploadImage))
            //    {
            //        ModelState.AddModelError("MyPic", "檔案格式錯誤");
            //        return View();
            //    }
            //    member.MyPic = Utility.SaveImage(uploadImage);
            //}
            await _memberService.CreateAsync(member);
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> CheckAccountAsync(string account)
        {
            if (!await _memberService.CheckAccountAsync(account))
            {
                return Json("帳號已被使用");
            }
            return Json("帳號可使用");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Units = new SelectList(_unitService.GetAll(), "Id", "Subject");
            
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
            {
                return RedirectToAction("Index");
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Member member, string? newPassword)
        {
            
            ViewBag.Units = new SelectList(_unitService.GetAll(), "Id", "Subject");

            if (!ModelState.IsValid)
            {
                return View(member);
            }
            await _memberService.UpdateAsync(member, newPassword);
            return RedirectToAction("Index");

            ////移除驗證
            //ModelState.Remove("Account");
            //ModelState.Remove("Password");
            //member.Password = Request["NewPassword"] != "" ? Utility.GenerateHashWithSalt(Request["NewPassword"], member.PasswordSalt) : Request["hash"];
            //member.Permission = member.Permission ?? "";
            //if (ModelState.IsValid)
            //{
            //    member.Update();
            //    if (actionName == null)
            //    {
            //        actionName = "index";
            //    }
            //    return RedirectToActionPermanent(actionName, null,
            //        new { page = Request["page"] });
            //}
            //ViewBag.Units = db.Units.ToList();
            //string strMenu = Utility.GetMenu(member.Permission);
            //ViewBag.TreeScript = strMenu.Trim();
            //return View(member);
        }

        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
            {
                return RedirectToAction("Index");
            }

            return View(member);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

    }

}
