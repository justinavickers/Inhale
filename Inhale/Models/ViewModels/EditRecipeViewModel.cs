using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models.ViewModels
{

    public class EditRecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<SelectListItem> Ingredients { get; set; }
        public List<int> SelectedIngredients { get; set; }
        public IEnumerable<SelectListItem> RecipeTypes { get; set; }
        public int SelectedRecipeType { get; set; }

        public List<Ingredient> IngredientsList { get; set; }
        public Dictionary<int, string> IngredientsWithAmount { get; set; }



        public EditRecipeViewModel(List<Ingredient> ingredients, List<RecipeType> recipes, List<RecipeIngredients> recipeIngredients)
        {
            IngredientsList = ingredients;

            Ingredients = ingredients.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.IngredientId.ToString(),
            }).ToList();

            RecipeTypes = recipes.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.RecipeTypeId.ToString(),
            }).ToList();

            Recipe = new Recipe();

            IngredientsWithAmount = new Dictionary<int, string>();

            foreach (var ingredient in ingredients)
            {
                var recipeIngredient = recipeIngredients.Find(x => x.IngredientId == ingredient.IngredientId);
                IngredientsWithAmount.Add(ingredient.IngredientId, recipeIngredient != null ? recipeIngredient.Amount : "");
            }
        }

        public EditRecipeViewModel()
        {

        }


    }
}