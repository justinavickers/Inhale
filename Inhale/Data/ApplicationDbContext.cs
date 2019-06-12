using System;
using System.Collections.Generic;
using System.Text;
using Inhale.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inhale.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){ }
            

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FavoriteRecipes> FavoriteRecipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<RecipeType> RecipeType { get; set; }

    



    }
}


