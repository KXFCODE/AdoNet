using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.EFCore
{
    [Table("Manager")]
    public class Manager
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Secname { get; set; }
        public Guid Id_main_dep { get; set; }
        public Guid? Id_sec_dep { get; set; }
        public Guid? Id_chief { get; set; }
        public DateTime? FiredDt { get; set; }


        public Department MainDep { get; set; }  // Reference prop
        public Department SecDep { get; set; }  // Reference prop
        public List<Sale> Sales { get; set; }
        public List<Products> Products { get; set; }
    }
}
