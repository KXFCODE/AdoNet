using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
    /// Interaction logic for OrmWindow.xaml
    /// </summary>
    public partial class OrmWindow : Window
    {
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public ObservableCollection<Entity.Products> Products { get; set; }
        public ObservableCollection<Entity.Sale> Sales { get; set; }

        private readonly MySqlConnection _connection;

        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Managers = new();
            Products = new();
            Sales = new();
            this.DataContext = this;   // місце пошуку {Binding Departments}
            _connection = new(App.ConnectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                MySqlCommand cmd = new() { Connection = _connection };

                #region Load Departments
                cmd.CommandText = "SELECT D.Id, D.Name FROM Departments D";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(
                        new Entity.Department
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                }
                reader.Close();
                #endregion

                #region Load Products
                cmd.CommandText = "SELECT P.* FROM Products P WHERE P.DeleteDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new(reader));
                }
                reader.Close();
                #endregion

                #region Load Managers
                cmd.CommandText = "SELECT M.Id, M.Surname, M.Name, M.Secname, M.Id_main_Dep, M.Id_sec_dep, M.Id_chief, M.FiredDt FROM Managers M";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(
                        new Entity.Manager
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                            Id_main_dep = reader.GetGuid(4),
                            Id_sec_dep = reader.GetValue(5) == DBNull.Value
                                        ? null
                                        : reader.GetGuid(5),
                            Id_chief = reader.IsDBNull(6)
                                        ? null
                                        : reader.GetGuid(6),
                            FiredDt = reader.IsDBNull(7)
                                        ? null
                                        : reader.GetDateTime(7)
                        });
                }
                reader.Close();
                #endregion

                #region Load Sales
                cmd.CommandText = "SELECT S.* FROM Sales S";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sales.Add(new(reader));
                }
                reader.Close();
                #endregion

                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Window will be closed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    CrudDepartmentWindow dialog = new(department);
                    if (dialog.ShowDialog() == true)  // виконана (підтверджена) дія
                    {
                        if (dialog.EditedDepartment is null)  // дія - видалення
                        {
                            Departments.Remove(department);
                            MessageBox.Show("Видалення: " + department.Name);
                        }
                        else  // дія - збереження
                        {
                            int index = Departments.IndexOf(department);
                            Departments.Remove(department);
                            Departments.Insert(index, department);
                            MessageBox.Show("Оновлення: " + department.Name);
                        }
                    }
                    else  // вікно закрите або натиснуто Cancel
                    {
                        MessageBox.Show("Дію скасовано");
                    }
                }
            }
        }



        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    CrudManagerWindow dialog = new(manager) { Owner = this };
                    if (dialog.ShowDialog() == true)
                    {

                    }

                    // MessageBox.Show(manager.Surname);
                }
            }
        }

        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Products product)
                {
                    CrudProductWindow dialog = new(product);
                    if (dialog.ShowDialog() == true)  // виконана (підтверджена) дія
                    {
                        if (dialog.EditedProduct is null)  // дія - видалення
                        {
                            using MySqlCommand cmd = new() { Connection = _connection };
                            cmd.CommandText = "UPDATE Products SET DeleteDt = CURRENT_TIMESTAMP WHERE Id = @id ";
                            cmd.Parameters.AddWithValue("@id", product.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                Products.Remove(product);
                                MessageBox.Show("Видалення: " + product.Name);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                        }
                        else  // дія - збереження
                        {
                            int index = Products.IndexOf(product);
                            Products.Remove(product);
                            Products.Insert(index, product);
                            MessageBox.Show("Оновлення: " + product.Name);
                        }
                    }
                    else  // вікно закрите або натиснуто Cancel
                    {
                        MessageBox.Show("Дію скасовано");
                    }
                    // MessageBox.Show(product.Name + " " + product.Price);
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            CrudProductWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.EditedProduct is not null)
                {
                    /* String sql = "INSERT INTO Products(Id, Name, Price) VALUES" +
                        $"('{dialog.EditedProduct.Id}', N'{dialog.EditedProduct.Name}', {dialog.EditedProduct.Price})";
                    using SqlCommand cmd = new(sql, _connection);
                    */
                    String sql = "INSERT INTO Products(Id, Name, Price) VALUES(@id, @name, @price)";
                    using MySqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.EditedProduct.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.EditedProduct.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.EditedProduct.Price);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Products.Add(dialog.EditedProduct);
                        MessageBox.Show("Додано: " + dialog.EditedProduct.Name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else  // вікно закрите або натиснуто Cancel
            {
                MessageBox.Show("Дію скасовано");
            }
        }
        private void SalesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Sale sale)
                {
                    CrudSaleWindow dialog = new(sale) { Owner = this };
                    if (dialog.ShowDialog() == true)  // подтвержденное действие
                    {
                        if (dialog.Sale == null)  // Удаление
                        {
                            String sql = "UPDATE Sales S SET delete_dt = CURRENT_TIMESTAMP WHERE S.Id = @id";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", sale.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Delete OK");
                                this.Sales.Remove(sale);
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Delete error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();

                        }
                        else 
                        {
                            String sql = "UPDATE Sales S SET product_id = @product_id, manager_id = @manager_id, units = @units WHERE S.id = @id;";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", dialog.Sale.Id);
                            cmd.Parameters.AddWithValue("@product_id", dialog.Sale.ProductId);
                            cmd.Parameters.AddWithValue("@manager_id", dialog.Sale.ManagerId);
                            cmd.Parameters.AddWithValue("@units", dialog.Sale.Cnt);
                            try
                            {
                                int index = this.Sales.IndexOf(sale);
                                this.Sales.Remove(sale);
                                this.Sales.Insert(index, sale);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Update OK");
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Update error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();
                        }
                    }
                    else  // окно закрыто или нажата кнопка Cancel
                    {
                        MessageBox.Show("Действие отменено");
                    }
                }
            }
        }


        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            CrudSaleWindow dialog = new(null) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.Sale is not null)
            {
                using MySqlCommand cmd = new(
                    "INSERT INTO Sales(Id, product_id, manager_id, units, sale_date) " +
                    "VALUES (@Id, @product_id, @manager_id, @units, @sale_date)",
                    _connection);

                cmd.Parameters.AddWithValue("@Id", dialog.Sale.Id);
                cmd.Parameters.AddWithValue("@product_id", dialog.Sale.ProductId);
                cmd.Parameters.AddWithValue("@manager_id", dialog.Sale.ManagerId);
                cmd.Parameters.AddWithValue("@units", dialog.Sale.Cnt);
                cmd.Parameters.AddWithValue("@sale_date", dialog.Sale.SaleDt);

                try
                {
                    cmd.ExecuteNonQuery();
                    Sales.Add(dialog.Sale);
                    MessageBox.Show("Додано успішно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка додавання: " + ex.Message);
                }
            }
        }
    }
}
