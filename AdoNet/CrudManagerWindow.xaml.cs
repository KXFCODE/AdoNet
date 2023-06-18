using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CrudManagerWindow.xaml
    /// </summary>
    public partial class CrudManagerWindow : Window
    {
        public Entity.Manager? EditedManager;
        private readonly MySqlConnection _connection;
        public CrudManagerWindow(Entity.Manager? EditedManager)
        {
            InitializeComponent();
            this.EditedManager = EditedManager;
            _connection = new(App.ConnectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
            
            DataContext = Owner;
            if (EditedManager is null)
            {
                EditedManager = new();  // id создается в конструкторе
                DeleteButton.IsEnabled = false;
            }
            else
            {
                ViewSurname.Text = EditedManager.Surname;
                ViewName.Text = EditedManager.Name;
                ViewSecname.Text = EditedManager.Secname;
                if (Owner is OrmWindow owner)
                {
                    MainDepCombobox.SelectedItem = owner.Departments.FirstOrDefault(dep => dep.Id == EditedManager.Id_main_dep);
                    SecDepCombobox.SelectedItem = owner.Departments.FirstOrDefault(dep => dep.Id == EditedManager.Id_sec_dep);
                    ChiefCombobox.SelectedItem = owner.Managers.FirstOrDefault(man => man.Id == EditedManager.Id_chief);
                }
                else
                {
                    MessageBox.Show("Owner is not OrmWindow");
                }
            }
            ViewId.Text = EditedManager.Id.ToString();

            if (EditedManager.Id_sec_dep == null) { RemoveSecDepBtn.Visibility = Visibility.Hidden; };
            if (EditedManager.Id_chief == null) { RemoveChiefBtn.Visibility = Visibility.Hidden; };
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using MySqlConnection connection = new (App.ConnectionString);
            connection.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE Managers SET FiredDt = CURRENT_TIMESTAMP WHERE Id = @Id", _connection);
            cmd.Parameters.AddWithValue("@Id", EditedManager.Id);
            cmd.ExecuteNonQuery();
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RemoveSecDepButton_Click(object sender, RoutedEventArgs e)
        {
            EditedManager.Id_sec_dep = null;
            SecDepCombobox.SelectedItem = null;
            RemoveSecDepBtn.Visibility = Visibility.Hidden;
        }

        private void RemoveChiefButton_Click(object sender, RoutedEventArgs e)
        {
            EditedManager.Id_chief = null;
            ChiefCombobox.SelectedItem = null;
            RemoveChiefBtn.Visibility = Visibility.Hidden;
        }


        private void SecDepCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveSecDepBtn.Visibility = Visibility.Visible;
        }

        private void ChiefCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveChiefBtn.Visibility = Visibility.Visible;
        }
    }
}
