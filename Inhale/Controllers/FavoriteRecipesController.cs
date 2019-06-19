using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inhale.Data;
using Inhale.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Inhale.Controllers
{
    public class FavoriteRecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteRecipesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        [Authorize]
        // GET: FavoriteRecipes
        public async Task<IActionResult> Index()
        {

            var favoriteRecipes = await _context.FavoriteRecipes
              .Include(f => f.Recipe).ToListAsync();



            favoriteRecipes.ForEach(f => {
                f.Recipe.RecipeIngredients = _context.RecipeIngredients.Include(ing => ing.Ingredient)
                .Where(ing => ing.RecipeId == f.Recipe.RecipeId).ToList();
                f.Recipe.RecipeType = _context.RecipeType.Where(rT => f.Recipe.RecipeTypeId == rT.RecipeTypeId).FirstOrDefault();
                f.Recipe.User = _context.ApplicationUsers.Where(u => f.Recipe.UserId == u.Id).FirstOrDefault();

            });
                
                

            
              //.FirstOrDefaultAsync(m => m.FavoriteRecipesId == id);
           // var applicationDbContext = _context.FavoriteRecipes.Include(f => f.Recipe).Include(f => f.User);

            return View(favoriteRecipes);

        }

        // GET: FavoriteRecipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteRecipes = await _context.FavoriteRecipes
                .Include(f => f.Recipe)
                .ThenInclude(f => f.RecipeIngredients)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FavoriteRecipesId == id);
            if (favoriteRecipes == null)
            {
                return NotFound();
            }

            return View(favoriteRecipes);
        }

        

        // POST: FavoriteRecipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var user = await GetCurrentUserAsync();

            if (_context.FavoriteRecipes.FirstOrDefault(fr => fr.RecipeId == id && fr.UserId == user.Id) !=null)
            {
                return RedirectToAction("Index", "Recipes");

            }

            FavoriteRecipes favoriteRecipes = new FavoriteRecipes
            {
                RecipeId = id,
                Recipe = _context.Recipes.FirstOrDefault( r => r.RecipeId == id),
                UserId = user.Id,
                User = user
            };
         

            if (ModelState.IsValid)
            {
                _context.Add(favoriteRecipes);
                _context.SaveChanges();
                return RedirectToAction("Index", "Recipes");
            }
            return View(favoriteRecipes);
        }

        // GET: FavoriteRecipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteRecipes = await _context.FavoriteRecipes.FindAsync(id);
            if (favoriteRecipes == null)
            {
                return NotFound();
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", favoriteRecipes.RecipeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", favoriteRecipes.UserId);
            return View(favoriteRecipes);
        }

        // POST: FavoriteRecipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FavoriteRecipesId,UserId,RecipeId")] FavoriteRecipes favoriteRecipes)
        {
            if (id != favoriteRecipes.FavoriteRecipesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favoriteRecipes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteRecipesExists(favoriteRecipes.FavoriteRecipesId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", favoriteRecipes.RecipeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", favoriteRecipes.UserId);
            return View(favoriteRecipes);
        }

        // GET: FavoriteRecipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteRecipes = await _context.FavoriteRecipes
                .Include(f => f.Recipe)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FavoriteRecipesId == id);
            if (favoriteRecipes == null)
            {
                return NotFound();
            }

            return View(favoriteRecipes);
        }

        // POST: FavoriteRecipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favoriteRecipes = await _context.FavoriteRecipes.FindAsync(id);
            _context.FavoriteRecipes.Remove(favoriteRecipes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteRecipesExists(int id)
        {
            return _context.FavoriteRecipes.Any(e => e.FavoriteRecipesId == id);
        }
    }
}
