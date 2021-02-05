using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;

namespace SahalinEnergyBoltStressCalculation
{
    class MyDataBaseContext : DbContext
    {
        // Конструктор родительского класса. Внутрь передаю имя подключения
        public MyDataBaseContext() : base("DefaultConnection")
        {
        }

        // Буду забирать данные из базы данных, но сохранять в неё ничего не буду, так как она неизменна

        // Переменные коллекций, куда собираются данные из базы данных
        public DbSet<BoltProperties> BoltProperties { get; set; }

        public DbSet<BoltGradeProperties> BoltGradeProperties { get; set; }

    }

}
