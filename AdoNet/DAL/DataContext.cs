using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.DAL
{
    internal class DataContext
    {
        internal DepartmentApi Departments;
        internal ManagerApi Managers;
        private MySqlConnection _connection;
        public DataContext()
        {
            _connection = new(App.ConnectionString);
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                App.Logger.Log(ex.Message, "SEVERE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
                throw new Exception("Data context init error. See logs for details");
            }
            Departments = new DepartmentApi(_connection, this);
            Managers = new ManagerApi(_connection, this);

        }
    }
}
