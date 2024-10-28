using Guardians.Filters;
using Guardians.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;
using Guardians.Data;

namespace Guardians.Controllers
{
    [Authorize]
    [PermissionFilter]
    public class RoleController : Controller
    {
        private readonly DBContext _context;

        public RoleController(DBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
        }

        // GET: RoleController
        public ActionResult Index()
        {
            return View(_context.Roles.ToList());
        }



        // GET: RoleController/Create
        public ActionResult Create()
        {

            ViewBag.Members = _context.Members.ToList();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load($@"wwwroot\Config\Menu.xml");
            XmlNode rootNode = xmlDoc.DocumentElement;

            StringBuilder sb = new StringBuilder("[");
            sb.Append(Utility.GetPermissionScript(rootNode));
            sb.Append("]");
            ViewBag.PermissionTreeScript = sb;



            return View();
        }



        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Role role, string hiddenMemberListSelect)
        {
            ViewBag.Members = _context.Members.ToList();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load($@"wwwroot\Config\Menu.xml");
            XmlNode rootNode = xmlDoc.DocumentElement;

            StringBuilder sb = new StringBuilder("[");
            sb.Append(Utility.GetPermissionScript(rootNode));
            sb.Append("]");
            ViewBag.PermissionTreeScript = sb;

            if (!ModelState.IsValid)
            {
                return View(role);
            }

            if (!string.IsNullOrWhiteSpace(hiddenMemberListSelect))
            {
                string[] strArray = hiddenMemberListSelect.Split(',');
                var members = _context.Members.Where(c => strArray.Contains(c.Id.ToString())).ToList();
                role.Members = members;
            }

            await role.Create(_context, User);
            return RedirectToAction("Index");
        }


        // GET: RoleController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Role role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.unRoleMembers = _context.Members.ToList().Where(x => !(role.Members.Contains(x)));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"wwwroot\Config\Menu.xml");
            XmlNode rootnode = xmlDoc.DocumentElement;
            StringBuilder sb = new StringBuilder("[");
            sb.Append(Utility.GetPermissionScript(rootnode));
            sb.Append("]");
            ViewBag.PermissionTreeScript = sb;

            return View(role);
        }

        // POST: RoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Role role, string hiddenMemberListSelect)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit");
            }

            var oldRole = await _context.Roles.FindAsync(role.Id);
            _context.Entry(oldRole).CurrentValues.SetValues(role);
            foreach (var oldRoleMember in oldRole.Members)
            {
                oldRole.Members.Remove(oldRoleMember);
            }

            if (!string.IsNullOrWhiteSpace(hiddenMemberListSelect))
            {
                string[] strArray = hiddenMemberListSelect.Split(',');
                var selectedMemberList = _context.Members.Where(x => strArray.Contains(x.Id.ToString())).ToList();
                oldRole.Members = selectedMemberList;
            }

            await oldRole.Update(_context, User);
            return RedirectToAction("Index");
        }



        // POST: RoleController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Role role = await _context.Roles.FindAsync(id);
            await role.Delete(_context, User);
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }





    }
}
