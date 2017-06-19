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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CookingAI
{
    /// <summary>
    /// Interaction logic for Manage_Ingredients.xaml
    /// </summary>
    public partial class Manage_Ingredients : Window
    {
        #region global variables
        Boolean _initialState;
        Ingredient _tempIngredient = new Ingredient();
        #endregion
        
        #region constructor
        public Manage_Ingredients()
        {
            InitializeComponent();
            _initialState = false;
        }
        #endregion
        
        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Owner.Hide();
            lview_Ingredients.ItemsSource = App._availableIngredients;
            setControls();
            cbox_AddIngredients.ItemsSource = App._availableIngredients;

        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = 0;
               // ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = string.Empty;
                App.refreshData();
                lview_Ingredients.ItemsSource = App._availableIngredients;
            }
            else
            {
                MessageBox.Show("Please Select an ingredient", "Warning");
            }

        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            tblock_errorMessage.Visibility = Visibility.Hidden;
            addIngredients();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = _tempIngredient.IngredientQty;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = _tempIngredient.QuantityUnit;
                lview_Ingredients.ItemsSource = null;
                lview_Ingredients.ItemsSource = App._availableIngredients;
            }


            popup_AddNew.IsOpen = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Regex check_input = new Regex(@"^[1-9][0-9]*$");
            if (cbox_AddIngredients.SelectedIndex != -1 && check_input.IsMatch(tbox_qty.Text))
            {
                App.refreshData();
                lview_Ingredients.ItemsSource = App._availableIngredients;
                popup_AddNew.IsOpen = false;
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

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            editIngredient();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.goBack();
        }

        private void tbox_Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbox_Filter.Text == string.Empty)
            {
                _initialState = false;
                tbox_Filter.Text = "Filter for Available ingredients";
                tbox_Filter.Foreground = Brushes.DarkGray;
            }

        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is StackPanel || e.OriginalSource is Border)
            {
                editIngredient();
            }

        }

        private void tbox_Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            tbox_Filter.Text = string.Empty;
            tbox_Filter.Foreground = Brushes.Black;
        }

        private void tbox_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialState)
            {
                if (App._availableIngredients != null)
                {
                    var elements = (from i in App._availableIngredients where i.IngredientName.StartsWith(tbox_Filter.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                    var elements_contain = (from i in App._availableIngredients where i.IngredientName.ToLower().Contains(tbox_Filter.Text.ToLower()) select i).ToList();
                    elements.AddRange(elements_contain);
                    lview_Ingredients.ItemsSource = elements.Distinct();
                    setControls();
                    cbox_AddIngredients.IsDropDownOpen = true;
                }
            }
            _initialState = true;
        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                cbox_AddIngredients.SelectedIndex = -1;
                cbox_AddIngredients.IsDropDownOpen = false;
            }

            if (App._allIngredients != null)
            {
                var elements = (from i in App._allIngredients where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in App._allIngredients where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
                cbox_AddIngredients.IsDropDownOpen = true;
            }

        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<ObservableCollection<Ingredient>>(App._ingredients, "ingredients.xml");
        }
        #endregion

        #region methods
        private void addIngredients()
        {
            tblock_errorMessage.Text = string.Empty;
            cbox_AddIngredients.ItemsSource = App._allIngredients;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
        }

        private void editIngredient()
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
                MessageBox.Show("Please Select an ingredient!", "Warning");
        }

        private void setControls()
        {
            if (lview_Ingredients.Items.Count == 0)
            {
                btn_Edit.IsEnabled = false;
                btn_Remove.IsEnabled = false;
            }
            else
            {
                btn_Edit.IsEnabled = true;
                btn_Remove.IsEnabled = true;
            }
        }
        #endregion
    }
}
