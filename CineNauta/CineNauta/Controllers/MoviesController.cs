using Microsoft.AspNetCore.Mvc;

namespace Cine_Nauta.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
