using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inhale.Models
{
    public class RecipeType
    {
        public int RecipeTypeId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; } 
    }
}
