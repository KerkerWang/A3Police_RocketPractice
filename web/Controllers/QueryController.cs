using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    public class QueryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
