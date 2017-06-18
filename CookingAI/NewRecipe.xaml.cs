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
        List<Ingredient> _localIngredients;
        Ingredient tempIngredient = new Ingredient();
        ObservableCollection<Ingredient> _recipeIngredients = new ObservableCollection<Ingredient>();
        public NewRecipe()
        {
            InitializeComponent();
        }

        private void tbox_RecipeName_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbox_RecipeName.Text == "Enter Recipe Name")
                tbox_RecipeName.Text = string.Empty;
            tbox_RecipeName.Foreground = Brushes.Black;
        }

        private void tbox_RecipeName_KeyUp(object sender, KeyEventArgs e)
        {
            if(App._recipes.Any(i=>i.RecipeName.ToLower().Equals(tbox_RecipeName.Text.ToLower())))
            {
                MessageBox.Show("This recipe already exists in our database","Warning");
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            addIngredients();
            
        }

        private void addIngredients()
        {
           //_localIngredients = new List<Ingredient>(App._ingredients);
            tblock_errorMessage.Text = string.Empty;
            _localIngredients.ForEach(i => { i.IngredientQty = 0; i.IsOptional = false; i.QuantityUnit = string.Empty; });
            cbox_AddIngredients.SelectedIndex = -1;
            cbox_AddIngredients.ItemsSource = null;
            cbox_AddIngredients.ItemsSource = _localIngredients;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Back || e.Key== Key.Delete)
            {
                cbox_AddIngredients.SelectedIndex = -1;
                //cbox_AddIngredients.IsDropDownOpen = false;
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
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = tempIngredient.IngredientQty;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = tempIngredient.QuantityUnit;
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
                MessageBox.Show("Please Select an ingredient","Warning");
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            editIngredients();  
        }

        private void editIngredients()
        {
            tblock_errorMessage.Text = string.Empty;
            if (lview_Ingredients.SelectedItem != null)
            {
                tempIngredient = new Ingredient
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
                MessageBox.Show("Please Select an ingredient","Warning");
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is StackPanel || e.OriginalSource is Border)
            {
                editIngredients();
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Owner.Hide();
            setControls();
            
            _localIngredients = App._ingredients.Select(i => new Ingredient { IngredientName = i.IngredientName, IngredientQty = 0, QuantityUnit = string.Empty, IsOptional = false }).ToList();
            lview_Ingredients.ItemsSource = _recipeIngredients;
        }

        private void setControls()
        {
            if(_recipeIngredients.Count==0)
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

        private void tbox_RecipeName_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbox_RecipeName.Text==string.Empty)
            {
                tbox_RecipeName.Text = "Enter Recipe Name";
                tbox_RecipeName.Foreground = Brushes.DarkGray;
            }
        }

        private void btn_SaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (tbox_RecipeName.Text == "Enter Recipe Name" || _recipeIngredients.Count == 0)
                MessageBox.Show("Give all details of the recipe","Warning");
            else
                SaveRecipe();
        }

        private void SaveRecipe()
        {
            List<Ingredient> finalIngredientList = new List<Ingredient>(_recipeIngredients);
            App._recipes.Add(new Recipe { RecipeID = App._recipes.Count+1, RecipeName = tbox_RecipeName.Text, RequiredIngredients = finalIngredientList });
            MessageBox.Show("New recipe successfully added","Success");
            btn_back.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

           // mainWindow.btn_Check.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<List<Recipe>>(App._recipes, "recipes.xml");
            //this.Close();
            //Owner.Show();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["isHome"]= true;
            this.Close();
            App.goBack();
        }
    }
}
