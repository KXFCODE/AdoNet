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

namespace AdoNet.EditView
{
    /// <summary>
    /// Interaction logic for EFEditDepartmentWindow.xaml
    /// </summary>
    public partial class EFEditDepartmentWindow : Window
    {
        EFCore.Department EditedDepartment;
        public EFEditDepartmentWindow(EFCore.Department department)
        {
            InitializeComponent();
            EditedDepartment = department;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Owner as EfWindow;
            try
            {
                owner.efContext.Departments.Where(x => x.Id == EditedDepartment.Id).First().Name = ViewName.Text;
            }
            catch
            {
                EditedDepartment.Name = ViewName.Text;
                owner.efContext.Departments.Add(EditedDepartment);
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Owner as EfWindow;
            // owner.efContext.Departments.Remove(EditedDepartment);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            (Owner as EfWindow).efContext.Departments.Where(x => x.Id == EditedDepartment.Id).First().DeleteDt = DateTime.Now;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewName.Text = EditedDepartment.Name;
            ViewId.Text = EditedDepartment.Id.ToString();
        }
    }
}
