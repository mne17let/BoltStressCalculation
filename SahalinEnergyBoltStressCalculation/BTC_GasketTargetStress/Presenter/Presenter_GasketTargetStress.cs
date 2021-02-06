using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.Presenter
{
    class Presenter_GasketTargetStress
    {

        // Реализация Singleton
        private static Presenter_GasketTargetStress instanse;

        private Presenter_GasketTargetStress() { }

        public static Presenter_GasketTargetStress GetInstance()
        {
            if (instanse == null)
            {
                instanse = new Presenter_GasketTargetStress();
            }
            return instanse;
        }



    }
}
