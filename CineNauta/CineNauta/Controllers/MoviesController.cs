﻿using Cine_Nauta.DAL;
using Cine_Nauta.DAL.Entities;
using Cine_Nauta.Helpers;
using Cine_Nauta.Models;
using Cine_Nauta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Drawing;

namespace Cine_Nauta.Controllers
{
    public class MoviesController : Controller
    {

        private readonly DataBaseContext _context;
        private readonly IDropDownListHelper _dropDownListHelper;
        private readonly IUserHelper _userHelper;

        public MoviesController(DataBaseContext context, IDropDownListHelper dropDownListHelper, IUserHelper userHelper)
        {
            _context = context;
            _dropDownListHelper = dropDownListHelper;
            _userHelper = userHelper;
        }

        private string GetUserFullName()
        {
            return _context.Users

                .Where(u => u.Email == User.Identity.Name)
                .Select(u => u.FullName)
                .FirstOrDefault();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserFullName = GetUserFullName();
            return View(await _context.Movies
                .Include(m => m.Gender)
                .Include(m => m.Classification)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? Id)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (Id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Gender)
                .Include(m => m.Classification)
                .Include(m => m.Functions)
                .ThenInclude(f => f.Room)  // Cargar la relación Room
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);

        }


        public async Task<IActionResult> DetailsMovie(int? Id)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (Id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Gender)
                .Include(m => m.Classification)
                .Include(m => m.Functions)
                .ThenInclude(f => f.Room)  // Cargar la relación Room
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);

        }


        public async Task<IActionResult> Movies()
        {
            ViewBag.UserFullName = GetUserFullName();
            return View(await _context.Movies
                .Include(m => m.Functions)
                //.Include(m => m.Hours)// Incluye los horarios relacionados
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.UserFullName = GetUserFullName();
            AddMovieViewModel addMovieViewModel = new()
            {
                Genders = await _dropDownListHelper.GetDDLGendersAsync(),
                Classifications = await _dropDownListHelper.GetDDLClassificationsAsync(),   
            };

            return View(addMovieViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddMovieViewModel addMovieViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {


                    Movie movie = new()
                    {
                        Title = addMovieViewModel.Title,
                        Description = addMovieViewModel.Description,
                        Director = addMovieViewModel.Director,
                        Duration = addMovieViewModel.Duration,
                        LaunchYear = addMovieViewModel.LaunchYear,
                        CreatedDate = DateTime.Now,
                        Gender = await _context.Genders.FindAsync(addMovieViewModel.GenderId),
                        Classification = await _context.Classifications.FindAsync(addMovieViewModel.ClassificationId),


                    };


                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una pelicula con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }


            addMovieViewModel.Genders = await _dropDownListHelper.GetDDLGendersAsync();
            addMovieViewModel.Classifications = await _dropDownListHelper.GetDDLClassificationsAsync();
            return View(addMovieViewModel);
        }


        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (Id == null) return NotFound();

            Movie movie = await _context.Movies.FindAsync(Id);
            if (movie == null) return NotFound();

            EditMovieViewModel editMovietViewModel = new()
            {
                Id = movie.Id,
                Title = movie.Title,
                LaunchYear = movie.LaunchYear,
                Description = movie.Description,
                Director = movie.Director,  
                Duration = movie.Duration,
                GenderId= movie.GenderId,
                ClassificationId= movie.ClassificationId,
                Genders = await _dropDownListHelper.GetDDLGendersAsync(),
                Classifications = await _dropDownListHelper.GetDDLClassificationsAsync(),
                
            };

            return View(editMovietViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? Id, EditMovieViewModel editMovietViewModel)
        {
            if (Id != editMovietViewModel.Id) return NotFound();

            try
            {
                Movie movie = await _context.Movies.FindAsync(editMovietViewModel.Id);

                //Aquí sobreescribo para luego guardar los cambios en BD
                movie.Title = editMovietViewModel.Title;
                movie.Description = editMovietViewModel.Description;
                movie.LaunchYear = editMovietViewModel.LaunchYear;
                movie.Director = editMovietViewModel.Director;
                movie.Duration = editMovietViewModel.Duration;
                movie.Gender = await _context.Genders.FindAsync(editMovietViewModel.GenderId);
                movie.Classification = await _context.Classifications.FindAsync(editMovietViewModel.ClassificationId);
                movie.ModifiedDate = DateTime.Now;

                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    ModelState.AddModelError(string.Empty, "Ya existe una pelicula con el mismo nombre.");
                else
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
            await FillDropDownListLocation(editMovietViewModel);
            return View(editMovietViewModel);
        }


        // DropDownListLocation es la lista desplegable de los generos y clasificaciones
        private async Task FillDropDownListLocation(EditMovieViewModel addMovieViewModel)
        {
            addMovieViewModel.Genders = await _dropDownListHelper.GetDDLGendersAsync();
            addMovieViewModel.Classifications = await _dropDownListHelper.GetDDLClassificationsAsync();
        }

        private bool Exists(int id)
        {
            return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }


       
        
        public async Task<IActionResult> Delete(int? Id)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (Id == null) return NotFound();

            Movie movie = await _context.Movies
                .Include(m => m.Gender)
                .Include(m => m.Classification)
                .Include(m => m.Functions)              
                .FirstOrDefaultAsync(p => p.Id == Id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            
            if (_context.Movies == null)
                return Problem("Entity set 'DataBaseContext.Movies' is null.");

            var movie = await _context.Movies.FindAsync(Id); //Select * From Movies Where Id = '1'
            if (movie != null) _context.Movies.Remove(movie);

            await _context.SaveChangesAsync(); //Delete From Movies where Id = '1'
            return RedirectToAction(nameof(Index));
        }


        #region Function

        public async Task<IActionResult> IndexFunction()
        {
            ViewBag.UserFullName = GetUserFullName();
            return View(await _context.Functions
                 .Include(c => c.Movie)
                 .Include(c => c.Room)
                 .ToListAsync());
        }


        public async Task<IActionResult> AddFunction(int movieId)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (movieId == null) return NotFound();

            Movie movie = await _context.Movies
                .FirstOrDefaultAsync(p => p.Id == movieId);

            if (movie == null) return NotFound();



            AddFunctionViewModel addFunctionViewModel = new()
            {
                MovieId = movie.Id,
                Rooms = await _dropDownListHelper.GetDDLRoomsAsync(),
            };

            return View(addFunctionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunction(AddFunctionViewModel addFunctionViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Function function = new()
                    {
                        Price = addFunctionViewModel.Price,
                        FunctionDate = addFunctionViewModel.FunctionDate,
                        CreatedDate = DateTime.Now,
                        Movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == addFunctionViewModel.MovieId),
                        Room = await _context.Rooms.FindAsync(addFunctionViewModel.RoomId),


                    };

                    _context.Add(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una función de la pelicula en la misma fecha y sala");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            addFunctionViewModel.Rooms = await _dropDownListHelper.GetDDLRoomsAsync();
            return View(addFunctionViewModel);
        }


        public async Task<IActionResult> EditFunction(int? functionId)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (functionId == null) return NotFound();

            Function function = await _context.Functions
                .Include(f => f.Movie)
                .FirstOrDefaultAsync( f => f.Id == functionId);

            if (function == null) return NotFound();

            EditFunctionViewModel editFunctionViewModel = new()
            {
                
                MovieId = function.Movie.Id,
                Id = function.Id,
                Price = function.Price,
                FunctionDate = function.FunctionDate,
                CreatedDate = DateTime.Now,              
                RoomId = function.RoomId,
                Rooms = await _dropDownListHelper.GetDDLRoomsAsync(),


            };

            return View(editFunctionViewModel);
        }

  


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFunction(int movieId, EditFunctionViewModel editFunctionViewModel)
        {
            if (movieId != editFunctionViewModel.MovieId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    Function function = new()
                    {
                        Id = editFunctionViewModel.Id,
                        Price = editFunctionViewModel.Price,
                        FunctionDate = editFunctionViewModel.FunctionDate,
                        ModifiedDate = editFunctionViewModel.ModifiedDate,
                        Room = await _context.Rooms.FindAsync(editFunctionViewModel.RoomId),
                        
                    };
                 

                  
                    _context.Update(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = editFunctionViewModel.MovieId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                        ModelState.AddModelError(string.Empty, "Ya existe una función de la pelicula en la misma fecha y sala");
                    else
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            
            await FillDropDownListLocation(editFunctionViewModel);
            return View(editFunctionViewModel);
        }

        // DropDownListLocation es la lista desplegable de las peliculas y salas
        private async Task FillDropDownListLocation(EditFunctionViewModel editFunctionViewModel)
        {
            editFunctionViewModel.Movies = await _dropDownListHelper.GetDDLMoviesAsync();
            editFunctionViewModel.Rooms = await _dropDownListHelper.GetDDLRoomsAsync();
        }
        

        public async Task<IActionResult> DetailsFunction(int? functionId)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (functionId == null) return NotFound();

            // Cargar la película y los datos relacionados (Género y Clasificación)     
            Function function = await _context.Functions
                .Include(m => m.Movie)
                .Include(m => m.Room)
                .FirstOrDefaultAsync(p => p.Id == functionId);
            //Movie movie = await _context.Movies.FirstOrDefaultAsync(p => p.Id == Id);

            if (function == null) return NotFound();

            return View(function);
        }


        public async Task<IActionResult> DeleteFunction(int? functionId)
        {
            ViewBag.UserFullName = GetUserFullName();
            if (functionId == null) return NotFound();

            Function function = await _context.Functions
                .Include(m => m.Movie)
                .Include(m => m.Room)
                .FirstOrDefaultAsync(p => p.Id == functionId);
            if (function == null) return NotFound();

            return View(function);
        }

        [HttpPost, ActionName("DeleteFunction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFunctionConfirmed(int? functionId)
        {
            if (_context.Functions == null) return Problem("Entity set 'DataBaseContext.Functions' is null.");

            Function function = await _context.Functions
                .Include(m => m.Movie)
               .FirstOrDefaultAsync(p => p.Id == functionId);

            if (function != null) _context.Functions.Remove(function);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = function.Movie.Id });
        }
        #endregion

        #region Reservation

        public async Task<IActionResult> AddFunctionInCart(int? functionId)
        {
            if (functionId == null) return NotFound();

            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            Function function = await _context.Functions.FindAsync(functionId);
            User user = await _userHelper.GetUserAsync(User.Identity.Name);

            if (user == null || function == null) return NotFound();

            // Busca una entrada existente en la tabla TemporalSale para este producto y usuario
            TemporalSale existingTemporalSale = await _context.TemporalSales
                .Where(t => t.Function.Id == functionId && t.User.Id == user.Id)
                .FirstOrDefaultAsync();

            if (existingTemporalSale != null)
            {
                // Si existe una entrada, incrementa la cantidad
                existingTemporalSale.Quantity += 1;
                existingTemporalSale.ModifiedDate = DateTime.Now;
            }
            else
            {
                // Si no existe una entrada, crea una nueva
                TemporalSale temporalSale = new()
                {
                    CreatedDate = DateTime.Now,
                    Function = function,
                    Quantity = 1,
                    User = user
                };

                _context.TemporalSales.Add(temporalSale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize] 
        public async Task<IActionResult> ShowCartAndConfirm()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null) return NotFound();

            List<TemporalSale>? temporalSales = await _context.TemporalSales
                .Include(ts => ts.Function)
                .ThenInclude(p => p.Room)
                .ThenInclude(p => p.Movie)
                .Where(ts => ts.User.Id == user.Id)
                .ToListAsync();

            ShowCartViewModel showCartViewModel = new()
            {
                User = user,
                TemporalSales = temporalSales
            };

            return View(showCartViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowCartAndConfirm(ShowCartViewModel showCartViewModel)
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null) return NotFound();

            showCartViewModel.User = user;
            showCartViewModel.TemporalSales = await _context.TemporalSales
                .Include(ts => ts.Function)
                .Where(ts => ts.User.Id == user.Id)
            .ToListAsync();

           // Response response = await _orderHelper.ProcessOrderAsync(showCartViewModel);
           //if (response.IsSuccess) return RedirectToAction(nameof(OrderSuccess));

            //ModelState.AddModelError(string.Empty, response.Message);
            return View(showCartViewModel);
        }


        public async Task<IActionResult> DecreaseQuantity(int? temporalSaleId)
        {
            if (temporalSaleId == null) return NotFound();

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(temporalSaleId);
            if (temporalSale == null) return NotFound();

            if (temporalSale.Quantity > 1)
            {
                temporalSale.Quantity--;
                temporalSale.ModifiedDate = DateTime.Now;
                _context.TemporalSales.Update(temporalSale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ShowCartAndConfirm));
        }

        public async Task<IActionResult> IncreaseQuantity(int? temporalSaleId)
        {
            if (temporalSaleId == null) return NotFound();

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(temporalSaleId);
            if (temporalSale == null) return NotFound();

            temporalSale.Quantity++;
            temporalSale.ModifiedDate = DateTime.Now;
            _context.TemporalSales.Update(temporalSale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ShowCartAndConfirm));
        }

        public async Task<IActionResult> DeleteTemporalSale(int? temporalSaleId)
        {
            if (temporalSaleId == null) return NotFound();

            TemporalSale temporalSale = await _context.TemporalSales.FindAsync(temporalSaleId);
            if (temporalSale == null) return NotFound();

            _context.TemporalSales.Remove(temporalSale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowCartAndConfirm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null) return NotFound();

            List<TemporalSale> temporalSale = await _context.TemporalSales
                .Where(ts => ts.User.Id == user.Id)
                .ToListAsync();

            _context.RemoveRange(temporalSale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion




    }
}