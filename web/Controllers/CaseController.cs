using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    public class CaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
