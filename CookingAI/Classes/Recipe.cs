using System.Collections.Generic;

namespace CookingAI
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public string RecipeName { get; set; }
        public List<Ingredient> RequiredIngredients { get; set; }
        public Recipe()
        {
            RequiredIngredients = new List<Ingredient>();
        }
    }
}