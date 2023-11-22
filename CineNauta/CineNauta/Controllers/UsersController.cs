using Cine_Nauta.DAL;
using Cine_Nauta.DAL.Entities;
using Cine_Nauta.Enum;
using Cine_Nauta.Helpers;
using Cine_Nauta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Nauta.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataBaseContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IDropDownListHelper _ddlHelper;
        

        public UsersController(DataBaseContext context, IUserHelper userHelper, IDropDownListHelper dropDownListHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _ddlHelper = dropDownListHelper;
            
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.City)
                .ThenInclude(c => c.State)
                .ThenInclude(s => s.Country)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            AddUserViewModel addUserViewModel = new()
            {
                Id = Guid.Empty,
                Countries = await _ddlHelper.GetDDLCountriesAsync(),
                States = await _ddlHelper.GetDDLStatesAsync(new Guid()),
                Cities = await _ddlHelper.GetDDLCitiesAsync(new Guid()),
                UserType = UserType.Admin,
            };

            return View(addUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(AddUserViewModel addUserViewModel)
        {
            if (ModelState.IsValid)
            {
                
                addUserViewModel.CreatedDate = DateTime.Now;

                User user = await _userHelper.AddUserAsync(addUserViewModel);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    await FillDropDownListLocation(addUserViewModel);
                    return View(addUserViewModel);
                }

                return RedirectToAction("Index", "Users");
            }

            await FillDropDownListLocation(addUserViewModel);
            return View(addUserViewModel);
        }

        private async Task FillDropDownListLocation(AddUserViewModel addUserViewModel)
        {
            addUserViewModel.Countries = await _ddlHelper.GetDDLCountriesAsync();
            addUserViewModel.States = await _ddlHelper.GetDDLStatesAsync(addUserViewModel.CountryId);
            addUserViewModel.Cities = await _ddlHelper.GetDDLCitiesAsync(addUserViewModel.StateId);
        }

        [HttpGet]
        public JsonResult GetStates(Guid countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);

            if (country == null) return null;

            return Json(country.States.OrderBy(d => d.Name));
        }

        [HttpGet]
        public JsonResult GetCities(Guid stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null) return null;

            return Json(state.Cities.OrderBy(c => c.Name));
        }

    }
}
