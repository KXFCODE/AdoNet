using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AdoNet.Entity
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ManagerId { get; set; }
        public Int32 Cnt { get; set; }
        public DateTime SaleDt { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Sale()
        {
            Id = Guid.NewGuid();
            Cnt = 1;
            SaleDt = DateTime.Now;
        }
        public Sale(MySqlDataReader reader)
        {
            Id = reader.GetGuid("Id"); 
            ProductId = reader.GetGuid("product_id"); 
            ManagerId = reader.GetGuid("manager_id");
            Cnt = reader.GetInt32("units");
            SaleDt = Convert.ToDateTime(reader["sale_date"]);
            DeleteDt = reader.IsDBNull("delete_dt") ? null : reader.GetDateTime("delete_dt");
        }
    }
}
