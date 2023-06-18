using AdoNet.EFCore;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X500;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdoNet
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {

        public EFContext efContext;
        private ICollectionView DepartmentsListView;
        private static readonly Random random = new();

        public EfWindow()
        {
            InitializeComponent();
            efContext = new();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            efContext.Sales.Load();

            efContext.Departments.Load();
            DepartmentsList.ItemsSource =
                efContext.Departments.Local.ToObservableCollection();


            DepartmentsListView = CollectionViewSource
                .GetDefaultView(DepartmentsList.ItemsSource);

            DepartmentsListView.Filter =
                obj => (obj as Department)?.DeleteDt == null;

            efContext.Managers.Load();   // завантаження даних
            ManagersList.ItemsSource =
                efContext.Managers.Local.ToObservableCollection();

            UpdateMonitor();
            UpdateDailyStatistics();
        }
        private void UpdateMonitor()
        {
            MonitorBlock.Text = "Departments: " + efContext.Departments.Count();
            MonitorBlock.Text += "\nProducts: " + efContext.Products.Count();
            MonitorBlock.Text += "\nManagers: " + efContext.Managers.Count();
            MonitorBlock.Text += "\nSales: " + efContext.Sales.Count();
        }

        private void UpdateDailyStatistics()
        {
            SalesChecks.Content = "0";
            SalesCnt.Content = "0";
            StartMoment.Content = "00:00:00";
            FinishMoment.Content = "00:00:00";
            MaxCheckCnt.Content = "0";
            AvgCheckCnt.Content = "0.0";
            DeletedCheckCnt.Content = "0";

            // ------------------------------------------------------------------
            DateTime date = new DateTime(2023, 3, 15);

            var dailySales = efContext.Sales
                .Where(sale => sale.SaleDt.Date == date.Date);

            var salesCount = dailySales.Count().ToString();
            SalesChecks.Content = salesCount;

            int totalProducts = dailySales.Sum(sale => sale.Cnt);
            SalesCnt.Content = totalProducts.ToString();

            var salesWithTime = dailySales.Select(sale => new { Sale = sale, Time = sale.SaleDt.TimeOfDay });
            var minTime = salesWithTime.OrderBy(sale => sale.Time).FirstOrDefault();
            StartMoment.Content = minTime?.Time.ToString() ?? "------";

            var maxTime = salesWithTime.OrderByDescending(sale => sale.Time).FirstOrDefault();
            FinishMoment.Content = maxTime?.Time.ToString() ?? "------";

            int? maxCnt = dailySales.Max(sale => sale.Cnt);
            MaxCheckCnt.Content = maxCnt?.ToString() ?? "------";


            if (dailySales.Any())
            {
                var avgCnt = dailySales.Average(sale => sale.Cnt);
                AvgCheckCnt.Content = Math.Round(avgCnt, 2);
            }
            else
            {
                MessageBox.Show($"За этот день невозможно посчитать среднее количество товаров в чеке, потому что их: {dailySales.Count()}",  "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            

            var deletedCnt = dailySales.Where(sale => sale.DeleteDt != null).Count();
            DeletedCheckCnt.Content = deletedCnt.ToString();

            // ------------------------------------------------------------------

            // var query1 = efContext.Products.GroupJoin(efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
            //                                                                   p => p.Id,
            //                                                                   s => s.ProductId,
            //                                                                   (p, sales) => new
            //                                                                   {
            //                                                                       Name = p.Name,
            //                                                                       Cnt = sales.Count()
            //                                                                   }
            //                                                                  );

            var query = efContext.Products
                 .GroupJoin(efContext.Sales
                     .Where(
                         s => s.SaleDt.Date == DateTime.Today),
                         p => p.Id,
                         s => s.ProductId,
                         (p, sales) => new { Name = p.Name, SalesChecks = sales.Count(), Price = p.Price, SalesCnt = sales.Sum(p => p.Cnt) }
                     );

            // BestProductChecks.Content = query1.OrderByDescending(item => item.Cnt).First().Name;

            foreach (var item in query)
            {
                LogBlock.Text += $"{item.Name}: {item.SalesChecks} чеков; {item.SalesCnt} товаров; {Math.Round((item.Price * item.SalesCnt), 2)} биткоинов\n";
            }

            var bestProduct = query.OrderByDescending(p => p.SalesChecks).First();
            BestProductChecks.Content = $"{bestProduct.Name} = {bestProduct.SalesChecks} шт.";
            BestProductCount.Content = $"{bestProduct.Name} = {bestProduct.SalesCnt} шт.";
            var bestProductSum = query.OrderByDescending(p => p.SalesCnt * p.Price).First();
            BestProductSum.Content = $"{bestProductSum.Name} = {Math.Round((bestProductSum.SalesCnt * bestProductSum.Price), 2)} грн.";

            // foreach (var item in query1)
            // {
            //     LogBlock.Text += $"{item.Name} --- {item.Cnt}\n";
            // }


            // --------------------------------------------------------------------------------------- //
            #region Кращий робітник (чеки)
            var bestManager = efContext.Managers
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,            // При групуванні результатів
                    s => s.ManagerId,     // поширеним є прийом за якого
                    (m, ss) => new        // у resultSelector залишають
                    {                     // самі сутності
                        Manager = m,      // Колекції краще обробляти, бо це ітератори,
                        Cnt = ss.Count()  //  які після завершення запиту можуть не спрацювати
                    })                    // Якщо колекція потрібна, то  ss.ToList()
                .OrderByDescending(m => m.Cnt)
                .First();                 // або .Take(3) якщо потрібно 3 кращих

            BestManager.Content = bestManager.Manager.Surname + " " +
                bestManager.Manager.Name[0] + ". " +
                bestManager.Manager.Secname[0] + ". -- " +
                bestManager.Cnt;
            #endregion

            #region Три Кращі робітники (за шт) - за сумою проданих товарів
            // Варіант 1
            var topThreeManagers = efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today)
                .GroupBy(s => s.ManagerId)
                .Select(g => new {
                    Manager = efContext.Managers.Single(m => m.Id == g.Key),
                    TotalSales = g.Sum(s => s.Cnt)
                })
                .OrderByDescending(m => m.TotalSales)
                .Take(3);
            // Варіант 2
            var bestManagers = efContext.Managers
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, ss) => new
                    {
                        Manager = m,
                        Pcs = ss.Sum(s => s.Cnt)
                    })
                .OrderByDescending(m => m.Pcs)
                .Take(3);

            int n = 1;
            BestManagers.Content = "";
            foreach (var item in bestManagers)
            {
                BestManagers.Content += $"{n++} - {item.Manager?.Surname} ({item.Pcs} шт) \n";
            }
            
            #endregion

            #region Кращий робітник (грн)
            /*
            var bestHrn = efContext.Sales
                 .Where(s => s.SaleDt.Date == DateTime.Today)
                 .GroupBy(s => s.ManagerId)
                 .Select(g => new
                     {
                         ManagerName = efContext.Managers.Single(m => m.Id == g.Key).Name,
                         TotalSales = g.Sum(s => s.Cnt * efContext.Products.Single(p => p.Id == s.ProductId).Price)
                     })
                 .OrderByDescending(m => m.TotalSales)
                 .FirstOrDefault();
            // SqlException - Agregate in subquery
            BestManagerHrn.Content = $"{bestHrn?.ManagerName} ({bestHrn?.TotalSales} грн)";
            */
            /*
            var bestHrn = efContext.Managers
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, sales) => new
                    {
                        Manager = m,
                        Hrn = sales
                                .Join(
                                    efContext.Products,
                                    s => s.ProductId,
                                    p => p.Id,
                                    (s, p) => s.Cnt * p.Price )
                                .Sum()
                    })
                .OrderByDescending(m => m.Hrn)
                .First();
            // працює нормально
            */
            var bestHrn = efContext.Managers
                .GroupJoin(
                    efContext.Sales
                        .Where(s => s.SaleDt.Date == DateTime.Today)
                        .Join(efContext.Products, s => s.ProductId, p => p.Id,
                            (s, p) => new { s.ManagerId, Summ = s.Cnt * p.Price }),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, sales) => new
                    {
                        Manager = m,
                        Hrn = sales.Sum(s => s.Summ)
                    })
                .OrderByDescending(m => m.Hrn)
                .First();

            BestManagerHrn.Content = $"{bestHrn.Manager.Surname} ({bestHrn.Hrn} грн)";
            #endregion
            /*
            foreach (var item in topThreeManagers)
            {
                LogBlock.Text += $"{n++} - {item.Manager?.Surname} ({item.TotalSales} шт) \n";
            }*/

            var today = DateTime.Today;
            var departmentsStat = efContext.Managers
                .Where(m => m.Id_main_dep != null)
                .GroupJoin(efContext.Sales.Where(s => s.SaleDt.Date == today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, s) => new
                    {
                        DepId = m.Id_main_dep,
                        GrnSum = s.Sum(x => x.Cnt * x.Product.Price),
                        ProductsCount = s.Sum(x => x.Cnt),
                        ChecksCount = s.Count()
                    })
                .Join(efContext.Departments,
                    m => m.DepId,
                    d => d.Id,
                    (m, d) => new
                    {
                        Dep_name = d.Name,
                        HrnSumm = m.GrnSum,
                        Products_cnt = m.ProductsCount,
                        Checks_cnt = m.ChecksCount
                    })
                .GroupBy(d => d.Dep_name)
                .Select(n => new
                {
                    Dep_name = n.Key,
                    HrnSum = Math.Round(n.Sum(x => x.HrnSumm), 2),
                    Checks_cnt = n.Sum(x => x.Checks_cnt),
                    Products_cnt = n.Sum(x => x.Products_cnt)
                })
                .OrderByDescending(d => d.HrnSum);

            DepartmentsStat.ItemsSource = departmentsStat.ToList();
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is EFCore.Department department)
                {
                    var edit = new EditView.EFEditDepartmentWindow(department);
                    edit.Owner = this;
                    edit.DataContext = this;
                    edit.ShowDialog();
                    efContext.SaveChanges();
                }
            }
        }
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                efContext.Departments.Add(
                    new Department()
                    {
                        Id = dialog.EditedDepartment.Id,
                        Name = dialog.EditedDepartment.Name
                    });



                efContext.SaveChanges();

                MonitorBlock.Text += "\nDepartments: " + efContext.Departments.Count();
            }
        }
        private bool HideDeletedDepartmentsFilter(object item)
        {
            if (item is Department department)
            {
                return department.DeleteDt is null;
            }
            return false;
        }
        private void ShowDeletedDepartmentsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = null;
            ((GridView)DepartmentsList.View).Columns[2].Width = Double.NaN;
        }
        private void ShowDeletedDepartmentsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = HideDeletedDepartmentsFilter;
            ((GridView)DepartmentsList.View).Columns[2].Width = 0;
        }
        private void AddSalesButton_Click(object sender, RoutedEventArgs e)
        {
            int manCnt = efContext.Managers.Count();
            int proCnt = efContext.Products.Count();
            double maxPrice = efContext.Products.Max(p => p.Price);

            for (int i = 0; i < 100; i++)
            {
                int randIndex = random.Next(manCnt);
                Manager manager = efContext.Managers.Skip(randIndex).First();
                randIndex = random.Next(proCnt);
                Products product = efContext.Products.Skip(randIndex).First();
                DateTime moment = DateTime.Today.AddHours(8).AddSeconds(random.Next(43200)).AddDays(-random.Next(2));
                int cntLimit = (int)(100 * (1 - product.Price / maxPrice) + 2);
                int cnt = random.Next(1, cntLimit);
                DateTime? deleteDt = random.Next(50) == 0 ? moment.AddHours(random.Next(1, 48)) : null;

                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Cnt = cnt,
                    SaleDt = moment,
                    DeleteDt = deleteDt
                });
            }
            efContext.SaveChanges();
            UpdateMonitor();
            UpdateDailyStatistics();
        }
    }
}
