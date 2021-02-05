using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne
{
    class BoltGradeProperties
    {
        // Класс-модель болта из таблицы с bolt grade

        [Key]
        public int Id { get; set; }
        public string Auxiliary { get; set; }
        public string BoltGrade { get; set; }
        public double BoltDiameter { get; set; }
        public double YieldStressPsi { get; set; }

        public BoltGradeProperties() { }

        public BoltGradeProperties(int i, string a, string bG, double bD, double ySP)
        {
            Id = i;
            Auxiliary = a;
            BoltGrade = bG;
            BoltDiameter = bD;
            YieldStressPsi = ySP;
        }
    }
}
