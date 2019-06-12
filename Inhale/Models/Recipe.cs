using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string RecipeNotes { get; set; }
        public RecipeType RecipeType { get; set; }
        public int RecipeTypeId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<FavoriteRecipes> FavoriteRecipes { get; set;  }
        public virtual ICollection<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
