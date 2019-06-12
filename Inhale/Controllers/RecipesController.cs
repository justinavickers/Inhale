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
using Inhale.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Inhale.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecipesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        [Authorize]
        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Recipes.Include(r => r.RecipeType).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            NewRecipeViewModel viewModel = new NewRecipeViewModel(_context.Ingredients.ToList(), _context.RecipeType.ToList());

            //ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "Name");
            //ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View(viewModel);
        }



        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewRecipeViewModel viewModel)
        {
            var user = await GetCurrentUserAsync();
            viewModel.Recipe.UserId = user.Id;

            if (ModelState.IsValid)
            {
                viewModel.Recipe.RecipeTypeId = viewModel.SelectedRecipeType;

                _context.Add(viewModel.Recipe);
                await _context.SaveChangesAsync();

                viewModel.SelectedIngredients.ForEach(i =>
                {
                    var ri = new RecipeIngredients
                    {
                        IngredientId = i,
                        Recipe = viewModel.Recipe,
                        Amount = "0"
                    };
                    _context.Add(ri);
                    _context.SaveChanges();

                });


                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "RecipeTypeId", viewModel.Recipe.RecipeTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", viewModel.Recipe.UserId);
            return View(viewModel.Recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            EditRecipeViewModel viewModel = new EditRecipeViewModel(_context.Ingredients.ToList(), _context.RecipeType.ToList());

            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            viewModel.Recipe = recipe;

            ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "RecipeTypeId", recipe.RecipeTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", recipe.UserId);
            return View(viewModel);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeId,Name,RecipeNotes,RecipeTypeId,UserId")] Recipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeId))
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
            ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "RecipeTypeId", recipe.RecipeTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", recipe.UserId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
