using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class DetailRecipeViewModel
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string RecipeNotes { get; set; }
        public RecipeType RecipeType { get; set; }
        public int RecipeTypeId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<FavoriteRecipes> FavoriteRecipes { get; set; }
        public virtual List<RecipeIngredients> RecipeIngredients { get; set; }
        public List<Ingredient> IngredientsList { get; set; }
        public Dictionary<int, string> IngredientsWithAmount { get; set; }

        public DetailRecipeViewModel(List<Ingredient> ingredients, List<RecipeIngredients> recipeIngredients)
        {
            IngredientsList = ingredients;
            IngredientsWithAmount = new Dictionary<int, string>();
            foreach (var ingredient in ingredients)
            {
                var recipeIngredient = recipeIngredients.Find(x => x.IngredientId == ingredient.IngredientId);
                if (recipeIngredient != null)
                {
                    IngredientsWithAmount.Add(ingredient.IngredientId, recipeIngredient != null ? recipeIngredient.Amount : "");
                }
            }

        }
    }
}
