using AdoNet.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdoNet.Services;
using MySql.Data.MySqlClient;

namespace AdoNet.DAL
{
    internal class ManagerApi
    {
        private readonly MySqlConnection _connection;
        private readonly ILogger _logger;
        private List<Manager> managers;
        private readonly DataContext _context;


        public ManagerApi(MySqlConnection connection, DataContext context)
        {
            _connection = connection;
            _logger = App.Logger;
            _context = context;
            managers = null!;
        }

        public List<Entity.Manager> GetAll(bool deleted = false, bool forceReload = false)
        {
            if (this.managers is not null && forceReload == false) {
                return this.managers;
            }
            var list = new List<Manager>();
            String sql = "SELECT M.* FROM Managers M" +
                (deleted ? "" : " WHERE M.FiredDt IS NULL");
            try
            {
                using MySqlCommand cmd = new(sql, _connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new(reader) { _dataContext = _context });
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, "SEVERE",
                    this.GetType().Name,
                    MethodInfo.GetCurrentMethod()?.Name ?? "");
            }
            return list;
        }
    }
}
