using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.Entity
{
    public class Products
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Products()
        {
            Id = Guid.NewGuid();
            Name = null!;
            DeleteDt = null;
        }
        public Products(MySqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Name = reader.GetString("Name");
            Price = reader.GetDouble("Price");
            DeleteDt = reader.IsDBNull(reader.GetOrdinal("DeleteDt"))
                        ? null
                        : reader.GetDateTime("DeleteDt");
        }
    }
}
