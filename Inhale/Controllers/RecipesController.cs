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
            var user = await GetCurrentUserAsync();


            List<Recipe> recipes = _context.Recipes.ToList();


            recipes.ForEach(
                r =>
                {
                    r.RecipeIngredients = _context.RecipeIngredients.Include(ing => ing.Ingredient)
                    .Where(ing => ing.RecipeId == r.RecipeId).ToList();
                    r.RecipeType = _context.RecipeType.Where(rT => r.RecipeTypeId == rT.RecipeTypeId).FirstOrDefault();
                    r.User = _context.ApplicationUsers.Where(u => r.UserId == u.Id).FirstOrDefault();

                });

            var filteredRecipes = recipes.Where(r => r.UserId != user.Id).ToList();

            return View(filteredRecipes);

        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await GetCurrentUserAsync();

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
            recipe.RecipeIngredients = _context.RecipeIngredients.Include(ing => ing.Ingredient).Where(ing => ing.RecipeId == recipe.RecipeId).ToList();

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {

            NewRecipeViewModel viewModel = new NewRecipeViewModel(_context.Ingredients.ToList(), _context.RecipeType.ToList());

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

                foreach (var ingredientId in viewModel.IngredientsWithAmount.Keys)
                {
                    var amount = viewModel.IngredientsWithAmount[ingredientId];

                    if (!String.IsNullOrEmpty(amount))
                    {
                        var ri = new RecipeIngredients
                        {
                            IngredientId = ingredientId,
                            RecipeId = viewModel.Recipe.RecipeId,
                            Amount = amount
                        };
                        _context.Add(ri);
                        _context.SaveChanges();
                    }
                }

                /*viewModel.SelectedIngredients.ForEach(i =>
                {
                    var ri = new RecipeIngredients
                    {
                        IngredientId = i,
                        Recipe = viewModel.Recipe,
                        Amount = "0"
                    };
                    _context.Add(ri);
                    _context.SaveChanges();

                });*/


                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "RecipeTypeId", viewModel.Recipe.RecipeTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", viewModel.Recipe.UserId);
            return View(viewModel.Recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*var user = await GetCurrentUserAsync();*/

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            recipe.RecipeIngredients = _context.RecipeIngredients.Include(ing => ing.Ingredient).Where(ing => ing.RecipeId == recipe.RecipeId).ToList();

            EditRecipeViewModel viewModel = new EditRecipeViewModel(
                _context.Ingredients.ToList(),
                _context.RecipeType.ToList(),
                recipe.RecipeIngredients.ToList());


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
        public async Task<IActionResult> Edit(int id, EditRecipeViewModel viewModel)
        {

            if (id != viewModel.Recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                viewModel.Recipe.RecipeTypeId = viewModel.SelectedRecipeType;
                var ingredientsInRecipe = _context.RecipeIngredients.Where(ri => ri.RecipeId == id).ToList();

                try
                {
                    foreach (var ingredientId in viewModel.IngredientsWithAmount.Keys)
                    {
                        var amount = viewModel.IngredientsWithAmount[ingredientId];
                        var foundRecipe = ingredientsInRecipe.SingleOrDefault(ir => ir.IngredientId == ingredientId);

                        if (foundRecipe == null && !String.IsNullOrEmpty(amount))
                        {
                            var ri = new RecipeIngredients
                            {
                                IngredientId = ingredientId,
                                RecipeId = viewModel.Recipe.RecipeId,
                                Amount = amount
                            };
                            _context.Add(ri);
                            _context.SaveChanges();
                        }
                        else if (foundRecipe != null && !String.IsNullOrEmpty(amount))
                        {
                            foundRecipe.Amount = amount;

                            _context.Update(foundRecipe);
                            _context.SaveChanges();
                        }
                        else if (foundRecipe != null && String.IsNullOrEmpty(amount))
                        {
                            _context.Remove(foundRecipe);
                            _context.SaveChanges();
                        }
                    }

                    /*viewModel.SelectedIngredients.ForEach(i =>
                    {
                        var foundRecipe = ingredientsInRecipe.SingleOrDefault(ir => ir.IngredientId == i);
                        if(foundRecipe == null)
                        {

                        var ri = new RecipeIngredients
                        {
                            IngredientId = i,
                            Recipe = viewModel.Recipe,
                            Amount = "0"
                        };
                        _context.Update(ri);
                        _context.SaveChanges();
                        }
                    });*/


                    //ingredientsInRecipe.ForEach(I =>
                    //{
                    //    var foundIngredient = viewModel.SelectedIngredients.SingleOrDefault(i => i == I.IngredientId);
                    //    if (foundIngredient == 0)
                    //    {
                    //        var recipeI = new RecipeIngredients
                    //        { 
                    //            IngredientId = I.IngredientId,
                    //            RecipeIngredientId = I.RecipeIngredientId,
                    //            Recipe = I.Recipe,
                    //            Amount = "0"
                    //        };
                    //        _context.Remove(recipeI);
                    //        _context.SaveChanges();
                    //    }
                    //});

                    _context.Update(viewModel.Recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(viewModel.Recipe.RecipeId))
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
            ViewData["RecipeTypeId"] = new SelectList(_context.RecipeType, "RecipeTypeId", "RecipeTypeId", viewModel.Recipe.RecipeTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", viewModel.Recipe.User.Id);
            return View(viewModel);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await GetCurrentUserAsync();

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

        // GET: Products/MyRecipes
        public async Task<IActionResult> MyRecipes()
        {
            var user = await GetCurrentUserAsync();
            //get only the user's (who is logged in) products 
            List<Recipe> recipes = _context.Recipes.ToList();


            recipes.ForEach(
                r =>
                {
                    r.RecipeIngredients = _context.RecipeIngredients.Include(ing => ing.Ingredient)
                    .Where(ing => ing.RecipeId == r.RecipeId).ToList();
                    r.RecipeType = _context.RecipeType.Where(rT => r.RecipeTypeId == rT.RecipeTypeId).FirstOrDefault();
                    r.User = _context.ApplicationUsers.Where(u => r.UserId == u.Id).FirstOrDefault();

                });

            var filteredRecipes = recipes.Where(r => r.UserId == user.Id).ToList();


            return View(filteredRecipes);


        }

        //POST: Recipe/ingredient
        //public async Task<IActionResult> addIngredient(NewRecipeViewModel viewModel)
        //{
        //    foreach (var selectedIngredientIndex in viewModel.SelectedIngredients) {
        //        var index = Convert.ToInt32(selectedIngredientIndex) - 1;
        //        var selectedIngredient = viewModel.IngredientsList[index];
        //        viewModel.IngredientsToEdit.Add(selectedIngredient.IngredientId, "");
        //    }
        //    return null;

        //}
    }
}
