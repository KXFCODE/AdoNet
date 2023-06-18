using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Interaction logic for CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        private MySqlConnection _connection;
        public Entity.Department EditedDepartment { get; private set; }


        public CrudDepartmentWindow(Entity.Department department)
        {
            InitializeComponent();
            this.EditedDepartment = department;
            _connection = new();
            _connection.ConnectionString = App.ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EditedDepartment != null)
            {
                ViewName.Text = EditedDepartment.Name;
            }
            else
            {
                EditedDepartment = new();
                ViewId.Text = string.Empty;
                ViewName.Text = string.Empty;
                DeleteButton.IsEnabled = false;
            }
            ViewId.Text = EditedDepartment.Id.ToString();
            try
            {
                _connection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewName.Text == String.Empty)
            {
                MessageBox.Show("Введите, пожалуйста, название отдела");
                return;
            }
            if (ViewId.Text == String.Empty)
            {
                using MySqlCommand cmd = new($"SELECT COUNT(*) FROM Departments WHERE Name = @name", _connection);
                cmd.Parameters.AddWithValue("@name", ViewName.Text);
                try
                {
                    object res = cmd.ExecuteScalar();


                    int cnt = Convert.ToInt32(res);
                    if (cnt > 0)
                    {
                        MessageBox.Show("Отдел с таким названием уже существует");
                        return;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "SQL error",
                        MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cast error",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                cmd.Dispose();
                cmd.Parameters.Clear();
                cmd.CommandText =
                 $@"INSERT INTO Departments 
                        ( Id, Name )
                  VALUES 
                    ( @id, @name )";
                cmd.Parameters.AddWithValue("@name", ViewName.Text);
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Create OK");

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Create error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop);
                }
                cmd.Dispose();
            }
            else
            {
                if (ViewName.Text == EditedDepartment.Name)
                {
                    MessageBox.Show("Нет изменений");
                    return;
                }
                String sql = $"UPDATE Departments SET Name = @name WHERE Id = @id";
                using MySqlCommand cmd = new(sql, _connection);
                cmd.Parameters.AddWithValue("@name", ViewName.Text);
                cmd.Parameters.AddWithValue("@id", EditedDepartment.Id);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update OK");
                    this.EditedDepartment.Name = ViewName.Text;
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
            this.DialogResult = true; // результат ShowDialog() = true и закрывается окно
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            String sql = $"DELETE FROM Departments WHERE Id = @id";
            using MySqlCommand cmd = new(sql, _connection);
            cmd.Parameters.AddWithValue("@id", EditedDepartment.Id);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Delete OK");
                this.EditedDepartment.Name = ViewName.Text;
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
            this.EditedDepartment = null;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
