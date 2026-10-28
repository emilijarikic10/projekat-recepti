
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Recepti.Services
{
    public class RecipeService
    {
        private readonly string _recipeFilePath;

        public RecipeService()
        {
           
            string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _recipeFilePath = Path.Combine(dataFolder, "recipes.xml");

            
            if (!File.Exists(_recipeFilePath))
                SaveRecipes(new List<Recipe>());
        }

        public List<Recipe> LoadRecipes()
        {
            if (!File.Exists(_recipeFilePath))
                return new List<Recipe>();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Recipe>));
            using (FileStream fs = new FileStream(_recipeFilePath, FileMode.Open))
            {
                return (List<Recipe>)serializer.Deserialize(fs);
            }
        }
        public void UpdateRecipe(Recipe recipe)
        {
            var recipes = LoadRecipes();
            var existing = recipes.FirstOrDefault(r => r.Id == recipe.Id);
            if (existing != null)
            {
                recipes.Remove(existing);
                recipes.Add(recipe);
                SaveRecipes(recipes);
            }
        }
        public int GetNextRecipeId()
        {
            var recipes = LoadRecipes();
            if (recipes.Count == 0) return 1;
            return recipes.Max(r => r.Id) + 1;
        }

        public void SaveRecipes(List<Recipe> recipes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Recipe>));
            using (FileStream fs = new FileStream(_recipeFilePath, FileMode.Create))
            {
                serializer.Serialize(fs, recipes);
            }
        }

        public void AddRecipe(Recipe recipe)
        {
            var recipes = LoadRecipes();
            recipes.Add(recipe);
            SaveRecipes(recipes);
        }

        public void DeleteRecipe(int recipeId)
        {
            var recipes = LoadRecipes();
            var recipe = recipes.FirstOrDefault(r => r.Id == recipeId);

            if (recipe != null)
            {
                recipes.Remove(recipe);
                SaveRecipes(recipes);
            }
        }
    }
}
