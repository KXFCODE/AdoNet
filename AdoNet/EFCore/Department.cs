using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.EFCore
{
    public class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime? DeleteDt { get; set; }

        public List<Manager> Workers { get; set; }  // Collection prop
        public List<Manager> SubWorkers { get; set; }  // Collection prop
    }
}
