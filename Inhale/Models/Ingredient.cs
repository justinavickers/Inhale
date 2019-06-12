using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsOil { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }


    }
}
