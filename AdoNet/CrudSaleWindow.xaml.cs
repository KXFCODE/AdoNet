using System;
using System.Collections.Generic;
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

namespace AdoNet
{
    /// <summary>
    /// Interaction logic for CrusSaleWindow.xaml
    /// </summary>
    public partial class CrudSaleWindow : Window
    {
        public Entity.Sale? Sale { get; set; }
        public CrudSaleWindow(Entity.Sale? sale)
        {
            InitializeComponent();
            this.Sale = sale;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Owner;
            if (Sale is null)
            {
                Sale = new(); 
                DeleteButton.IsEnabled = false;
            }
            else
            {
                SaveButton.IsEnabled = false;
                if (Owner is OrmWindow owner)
                {
                    ProductCombobox.SelectedItem = owner.Products.FirstOrDefault(product => product.Id == Sale.ProductId);
                    ManagerCombobox.SelectedItem = owner.Managers.FirstOrDefault(man => man.Id == Sale.ManagerId);
                }
                else
                {
                    MessageBox.Show("Owner is not OrmWindow");
                }
            }
            ViewId.Text = Sale.Id.ToString();
            ViewSaleDt.Text = Sale.SaleDt.ToString();
            ViewCnt.Text = Sale.Cnt.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Sale is null)
            {
                return;
            }
            if (ViewCnt.Text == String.Empty)
            {
                MessageBox.Show("Enter amount");
                ViewCnt.Focus();
                return;
            }
            int cnt;
            try
            {
                cnt = Convert.ToInt32(ViewCnt.Text);
            }
            catch
            {
                MessageBox.Show("Count is undefined. It must be a number");
                ViewCnt.Focus();
                return;
            }
            if (ProductCombobox.SelectedItem == null)
            {
                MessageBox.Show("Select product");
                ProductCombobox.Focus();
                return;
            }
            if (ManagerCombobox.SelectedItem == null)
            {
                MessageBox.Show("Select manager");
                ManagerCombobox.Focus();
                return;
            }


            this.Sale.Cnt = cnt;
            if (ProductCombobox.SelectedItem is Entity.Products product)
            {
                this.Sale.ProductId = product.Id;
            }
            else
            {
                MessageBox.Show("Error when selecting product");
            }
            if (ManagerCombobox.SelectedItem is Entity.Manager man)
            {
                this.Sale.ManagerId = man.Id;
            }
            else
            {
                MessageBox.Show("Error when selecting manager");
            }



            this.DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Sale = null;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ViewCnt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Sale is not null && Convert.ToInt32(ViewCnt.Text) != this.Sale.Cnt)
            {
                SaveButton.IsEnabled = true;
            }
        }
        private void ManagerCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManagerCombobox.SelectedItem is Entity.Manager manager && this.Sale is not null)
            {
                if (manager.Id != this.Sale.ManagerId)
                {
                    SaveButton.IsEnabled = true;
                }
            }
        }

        private void ProductCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductCombobox.SelectedItem is Entity.Products prod && this.Sale is not null)
            {
                if (prod.Id != this.Sale.ProductId)
                {
                    SaveButton.IsEnabled = true;
                }
            }
        }

    }
}
