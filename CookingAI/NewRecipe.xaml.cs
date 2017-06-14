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
using System.Windows.Shapes;

namespace CookingAI
{
    /// <summary>
    /// Interaction logic for NewRecipe.xaml
    /// </summary>
    public partial class NewRecipe : Window
    {
        List<Ingredient> _localIngredients;
        ObservableCollection<Ingredient> _recipeIngredients = new ObservableCollection<Ingredient>();
        public NewRecipe()
        {
            InitializeComponent();
        }

        private void tbox_RecipeName_GotFocus(object sender, RoutedEventArgs e)
        {
            tbox_RecipeName.Text = string.Empty;
            tbox_RecipeName.Foreground = Brushes.Black;
        }

        private void tbox_RecipeName_KeyUp(object sender, KeyEventArgs e)
        {
            if(App._recipes.Any(i=>i.RecipeName.Equals(tbox_RecipeName.Text)))
            {
                MessageBox.Show("This recipe already exists in our database");
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            addIngredients();
            
        }

        private void addIngredients()
        {
            _localIngredients = new List<Ingredient>(App._ingredients);
            _localIngredients.ForEach(i => { i.IngredientQty = 0; i.IsOptional = false; i.QuantityUnit = string.Empty; });
            cbox_AddIngredients.SelectedIndex = -1;
            cbox_AddIngredients.ItemsSource = _localIngredients;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (_localIngredients != null)
            {
                var elements = (from i in _localIngredients where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in _localIngredients where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            
            Ingredient ingredientTemp = App._ingredients.SingleOrDefault(sc => sc.IngredientName.Equals(((Ingredient)cbox_AddIngredients.SelectedItem).IngredientName));
            if (ingredientTemp == null)
                App._ingredients.Add((Ingredient)cbox_AddIngredients.SelectedItem);

            _recipeIngredients.Add((Ingredient)cbox_AddIngredients.SelectedItem);
            popup_AddNew.IsOpen = false;
            setControls();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
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
                MessageBox.Show("Please Select an ingredient");
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            editIngredients();  
        }

        private void editIngredients()
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                cbox_AddIngredients.ItemsSource = lview_Ingredients.ItemsSource;
                cbox_AddIngredients.SelectedItem = lview_Ingredients.SelectedItem;
                cbox_AddIngredients.IsEnabled = false;
                popup_AddNew.IsOpen = true;
            }
            else
                MessageBox.Show("Please Select an ingredient! ");
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editIngredients();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Owner.Hide();
            setControls();
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
                MessageBox.Show("Give all details of the recipe");
            else
                SaveRecipe();
        }

        private void SaveRecipe()
        {
            List<Ingredient> finalIngredientList = new List<Ingredient>(_recipeIngredients);
            App._recipes.Add(new Recipe { RecipeID = App._recipes.Count+1, RecipeName = tbox_RecipeName.Text, RequiredIngredients = finalIngredientList });
            MessageBox.Show("New recipe successfully added");
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<List<Recipe>>(App._recipes, "recipes.xml");
            Owner.Show();
        }
    }
}
