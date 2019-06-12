using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class RecipeIngredients
    {
        [Key]
        public int RecipeIngredientId { get; set; }
        public int IngredientId { get; set; }
        public string Amount { get; set; }
        public Ingredient Ingredient { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
