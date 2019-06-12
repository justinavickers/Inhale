using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Inhale.Models
{
    /*
        Add profile data for application users by adding
        properties to the ApplicationUser class
    */
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public virtual ICollection<FavoriteRecipes> FavoriteRecipes { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

      

        /*
            Which resources are related to the User? The code
            below handles a case where a user can create many
            products.
        */
    }
}