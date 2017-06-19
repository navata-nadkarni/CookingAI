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
    /// Interaction logic for NewRecipe.xaml
    /// </summary>
    public partial class NewRecipe : Window
    {

        #region global variables 
            List<Ingredient> _localIngredients;
            Ingredient _tempIngredient = new Ingredient();
            ObservableCollection<Ingredient> _recipeIngredients = new ObservableCollection<Ingredient>();
        #endregion

        #region constructor

            public NewRecipe()
        {
            InitializeComponent();
        }

        #endregion

        #region events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Owner.Hide();
            setControls();

            _localIngredients = App._ingredients.Select(i => new Ingredient { IngredientName = i.IngredientName, IngredientQty = 0, QuantityUnit = string.Empty, IsOptional = false }).ToList();
            lview_Ingredients.ItemsSource = _recipeIngredients;
        }



        private void tbox_RecipeName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbox_RecipeName.Text == string.Empty)
            {
                tbox_RecipeName.Text = "Enter Recipe Name";
                tbox_RecipeName.Foreground = Brushes.DarkGray;
            }
        }

        private void btn_SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (tbox_RecipeName.Text == "Enter Recipe Name" || _recipeIngredients.Count == 0)
                MessageBox.Show("Give all details of the recipe", "Warning");
            else
                saveRecipe();
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is StackPanel || e.OriginalSource is Border)
            {
                editIngredients();
            }

        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                cbox_AddIngredients.SelectedIndex = -1;
            }
            if (_localIngredients != null)
            {
                var elements = (from i in _localIngredients where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in _localIngredients where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
                cbox_AddIngredients.IsDropDownOpen = true;
            }
        }

        private void tbox_RecipeName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbox_RecipeName.Text == "Enter Recipe Name")
                tbox_RecipeName.Text = string.Empty;
            tbox_RecipeName.Foreground = Brushes.Black;
        }

        private void tbox_RecipeName_KeyUp(object sender, KeyEventArgs e)
        {
            if (App._recipes.Any(i => i.RecipeName.ToLower().Equals(tbox_RecipeName.Text.ToLower())))
            {
                MessageBox.Show("This recipe already exists in our database", "Warning");
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            addIngredients();

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Regex check_input = new Regex(@"^[1-9][0-9]*$");
            if (cbox_AddIngredients.SelectedIndex != -1 && check_input.IsMatch(tbox_qty.Text))
            {
                _localIngredients.Remove((Ingredient)cbox_AddIngredients.SelectedItem);
                Ingredient ingredientTemp = _recipeIngredients.SingleOrDefault(sc => sc.IngredientName.Equals(((Ingredient)cbox_AddIngredients.SelectedItem).IngredientName));
                if (ingredientTemp == null)
                {
                    _recipeIngredients.Add((Ingredient)cbox_AddIngredients.SelectedItem);
                }
                lview_Ingredients.ItemsSource = _recipeIngredients;
                popup_AddNew.IsOpen = false;
                setControls();
            }
            else
            {
                tblock_errorMessage.Foreground = Brushes.Red;
                tblock_errorMessage.Visibility = Visibility.Visible;
                if (cbox_AddIngredients.Text == string.Empty || tbox_qty.Text == string.Empty)
                {
                    tblock_errorMessage.Text = "Please enter all the ingredient details";
                }
                else if (!(check_input.IsMatch(tbox_qty.Text)))
                {
                    tblock_errorMessage.Text = "Quantity is not valid";
                }
                else
                {
                    tblock_errorMessage.Text = "Ingredient does not exist";
                }
            }

        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = _tempIngredient.IngredientQty;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = _tempIngredient.QuantityUnit;
                lview_Ingredients.ItemsSource = null;
                lview_Ingredients.ItemsSource = _recipeIngredients;
            }
            popup_AddNew.IsOpen = false;
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = 0;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = string.Empty;
                _localIngredients.Add((Ingredient)lview_Ingredients.SelectedItem);
                _recipeIngredients.Remove((Ingredient)lview_Ingredients.SelectedItem);
                setControls();
            }
            else
            {
                MessageBox.Show("Please Select an ingredient", "Warning");
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            editIngredients();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["isHome"] = true;
            this.Close();
            App.goBack();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<List<Recipe>>(App._recipes, "recipes.xml");

        }

        
        #endregion

        #region methods
        private void addIngredients()
        {
            tblock_errorMessage.Text = string.Empty;
            _localIngredients.ForEach(i => { i.IngredientQty = 0; i.IsOptional = false; i.QuantityUnit = string.Empty; });
            cbox_AddIngredients.SelectedIndex = -1;
            cbox_AddIngredients.ItemsSource = null;
            cbox_AddIngredients.ItemsSource = _localIngredients;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
        }

        private void editIngredients()
        {
            tblock_errorMessage.Text = string.Empty;
            if (lview_Ingredients.SelectedItem != null)
            {
                _tempIngredient = new Ingredient
                {
                    IngredientName = ((Ingredient)lview_Ingredients.SelectedItem).IngredientName,
                    IngredientQty = ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty,
                    QuantityUnit = ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit
                };
                App.refreshData();
                cbox_AddIngredients.ItemsSource = null;
                cbox_AddIngredients.ItemsSource = lview_Ingredients.ItemsSource;
                cbox_AddIngredients.SelectedItem = lview_Ingredients.SelectedItem;
                cbox_AddIngredients.IsEnabled = false;
                popup_AddNew.IsOpen = true;
            }
            else
                MessageBox.Show("Please Select an ingredient", "Warning");
        }

        private void setControls()
        {
            if (_recipeIngredients.Count == 0)
            {
                btn_Edit.IsEnabled = false;
                btn_Remove.IsEnabled = false;
                btn_SaveRecipe.IsEnabled = false;
            }
            else
            {
                btn_Edit.IsEnabled = true;
                btn_Remove.IsEnabled = true;
                btn_SaveRecipe.IsEnabled = true;
            }
        }

        private void saveRecipe()
        {
            List<Ingredient> finalIngredientList = new List<Ingredient>(_recipeIngredients);
            App._recipes.Add(new Recipe { RecipeID = App._recipes.Count + 1, RecipeName = tbox_RecipeName.Text, RequiredIngredients = finalIngredientList });
            MessageBox.Show("New recipe successfully added", "Success");
            btn_back.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        #endregion

    }
}
