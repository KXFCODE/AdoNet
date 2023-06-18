using AdoNet.Entity;
using AdoNet.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdoNet.DAL
{
    internal class DepartmentApi
    {
        private readonly MySqlConnection _connection;
        private readonly ILogger _logger;
        private List<Department> departments;
        private readonly DataContext _context;
        public DepartmentApi(MySqlConnection connection, DataContext context)
        {
            _connection = connection;
            _logger = App.Logger;
            _context = context;
            departments = null!; // Lazy - коллекция будет построена с первым запросом
        }

        /// <summary>
        /// Returns list of Departments from DB
        /// </summary>
        /// <param name="forceReload">Defines use of cached list or to reload DB</param>
        /// <returns></returns>
        public List<Department> GetAll(bool forceReload = false)
        {
            if (this.departments is not null && forceReload == false)   // Если коллекция создавалась ранее, то возвращаем ее
            {
                return this.departments;
            }
            departments = new();
            try
            {
                using MySqlCommand command = new("SELECT D.* FROM Departments D", _connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new(reader) { _dataContext = _context });
                }
                reader.Close();
            }
            catch (Exception ex)    // Logging - правильный способ обработки исключений
            {
                _logger.Log(ex.Message, "SEVERE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
            }
            return departments;
        }

        public Department? GetById(Guid? id)
        {
            if (departments == null)
            {
                return GetAll().Find(dep => dep.Id == id);
            }
            return departments.Find(dep => dep.Id == id);
        }

        public void Create(Department department)
        {
            try
            {
                using MySqlCommand command = new("INSERT INTO Departments (Id, Name) VALUES (@id, @name)", _connection);
                command.Parameters.AddWithValue("@id", department.Id);
                command.Parameters.AddWithValue("@name", department.Name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "SERVE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
                MessageBox.Show("Create Error. See logs for more info");
            }
        }
        public void Update(Department department)
        {
            try
            {
                using MySqlCommand command = new("UPDATE Departments SET Name = @name WHERE Id = @id", _connection);
                command.Parameters.AddWithValue("@id", department.Id);
                command.Parameters.AddWithValue("@name", department.Name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "SERVE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
                MessageBox.Show("Update Error. See logs for more info");
            }
        }
        public void Delete(Department department)
        {
            try
            {
                using MySqlCommand command = new("UPDATE Departments SET DeleteDt = CURRENT_TIMESTAMP WHERE Id = @id", _connection);
                command.Parameters.AddWithValue("@id", department.Id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "SERVE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
                MessageBox.Show("Delete Error. See logs for more info");
            }
        }
    }
}
