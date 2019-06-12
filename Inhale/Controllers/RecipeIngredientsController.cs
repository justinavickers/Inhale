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

namespace Inhale.Controllers
{
    public class RecipeIngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipeIngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: RecipeIngredients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RecipeIngredients.Include(r => r.Ingredient).Include(r => r.Recipe);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RecipeIngredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeIngredients = await _context.RecipeIngredients
                .Include(r => r.Ingredient)
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);
            if (recipeIngredients == null)
            {
                return NotFound();
            }

            return View(recipeIngredients);
        }

        // GET: RecipeIngredients/Create
        public IActionResult Create()
        {
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "IngredientId", "IngredientId");
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId");
            return View();
        }

        // POST: RecipeIngredients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeIngredientId,IngredientId,Amount,RecipeId")] RecipeIngredients recipeIngredients)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipeIngredients);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "IngredientId", "IngredientId", recipeIngredients.IngredientId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeIngredients.RecipeId);
            return View(recipeIngredients);
        }

        // GET: RecipeIngredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeIngredients = await _context.RecipeIngredients.FindAsync(id);
            if (recipeIngredients == null)
            {
                return NotFound();
            }
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "IngredientId", "IngredientId", recipeIngredients.IngredientId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeIngredients.RecipeId);
            return View(recipeIngredients);
        }

        // POST: RecipeIngredients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeIngredientId,IngredientId,Amount,RecipeId")] RecipeIngredients recipeIngredients)
        {
            if (id != recipeIngredients.RecipeIngredientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipeIngredients);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeIngredientsExists(recipeIngredients.RecipeIngredientId))
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
            ViewData["IngredientId"] = new SelectList(_context.Ingredients, "IngredientId", "IngredientId", recipeIngredients.IngredientId);
            ViewData["RecipeId"] = new SelectList(_context.Recipes, "RecipeId", "RecipeId", recipeIngredients.RecipeId);
            return View(recipeIngredients);
        }

        // GET: RecipeIngredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeIngredients = await _context.RecipeIngredients
                .Include(r => r.Ingredient)
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);
            if (recipeIngredients == null)
            {
                return NotFound();
            }

            return View(recipeIngredients);
        }

        // POST: RecipeIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipeIngredients = await _context.RecipeIngredients.FindAsync(id);
            _context.RecipeIngredients.Remove(recipeIngredients);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeIngredientsExists(int id)
        {
            return _context.RecipeIngredients.Any(e => e.RecipeIngredientId == id);
        }
    }
}
