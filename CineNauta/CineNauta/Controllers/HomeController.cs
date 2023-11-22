using Cine_Nauta.DAL;
using Cine_Nauta.DAL.Entities;
using Cine_Nauta.Helpers;
using Cine_Nauta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Cine_Nauta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _context;
        private readonly IUserHelper _userHelper;
        

        public HomeController(ILogger<HomeController> logger, DataBaseContext context, IUserHelper userHelper)
        {
            _logger = logger;
            _context = context;
            _userHelper = userHelper;
            
        }





        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "NameDesc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "PriceDesc" : "Price";
            ViewBag.UserFullName = GetUserFullName();
            ViewBag.CurrentFilter = searchString;


            IQueryable<Movie> query = _context.Movies

               .Include(p => p.Functions);
               


            //Begins New change
            HomeViewModel homeViewModel = new()
            {
                Movies = await query.ToListAsync(),
                
            };

            

            return View(homeViewModel);
            //Ends New change
        }

        private string GetUserFullName()
        {
            return _context.Users

                .Where(u => u.Email == User.Identity.Name)
                .Select(u => u.FullName)
                .FirstOrDefault();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}