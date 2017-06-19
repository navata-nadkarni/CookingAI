using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CookingAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region global variables
        List<Ingredient> ingredientsAbsent=new List<Ingredient>();
        List<Ingredient> ingredientsPresent = new List<Ingredient>();
        int portionSize;
        List<Recipe> recipeItemSource;
        #endregion

        #region constructor for window
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeWindow();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<List<Recipe>>(App._recipes, "recipes.xml");
        }

        private void btn_Check_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = checkPortionValue();
            if (isSuccess)
            {
                if (cbox_meals.SelectedIndex != -1 && tbox_Servings.Text != string.Empty)
                {
                    spanel_Home.Visibility = Visibility.Hidden;
                    spanel_Result.Visibility = Visibility.Visible;
                    ingredientsAbsent = new List<Ingredient>();
                    ingredientsPresent = new List<Ingredient>();

                    checkIfPossible((Recipe)cbox_meals.SelectedItem);
                    lbox_ingredientsRequired.ItemsSource = ((Recipe)cbox_meals.SelectedItem).RequiredIngredients;
                    if (ingredientsAbsent.Count == 0)
                    {
                       
                        tblock_result.Text = "It is possible for you to make " + ((Recipe)cbox_meals.SelectedItem).RecipeName.ToString() + " for ";
                        lbox_MissingIngredients.Visibility = Visibility.Hidden;
                        btn_addToCart.Visibility = Visibility.Hidden;
                        btn_updateRec.Visibility = Visibility.Visible;
                        spanel_headMissing.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        if (ingredientsAbsent.Any(i => i.IsOptional.Equals(false)))
                        {
                            tblock_result.Text = "It is not possible for you to make " + ((Recipe)cbox_meals.SelectedItem).RecipeName.ToString() + " for ";
                            btn_updateRec.Visibility = Visibility.Hidden;

                        }
                        else
                        {
                            tblock_result.Text = "It is possible for you to make " + ((Recipe)cbox_meals.SelectedItem).RecipeName.ToString() + " for ";
                            btn_updateRec.Visibility = Visibility.Visible;
                        }
                        spanel_headMissing.Visibility = Visibility.Visible;
                        lbox_MissingIngredients.ItemsSource = ingredientsAbsent;
                        spanel_headMissing.Visibility = Visibility.Visible;
                        lbox_MissingIngredients.Visibility = Visibility.Visible;
                        btn_addToCart.Visibility = Visibility.Visible;
                      
                    }
                    btn_Home.Visibility = Visibility.Visible;
                }
                else
                {
                    if (cbox_meals.Text == string.Empty)
                        MessageBox.Show("Please enter meal details", "Warning");
                    else
                        MessageBox.Show("Recipe does not exist", "Error");
                }
            }


        }

        private void btn_ManageIngredients_Click(object sender, RoutedEventArgs e)
        {
            Manage_Ingredients manageIngredients = new Manage_Ingredients();
            manageIngredients.Owner = this;
            setSessionVariables();
            manageIngredients.Show();
        }

        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            tblock_result.Text = string.Empty;
            cbox_meals.SelectedIndex = -1;
            initializeWindow();
        }

        private void tbox_resultPortion_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (spanel_Home.Visibility == Visibility.Hidden)
                btn_Check.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void btn_addToCart_Click(object sender, RoutedEventArgs e)
        {

            foreach (Ingredient missingIngredient in ingredientsAbsent)
            {
                if (App._shoppingCart != null)
                {
                    Ingredient checkIfExists = (Ingredient)App._shoppingCart.SingleOrDefault(i => i.IngredientName.Equals(missingIngredient.IngredientName));
                    if (checkIfExists == null)
                    {
                        App._shoppingCart.Add(missingIngredient);
                    }
                    else
                    {
                        checkIfExists.IngredientQty = checkIfExists.IngredientQty + missingIngredient.IngredientQty;
                       
                    }
                }
                else
                {
                    App._shoppingCart.Add(missingIngredient);
                }


            }
            MyStorage.storeXML<ObservableCollection<Ingredient>>(App._shoppingCart, "shoppingCart.xml");
            MessageBox.Show("Ingredients successfully added to cart", "Success");
        }

        private void btn_ViewCart_Click(object sender, RoutedEventArgs e)
        {
            Shopping_Cart sCart = new Shopping_Cart();
            sCart.Owner = this;
            setSessionVariables();
            sCart.Show();
        }

        private void cbox_meals_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                cbox_meals.SelectedIndex = -1;
                cbox_meals.IsDropDownOpen = false;
            }

            if (App._recipes != null)
            {
                var elements = (from r in App._recipes where r.RecipeName.StartsWith(cbox_meals.Text, StringComparison.InvariantCultureIgnoreCase) select r).ToList();
                var elements_contain = (from r in App._recipes where r.RecipeName.ToLower().Contains(cbox_meals.Text.ToLower()) select r).ToList();
                elements.AddRange(elements_contain);
                elements.Add(new Recipe { RecipeName = "Add new Recipe" });
                cbox_meals.ItemsSource = elements.Distinct();
                cbox_meals.IsDropDownOpen = true;

            }

        }

        private void cbox_meals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_meals.SelectedItem != null)
            {
                if (((Recipe)cbox_meals.SelectedItem).RecipeName == "Add new Recipe")
                {
                    NewRecipe newRecipe = new NewRecipe();
                    newRecipe.Owner = this;
                    newRecipe.Show();

                }
                else if (spanel_Home.Visibility == Visibility.Hidden)
                    btn_Check.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }

        }

        private void btn_updateRec_Click(object sender, RoutedEventArgs e)
        {
            foreach (Ingredient item in ingredientsPresent)
            {
                Ingredient temp = App._availableIngredients.SingleOrDefault(i => i.IngredientName.Equals(item.IngredientName));
                if (temp != null)
                {
                    temp.IngredientQty = temp.IngredientQty - item.IngredientQty;
                    if (temp.IngredientQty == 0)
                        temp.QuantityUnit = string.Empty;
                }

            }
            App.refreshData();
            MessageBox.Show("Your available ingredients have been updated", "Success");
            cbox_meals.SelectedIndex = -1;
            initializeWindow();
        }

        #endregion events

        #region methods
        private void initializeWindow()
        {
            goToHomeWindow();
            recipeItemSource = new List<Recipe>(App._recipes);
            recipeItemSource.Add(new Recipe { RecipeName = "Add new Recipe" });
            cbox_meals.ItemsSource = recipeItemSource;
            tbox_Servings.Text = "1";
        }

        //implements depth first search to find out if it is possible to make a recipe or not.
        private void checkIfPossible(Recipe selectedRecipe)
        {
            portionSize = int.Parse(tbox_Servings.Text.ToString());
            Ingredient ingredientToCheck = new Ingredient();
            Ingredient ingredientTemp;
            foreach (Ingredient requiredIngredient in selectedRecipe.RequiredIngredients)
            {
                ingredientToCheck = App._availableIngredients.SingleOrDefault(i => i.IngredientName.Equals(requiredIngredient.IngredientName));
                if (!(ingredientToCheck == null))
                {
                    if (ingredientToCheck.IngredientQty >= (requiredIngredient.IngredientQty * portionSize))
                    {
                        ingredientsPresent.Add(requiredIngredient);
                    }
                    else
                    {
                        ingredientTemp = new Ingredient();
                        ingredientTemp.IngredientName = requiredIngredient.IngredientName.ToString();
                        ingredientTemp.IngredientQty = (requiredIngredient.IngredientQty * portionSize) - ingredientToCheck.IngredientQty;
                        ingredientTemp.QuantityUnit = requiredIngredient.QuantityUnit;
                        ingredientTemp.IsOptional = requiredIngredient.IsOptional;

                        ingredientsAbsent.Add(ingredientTemp);
                    }

                }
                else
                {
                    Recipe rec = App._recipes.SingleOrDefault(r => r.RecipeName.Equals(requiredIngredient.IngredientName));
                    if (rec == null)
                    {
                        ingredientTemp = new Ingredient();
                        ingredientTemp.IngredientName = requiredIngredient.IngredientName.ToString();
                        ingredientTemp.IngredientQty = requiredIngredient.IngredientQty * portionSize;
                        ingredientTemp.QuantityUnit = requiredIngredient.QuantityUnit;
                        ingredientTemp.IsOptional = requiredIngredient.IsOptional;

                        ingredientsAbsent.Add(ingredientTemp);
                    }
                    else
                    {
                        checkIfPossible(rec);
                    }
                }

            }

        }

        private void setSessionVariables()
        {
            Application.Current.Resources["selectedItem"] = (Recipe)cbox_meals.SelectedItem;
            Application.Current.Resources["noOfPersons"] = tbox_Servings.Text;
            if (spanel_Home.Visibility == Visibility.Visible)
                Application.Current.Resources["isHome"] = true;
            else
                Application.Current.Resources["isHome"] = false;
        }

        //validation of number of portions/servings
        private bool checkPortionValue()
        {
            Regex check_input = new Regex(@"^[1-9][0-9]*$");
            if (check_input.IsMatch(tbox_Servings.Text))
            {
                return true;
            }
            else
            {
                goToHomeWindow();
                MessageBox.Show("Please enter valid no. of persons/servings", "Warning");
                return false;
            }
        }

        //resetting the homepage controls visibility
        private void goToHomeWindow()
        {
            spanel_Home.Visibility = Visibility.Visible;
            spanel_Result.Visibility = Visibility.Hidden;
            btn_addToCart.Visibility = Visibility.Hidden;
            btn_updateRec.Visibility = Visibility.Hidden;
            btn_Home.Visibility = Visibility.Hidden;
        }

        #endregion

    }
}
