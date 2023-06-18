using AdoNet.DAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.Entity
{
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
        public Manager()
        {
            Id = Guid.NewGuid();
            Name = null!;
            Surname = null!;
            Secname = null!;
            FiredDt = null!;
        }
        public Manager(DbDataReader reader)
        {
            Id = reader.GetGuid(0);
            Name = reader.GetString(1);
            Surname = reader.GetString(2);
            Secname = reader.GetString(3);
            Id_main_dep = reader.GetGuid(4);
            Id_sec_dep = reader.GetValue(5) == DBNull.Value ? null : reader.GetGuid(5);
            Id_chief = reader.IsDBNull(6) ? null : reader.GetGuid(6);
            FiredDt = reader.IsDBNull(7) ? null : reader.GetDateTime(7);
        }

        internal DataContext? _dataContext { get; set; } // зависимость - ссылка на контекст данных
        public Department? MainDep  // навигационное свойство
        {
            get
            {
                return _dataContext?
                    .Departments
                    .GetById(this.Id_main_dep);
            }
        }

        public Department? SecDep
        {
            get
            {
                return _dataContext?
                    .Departments
                    .GetById(this.Id_sec_dep);
            }
        }

        public Manager? Chief
        {
            get
            {
                return _dataContext?
                    .Managers
                    .GetAll()
                    .Find(m => m.Id == this.Id_chief);
            }
        }
    }
}
