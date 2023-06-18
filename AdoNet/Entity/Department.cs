using AdoNet.DAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.Entity
{
    public class Department
    {
        public Guid Id { get; set; }        
        public String Name { get; set; }   
        public DateTime? DeleteDt { get; set; }
        public Department()
        {
            Id = Guid.NewGuid();
            Name = null!;
        }
        public Department(DbDataReader reader)
        {
            Id = reader.GetGuid(0);
            Name = reader.GetString(1);
            DeleteDt = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
        }


        internal DataContext? _dataContext { get; set; } // зависимость - ссылка на контекст данных
        public List<Entity.Manager>? MainManagers
        {
            get => _dataContext?
                .Managers.
                GetAll().
                FindAll(m => m.Id_main_dep == this.Id);
        }

        public List<Entity.Manager>? SecManagers
        {
            get => _dataContext?
                .Managers.
                GetAll().
                FindAll(m => m.Id_sec_dep == this.Id);
        }
    }
}
