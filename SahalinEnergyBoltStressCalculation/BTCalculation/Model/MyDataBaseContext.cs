using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;

namespace SahalinEnergyBoltStressCalculation
{
    class MyDataBaseContext: DbContext 
    {
        // Конструктор родительского класса. Внутрь передаю имя подключения имя подключаения
        public MyDataBaseContext():base("DefaultConnection")
        {
        }

        // Буду забирать данные из базы данных, но сохранять в неё ничего не буду, так как она неизменна
        public DbSet<BoltProperties> BoltProperties { get; set; }

        public DbSet<BoltGradeProperties> BoltGradeProperties { get; set; }
    }
}
