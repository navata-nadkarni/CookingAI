using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Shopping_Cart.xaml
    /// </summary>
    public partial class Shopping_Cart : Window
    {
        #region global variables
        ObservableCollection<Ingredient> _shoppingCartOptions;
            Boolean _initialState;
            Ingredient _tempIngredient = new Ingredient();
        #endregion

        #region constructor
        public Shopping_Cart()
        {
            _initialState = false;
            InitializeComponent();
           
        }
        #endregion

        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Owner.Hide();
            initializeWindow();
            
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = 0;
                //((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = string.Empty;
                _shoppingCartOptions.Add((Ingredient)lview_Ingredients.SelectedItem);
                App._shoppingCart.Remove((Ingredient)lview_Ingredients.SelectedItem);
                lview_Ingredients.ItemsSource = App._shoppingCart;
                setControls();
            }
            else
            {
                MessageBox.Show("Please Select an ingredient", "Warning");
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            tblock_errorMessage.Text = string.Empty;
            addIngredients();
        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                cbox_AddIngredients.SelectedIndex = -1;
            }
            if (App._allIngredients != null)
            {
                var elements = (from i in _shoppingCartOptions where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in _shoppingCartOptions where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
                cbox_AddIngredients.IsDropDownOpen = true;
            }
        }

        private void tbox_Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            tbox_Filter.Text = string.Empty;
            tbox_Filter.Foreground = Brushes.Black;
        }

        private void tbox_Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbox_Filter.Text == string.Empty)
            {
                _initialState = false;
                tbox_Filter.Text = "Filter for Shopping Cart";
                tbox_Filter.Foreground = Brushes.DarkGray;
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            editIngredient();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = _tempIngredient.IngredientQty;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = _tempIngredient.QuantityUnit;
                lview_Ingredients.ItemsSource = null;
                lview_Ingredients.ItemsSource = App._shoppingCart;
            }

            popup_AddNew.IsOpen = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Regex check_input = new Regex(@"^[1-9][0-9]*$");
            if (cbox_AddIngredients.SelectedIndex != -1 && check_input.IsMatch(tbox_qty.Text))
            {
                Ingredient ingredientTemp = App._shoppingCart.SingleOrDefault(sc => sc.IngredientName.Equals(((Ingredient)cbox_AddIngredients.SelectedItem).IngredientName));
                if (ingredientTemp == null)
                {
                    App._shoppingCart.Add((Ingredient)cbox_AddIngredients.SelectedItem);

                }
                _shoppingCartOptions.Remove((Ingredient)cbox_AddIngredients.SelectedItem);
                lview_Ingredients.ItemsSource = App._shoppingCart;
                setControls();
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

        private void tbox_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initialState)
            {
                if (App._availableIngredients != null)
                {
                    var elements = (from i in App._shoppingCart where i.IngredientName.StartsWith(tbox_Filter.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                    var elements_contain = (from i in App._shoppingCart where i.IngredientName.ToLower().Contains(tbox_Filter.Text.ToLower()) select i).ToList();
                    elements.AddRange(elements_contain);
                    lview_Ingredients.ItemsSource = elements.Distinct();
                    setControls();
                }
            }
            _initialState = true;
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is StackPanel || e.OriginalSource is Border)
            {
                editIngredient();
            }
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            string text = getTextToBeExported();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, text.ToString());
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            App._shoppingCart.Clear();
            refreshData();
            initializeWindow();

        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.goBack();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<ObservableCollection<Ingredient>>(App._shoppingCart, "shoppingCart.xml");
        }
        #endregion

        #region methods
        private void initializeWindow()
        {
            
            lview_Ingredients.ItemsSource = App._shoppingCart;
            setControls();
            refreshData();
            cbox_AddIngredients.ItemsSource = _shoppingCartOptions;
        }

        private void refreshData()
        {
            _shoppingCartOptions = new ObservableCollection<Ingredient>();
            foreach (Ingredient item in App._allIngredients)
            {
                item.IngredientQty = 0;
                //item.QuantityUnit = string.Empty;
                _shoppingCartOptions.Add(item);
            }


            foreach (Ingredient item in App._shoppingCart)
            {
                Ingredient temp = _shoppingCartOptions.SingleOrDefault(i => i.IngredientName.Equals(item.IngredientName));

                if (temp != null)
                    _shoppingCartOptions.Remove(temp);
            }
        }

        private void setControls()
        {
            if (lview_Ingredients.Items.Count == 0)
            {
                btn_Edit.IsEnabled = false;
                btn_Remove.IsEnabled = false;
                btn_Export.IsEnabled = false;
                btn_Clear.IsEnabled = false;
            }
            else
            {
                btn_Edit.IsEnabled = true;
                btn_Remove.IsEnabled = true;
                btn_Export.IsEnabled = true;
                btn_Clear.IsEnabled = true;
            }
        }

        private void addIngredients()
        {
            cbox_AddIngredients.SelectedIndex = -1;
            cbox_AddIngredients.ItemsSource = null;
            cbox_AddIngredients.ItemsSource = _shoppingCartOptions;
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
                MessageBox.Show("Please Select an ingredient! ", "Warning");
        }

        private string getTextToBeExported()
        {
            StringBuilder sbObj = new StringBuilder();
            foreach (Ingredient item in App._shoppingCart)
            {
                sbObj.Append(item.IngredientName + " - " + item.IngredientQty + " " + item.QuantityUnit + Environment.NewLine);
            }
            return sbObj.ToString();
        }


        #endregion
    }
}
