using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class FavoriteRecipes
    {
        public int FavoriteRecipesId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

    }
}
