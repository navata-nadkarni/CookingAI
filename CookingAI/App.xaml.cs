using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CookingAI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<Ingredient> _ingredients = new ObservableCollection<Ingredient>();
        public static List<Recipe> _recipes = new List<Recipe>();
        public static ObservableCollection<Ingredient> _availableIngredients;
        public static ObservableCollection<Ingredient> _allIngredients;
        public static ObservableCollection<Ingredient> _shoppingCart = new ObservableCollection<Ingredient>();
        //public static ObservableCollection<Ingredient> _missingIngredients;

        public App()
        {
            refreshDataFromXML();
        }

        public static void refreshData()
        {
            _availableIngredients = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty != 0 select i).ToList());
            _allIngredients = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty == 0 select i).ToList());
        }
        public static void refreshDataFromXML()
        {
            //_missingIngredients = MyStorage.readXML<ObservableCollection<Ingredient>>("shopping_cart.xml");
            _recipes = MyStorage.readXML<List<Recipe>>("recipes.xml");
            _ingredients = MyStorage.readXML<ObservableCollection<Ingredient>>("ingredients.xml");
            _shoppingCart = MyStorage.readXML<ObservableCollection<Ingredient>>("shoppingCart.xml");
            _availableIngredients = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty != 0 select i).ToList());
            _allIngredients = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty == 0 select i).ToList());
        }

    }
}
