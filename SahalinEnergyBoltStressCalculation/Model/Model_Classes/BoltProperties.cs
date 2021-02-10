using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SahalinEnergyBoltStressCalculation
{
    class BoltProperties
    {

        // Класс-модель болта из таблицы с bolt size


        [Key]
        public string BoltSize {get; set;}

        public double ThreadMajorDiameter_D { get; set; }
        public double PitchDiameterOfThread_E { get; set; }
        public double HexSize_H { get; set; }
        public double NutInternalChamfer_K { get; set; }
        public double ThreadPitch_P { get; set; }

        public BoltProperties() { }

        public BoltProperties(string bs, double d, double e, double h, double k, double p)
        {
            BoltSize = bs;
            ThreadMajorDiameter_D = d;
            PitchDiameterOfThread_E = e;
            HexSize_H = h;
            NutInternalChamfer_K = k;
            ThreadPitch_P = p;
        }
    }
}
