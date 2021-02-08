using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.CalculationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.CalculationClass
{
    class Calculator_PressureAndGasketType: Calculator_GasketTargetStress
    {
        public double internalDesignPressurePresenter;
        public double gasketFactorPresenter;
        public double minimumDesignSeatingStressPresenter;
        public double basicGasketSeatingWidthPresenter;
        public double gasketWidthPresenter;

        private double wM_1;
        private double wM_2;
        private double singleBoltLoad_Newton;
        private double singleBoltLoad_Lbf;

    }
}
