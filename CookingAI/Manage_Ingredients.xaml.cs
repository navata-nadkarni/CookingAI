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
        //ObservableCollection<Ingredient> _ingredients = new ObservableCollection<Ingredient>();
        //List<Recipe> _recipes = new List<Recipe>();
        //ObservableCollection<Ingredient> _availableIngredients=new ObservableCollection<Ingredient>();
        //ObservableCollection<Ingredient> _allIngredients;
        Boolean _initialState;
        Ingredient tempIngredient=new Ingredient();
        public Manage_Ingredients()
        {
            InitializeComponent();
            _initialState = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // _recipes = MyStorage.readXML<List<Recipe>>("recipes.xml");
            //_ingredients = MyStorage.readXML<ObservableCollection<Ingredient>>("ingredients.xml");
            //var availableIngredients = from i in _ingredients where i.IngredientQty != 0 select i;
            //_availableIngredients = new ObservableCollection<Ingredient>((from i in _ingredients where i.IngredientQty != 0 select i).ToList());
            Owner.Hide();

            lview_Ingredients.ItemsSource = App._availableIngredients;
            if (App._availableIngredients==null)
            {
                btn_Edit.IsEnabled = false;
                btn_Remove.IsEnabled = false;
            }
            cbox_AddIngredients.ItemsSource = App._availableIngredients;

        }

        private void cbox_AddIngredients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                cbox_AddIngredients.SelectedIndex = -1;
                cbox_AddIngredients.IsDropDownOpen = false;
            }

            if (App._allIngredients!=null)
            {
                var elements = (from i in App._allIngredients where i.IngredientName.StartsWith(cbox_AddIngredients.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                var elements_contain = (from i in App._allIngredients where i.IngredientName.ToLower().Contains(cbox_AddIngredients.Text.ToLower()) select i).ToList();
                elements.AddRange(elements_contain);
                cbox_AddIngredients.ItemsSource = elements.Distinct();
                cbox_AddIngredients.IsDropDownOpen = true;
            }
            
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
           if(lview_Ingredients.SelectedItem!=null)
            {
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = 0;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = "";
                App.refreshData();
                //App._availableIngredients = new ObservableCollection<Ingredient>((from i in App._ingredients where i.IngredientQty != 0 select i).ToList());

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
            AddIngredients();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (lview_Ingredients.SelectedItem != null)
            {
                //lview_Ingredients.ItemsSource = null;
                //lview_Ingredients.ItemsSource = App._availableIngredients;
                ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty = tempIngredient.IngredientQty;
                ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit = tempIngredient.QuantityUnit;
                lview_Ingredients.ItemsSource = null;
                lview_Ingredients.ItemsSource = App._availableIngredients;
            }

           
            popup_AddNew.IsOpen = false;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //var newIngredient = ((Ingredient)cbox_AddIngredients.SelectedItem).IngredientName;
            //((Ingredient)cbox_AddIngredients.SelectedItem).IngredientQty = int.Parse(tbox_qty.Text.ToString());
            //((Ingredient)cbox_AddIngredients.SelectedItem).QuantityUnit = tbox_unit.Text.ToString();
            ////App._availableIngredients = new ObservableCollection<Ingredient>((from i in App._ingredients where i.IngredientQty != 0 select i).ToList());
            ////lview_Ingredients.ItemsSource = App._availableIngredients;
            //btn_Edit.IsEnabled = true;
            //btn_Remove.IsEnabled = true;
            Regex check_input = new Regex(@"^[1-9][0-9]*$");
            if (cbox_AddIngredients.SelectedIndex!=-1 && check_input.IsMatch(tbox_qty.Text))
            {
                App.refreshData();
                lview_Ingredients.ItemsSource = App._availableIngredients;
                popup_AddNew.IsOpen = false;
            }
            else
            {
                tblock_errorMessage.Foreground = Brushes.Red;
                tblock_errorMessage.Visibility = Visibility.Visible;
                if(cbox_AddIngredients.Text==string.Empty || tbox_qty.Text==string.Empty)
                {
                    tblock_errorMessage.Text = "Please enter all the ingredient details";
                }
                else if(!(check_input.IsMatch(tbox_qty.Text))) 
                {
                    tblock_errorMessage.Text = "Quantity is not valid";
                }
                else
                {
                    tblock_errorMessage.Text = "Ingredient does not exist";
                }
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyStorage.storeXML<ObservableCollection<Ingredient>>(App._ingredients, "ingredients.xml");
           // Owner.Show();
        }

        private void tbox_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_initialState)
            {
                if (App._availableIngredients != null)
                {
                    var elements = (from i in App._availableIngredients where i.IngredientName.StartsWith(tbox_Filter.Text, StringComparison.InvariantCultureIgnoreCase) select i).ToList();
                    var elements_contain = (from i in App._availableIngredients where i.IngredientName.ToLower().Contains(tbox_Filter.Text.ToLower()) select i).ToList();
                    elements.AddRange(elements_contain);
                    lview_Ingredients.ItemsSource = elements.Distinct();
                    cbox_AddIngredients.IsDropDownOpen = true;
                }
            }
            _initialState = true;
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            tblock_errorMessage.Visibility = Visibility.Hidden;
            Edit_Ingredient();
                     
        }

        private void Edit_Ingredient()
        {
            //cbox_AddIngredients.ItemsSource = _ingredients;
            //cbox_AddIngredients.DisplayMemberPath = ((Ingredient)lview_Ingredients.SelectedItem).IngredientName;
            if (lview_Ingredients.SelectedItem != null)
            {
                //cbox_AddIngredients.SelectedIndex = -1;
                //if (cbox_AddIngredients.ItemsSource != null) cbox_AddIngredients.ItemsSource = null;
                //cbox_AddIngredients.Items.Add((Ingredient)lview_Ingredients.SelectedItem);
                //cbox_AddIngredients.SelectedItem = (Ingredient)lview_Ingredients.SelectedItem;
                //cbox_AddIngredients.IsEnabled = false;
                //tbox_qty.Text = (((Ingredient)lview_Ingredients.SelectedItem).IngredientQty).ToString();
                //tbox_unit.Text = ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit;
                tempIngredient = new Ingredient
                {
                    IngredientName = ((Ingredient)lview_Ingredients.SelectedItem).IngredientName,
                    IngredientQty = ((Ingredient)lview_Ingredients.SelectedItem).IngredientQty,
                    QuantityUnit = ((Ingredient)lview_Ingredients.SelectedItem).QuantityUnit
                };
                App.refreshData();
                cbox_AddIngredients.ItemsSource = lview_Ingredients.ItemsSource;
                cbox_AddIngredients.SelectedItem = lview_Ingredients.SelectedItem;
                cbox_AddIngredients.IsEnabled = false;
                popup_AddNew.IsOpen = true;
            }
            else
                MessageBox.Show("Please Select an ingredient!","Warning");
        }

        private void btn_AddIngredients_Click(object sender, RoutedEventArgs e)
        {
            AddIngredients();
        }

        private void AddIngredients()
        {
            //App._allIngredients = new ObservableCollection<Ingredient>((from i in App._ingredients where i.IngredientQty == 0 select i).ToList());
            //cbox_AddIngredients.DataContext = this;
            //if (cbox_AddIngredients.ItemsSource == null) cbox_AddIngredients.Items.Clear();

            //cbox_AddIngredients.ItemsSource = App._allIngredients;
            //cbox_AddIngredients.IsEnabled = true;
            //tbox_qty.Text = null;
            //tbox_unit.Text = null; 
            cbox_AddIngredients.ItemsSource = App._allIngredients;
            cbox_AddIngredients.IsEnabled = true;
            popup_AddNew.IsOpen = true;
        }

        private void tbox_Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            tbox_Filter.Text = string.Empty;
            tbox_Filter.Foreground = Brushes.Black;
        }

        private void lview_Ingredients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock || e.OriginalSource is StackPanel || e.OriginalSource is Border)
            {
                Edit_Ingredient();
            }
            
        }

        private void tbox_Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tbox_Filter.Text==string.Empty)
            {
                _initialState = false;
                tbox_Filter.Text = "Filter for Available ingredients";
                tbox_Filter.Foreground = Brushes.DarkGray;
            }
            
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
            //if(mainWindow.spanel_Home.Visibility==Visibility.Hidden)
            //{
                if ((Recipe)Application.Current.Resources["selectedItem"] != null && Application.Current.Resources["noOfPersons"].ToString() != string.Empty)
                {
                    // mainWindow.InitializeComponent();
                    mainWindow.cbox_meals.SelectedItem = (Recipe)Application.Current.Resources["selectedItem"];
                    mainWindow.tbox_Servings.Text = Application.Current.Resources["noOfPersons"].ToString();
                    mainWindow.btn_Check.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            //}
            
        }
    }
}
