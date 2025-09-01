using Microsoft.AspNetCore.Mvc;

namespace CourseDx.Controllers
{
    public class About : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
