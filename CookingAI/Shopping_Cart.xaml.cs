﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for Shopping_Cart.xaml
    /// </summary>
    public partial class Shopping_Cart : Window
    {
        ObservableCollection<Ingredient> _shoppingCartOptions = new ObservableCollection<Ingredient>();
        Boolean _initialState;
        public Shopping_Cart()
        {
            InitializeComponent();
            _initialState = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lview_Ingredients.ItemsSource = App._shoppingCart;
            if (App._shoppingCart == null)
            {
                btn_Edit.IsEnabled = false;
                btn_Remove.IsEnabled = false;
            }

            foreach (Ingredient item in App._ingredients)
            {
                _shoppingCartOptions.Add(item);
            }

            //_shoppingCartOptions = ((ObservableCollection<Ingredient>)_shoppingCartOptions.Where(x => !App._shoppingCart.Any(y => y.IngredientName == x.IngredientName)));

            foreach (Ingredient item in App._shoppingCart)
            {
                Ingredient temp = _shoppingCartOptions.SingleOrDefault(i => i.IngredientName.Equals(item.IngredientName));
               // Ingredient temp = (Ingredient)from i in _shoppingCartOptions where i.IngredientName == item.IngredientName select i;
                if (temp != null)
                    _shoppingCartOptions.Remove(temp);
            }
            cbox_AddIngredients.ItemsSource = _shoppingCartOptions;
        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (App._allIngredients != null)
            {
                var elements = (from i in _shoppingCartOptions where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in _shoppingCartOptions where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
            }
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                App._shoppingCart.Remove((Ingredient)lview_Ingredients.SelectedItem);
            }
            else
            {
                MessageBox.Show("Please Select an ingredient");
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddIngredients();
        }

        private void AddIngredients()
        {
            cbox_AddIngredients.ItemsSource = _shoppingCartOptions;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
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
            Edit_Ingredient();
        }

        private void Edit_Ingredient()
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

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            popup_AddNew.IsOpen = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Ingredient ingredientTemp = App._shoppingCart.SingleOrDefault(sc => sc.IngredientName.Equals(((Ingredient)cbox_AddIngredients.SelectedItem).IngredientName));
            if(ingredientTemp==null)
                App._shoppingCart.Add((Ingredient)cbox_AddIngredients.SelectedItem);
            
            popup_AddNew.IsOpen = false;
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
                }
            }
            _initialState = true;
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit_Ingredient();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<ObservableCollection<Ingredient>>(App._shoppingCart, "shoppingCart.xml");
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

        private string getTextToBeExported()
        {
            StringBuilder sbObj = new StringBuilder();
            foreach (Ingredient item in App._shoppingCart)
            {
                sbObj.Append(item.IngredientName + " - " + item.IngredientQty + " " + item.QuantityUnit + Environment.NewLine);
            }
            return sbObj.ToString();
        }
    }
}