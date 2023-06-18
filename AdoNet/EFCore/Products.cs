using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.EFCore
{
    public class Products
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }

        public List<Sale> Sales { get; set; }
        public List<Manager> Managers { get; set; }
    }
}
