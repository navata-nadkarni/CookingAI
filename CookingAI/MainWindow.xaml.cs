using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CookingAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ObservableCollection<Ingredient> availableIngredients = new ObservableCollection<Ingredient>();
        //ObservableCollection<Ingredient> _ingredients = new ObservableCollection<Ingredient>();
        //List<Recipe> _recipes = new List<Recipe>();
        //ObservableCollection<Ingredient> _availableIngredients;
        List<string> ingredientsFound;
        List<string> ingredientsNotFound;

        public MainWindow()
        {
            InitializeComponent();
            //AddAvailableIngredients();
        }

        private void AddAvailableIngredients()
        {
            Recipe rc1 = new Recipe();
            rc1.RecipeID = 1;
            rc1.RecipeName = "Schnitzel";
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 1, IngredientName = "Eggs", IngredientQty = 2, QuantityUnit = "" });
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 12, IngredientName = "Flour", IngredientQty = 200, QuantityUnit = "grams" });
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 13, IngredientName = "Salt", IngredientQty = 2, QuantityUnit = "tbspn" });
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 14, IngredientName = "Pepper", IngredientQty = 2, QuantityUnit = "tbspn" });
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 15, IngredientName = "Paprika", IngredientQty = 2, QuantityUnit = "tbspn" });
            rc1.RequiredIngredients.Add(new Ingredient { IngredientId = 16, IngredientName = "Pork", IngredientQty = 250, QuantityUnit = "grams" });
            App._recipes.Add(rc1);

            Ingredient av1 = new Ingredient { IngredientId = 1, IngredientName = "Eggs", IngredientQty = 12, QuantityUnit = "" };
            Ingredient av2 = new Ingredient { IngredientId = 2, IngredientName = "Chicken", IngredientQty = 400, QuantityUnit = "grams" };
            Ingredient av3 = new Ingredient { IngredientId = 3, IngredientName = "Rice", IngredientQty = 1, QuantityUnit = "Kg" };
            Ingredient av4 = new Ingredient { IngredientId = 4, IngredientName = "Mushrooms", IngredientQty = 500, QuantityUnit = "grams" };
            Ingredient av5 = new Ingredient { IngredientId = 5, IngredientName = "Tomato Puree", IngredientQty = 500, QuantityUnit = "ml" };
            Ingredient av6 = new Ingredient { IngredientId = 6, IngredientName = "Yogurt", IngredientQty = 250, QuantityUnit = "ml" };
            Ingredient av7 = new Ingredient { IngredientId = 7, IngredientName = "Strawberry Yogurt", IngredientQty = 250, QuantityUnit = "ml" };
            Ingredient i1 = new Ingredient { IngredientId = 8, IngredientName = "Spaghetti" };
            Ingredient i2 = new Ingredient { IngredientId = 9, IngredientName = "spinach"};
            Ingredient i3 = new Ingredient { IngredientId = 10, IngredientName = "brocolli"};
            Ingredient i4 = new Ingredient { IngredientId = 11, IngredientName = "Cheese"};
            Ingredient i5 = new Ingredient { IngredientId = 12, IngredientName = "Condensed Milk"};
           
            App._ingredients.Add(av1);
            App._ingredients.Add(av2);
            App._ingredients.Add(av3);
            App._ingredients.Add(av4);
            App._ingredients.Add(av5);
            App._ingredients.Add(av6);
            App._ingredients.Add(av7);
            App._ingredients.Add(i1);
            App._ingredients.Add(i2);
            App._ingredients.Add(i3);
            App._ingredients.Add(i4);
            App._ingredients.Add(i5);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_recipes = MyStorage.readXML<List<Recipe>>("recipes.xml");
            //_ingredients = MyStorage.readXML<ObservableCollection<Ingredient>>("ingredients.xml");
            cbox_meals.ItemsSource = App._recipes;
            tbox_Servings.Text = "1";
            //var availableIngredients = from i in _ingredients where i.IngredientQty !=0 select i;
            //_available = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty != 0 select i).ToList());
            //lview_Ingredients.ItemsSource = availableIngredients;

        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           MyStorage.storeXML<List<Recipe>>(App._recipes, "recipes.xml");
            //MyStorage.storeXML<ObservableCollection<Ingredient>>(_ingredients, "ingredients.xml");
        }

        private void btn_Check_Click(object sender, RoutedEventArgs e)
        {
            if(cbox_meals.SelectedIndex!=-1 && tbox_Servings.Text!=string.Empty)
            {
                spanel_Home.Visibility = Visibility.Hidden;
                spanel_Result.Visibility = Visibility.Visible;
                ingredientsFound = new List<string>();
                ingredientsNotFound = new List<string>();
                checkPossibility(((Recipe)cbox_meals.SelectedItem).RecipeName.ToString());
                lbox_ingredientsRequired.ItemsSource = ((Recipe)cbox_meals.SelectedItem).RequiredIngredients;
                if (ingredientsNotFound.Count == 0)
                {
                    tblock_MissingIngredients.Text = "";
                    tblock_result.Text = "It is possible for you to make " + ((Recipe)cbox_meals.SelectedItem).RecipeName.ToString() + " for ";
                    lbox_ingredientsRequired.IsEnabled = false;
                    //MAKE BUTTON AND ENABLE IT!

                    //MessageBox.Show("Possible and you will need ");
                    //for (int i = 0; i < ingredientsFound.Count; i++)
                    //{
                    //    MessageBox.Show(ingredientsFound[i].ToString());

                    //}

                }
                else
                {
                    tblock_MissingIngredients.Text = "";
                    tblock_result.Text = "It is not possible for you to make " + ((Recipe)cbox_meals.SelectedItem).RecipeName.ToString() + " for ";
                    tblock_MissingIngredients.Foreground = Brushes.Red;
                    tblock_MissingIngredients.Text = "You will need ";
                    for (int i = 0; i < ingredientsNotFound.Count; i++)
                    {
                        tblock_MissingIngredients.Text = tblock_MissingIngredients.Text + ingredientsNotFound[i] + ", ";
                    }

                    lbox_ingredientsRequired.IsEnabled = false;
                    // tblock_MissingIngredients.Text = "You dont have " + String.Concat(ingredientsNotFound);
                    //MessageBox.Show("Not possible and you will need ");
                    //String.Concat(ingredientsNotFound);

                }
                btn_Home.Visibility = Visibility.Visible;
            }
            //tblock_result.Visibility = Visibility.Visible;
            
        }

        private void checkPossibility(string selectedRecipeName)
        {
            int portionSize = int.Parse(tbox_Servings.Text.ToString());
            Recipe selectedRecipe = App._recipes.SingleOrDefault(r => r.RecipeName.Equals(selectedRecipeName));
            App._availableIngredients = new ObservableCollection<Ingredient>((from i in App._ingredients where i.IngredientQty != 0 select i).ToList());
            if (selectedRecipe == null || selectedRecipe.RequiredIngredients == null
                || selectedRecipe.RequiredIngredients.Count == 0)
            {   //During a rcursive call this will check if an unavailable ingredient has a recipe or not
                ingredientsNotFound.Add(selectedRecipeName);
            }
            else
            {
                Boolean flag;
                foreach (Ingredient ingredientRequired in selectedRecipe.RequiredIngredients)
                {
                    flag = false;
                    foreach (Ingredient ingredientAvailable in App._availableIngredients)
                    {//compare name of ingredients and remove id from available ingredient class
                        if (ingredientsFound.Contains(ingredientAvailable.IngredientId.ToString()) || ingredientsNotFound.Contains(ingredientAvailable.IngredientId.ToString()))
                        {
                            continue;
                        }
                        else
                        {
                            if (ingredientRequired.IngredientName == ingredientAvailable.IngredientName && ingredientRequired.IngredientQty <= ingredientAvailable.IngredientQty && ingredientRequired.QuantityUnit == ingredientAvailable.QuantityUnit)
                            {
                                ingredientsFound.Add(ingredientAvailable.IngredientName);
                                flag = true;
                                break;
                            }
                        }

                    }
                    if (!flag)
                    {//Flag = false means the ingredient is not present in available ingredients
                     // now we have to check if this particular ingredient is a recipe itself.
                        checkPossibility(ingredientRequired.IngredientName);
                    }
                    // MessageBox.Show(ingredientRequired.ingredientId.ToString());
                }
            }
        }

        private void btn_ManageIngredients_Click(object sender, RoutedEventArgs e)
        {
            Manage_Ingredients manageIngredients = new Manage_Ingredients();
            manageIngredients.Owner = this;
            //this.Visibility = Visibility.Hidden;
            manageIngredients.ShowDialog();
        }

        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            spanel_Home.Visibility = Visibility.Visible;
            spanel_Result.Visibility = Visibility.Hidden;
            tblock_result.Text = string.Empty;
            cbox_meals.SelectedIndex = -1;
            btn_Home.Visibility = Visibility.Hidden;
        }
    }
}
