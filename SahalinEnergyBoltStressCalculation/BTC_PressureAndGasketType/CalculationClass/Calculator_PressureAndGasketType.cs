using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.CalculationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.CalculationClass
{
    public class Calculator_PressureAndGasketType: Calculator_GasketTargetStress
    {
        public double internalDesignPressure;
        public double gasketFactor_m;
        public double minimumDesignSeatingStress_y;
        public double basicGasketSeatingWidth_b0;
        public double gasketWidth_N;

        private double wM_1;
        private double wM_2;
        private double singleBoltLoad_Newton;
        private double singleBoltLoad_Lbf;

        private double g_3calc;
        private double b_3calc;



        private double GetB_3calc()
        {
            if (basicGasketSeatingWidth_b0 <= 6.0)
            {
                b_3calc = basicGasketSeatingWidth_b0;
            } else
            {
                b_3calc = 2.5 * Math.Sqrt(basicGasketSeatingWidth_b0);
            }

            return b_3calc;
        }


        private double GetG_3calc()
        {

            if (basicGasketSeatingWidth_b0 <= 6.0)
            {
                g_3calc = (gasketInsideDiameter + gasketOutsideDiameter) / 2.0;
            } else
            {
                double n = 2 * GetB_3calc();
                g_3calc = gasketOutsideDiameter - n;
            }

            return g_3calc;
        }


        public double Get_Wm1()
        {
            double n1_1 = Math.Pow(GetG_3calc(), 2.0);
            double n1_2 = n1_1 * 0.785 * internalDesignPressure;


            double n2_1 = 2 * GetB_3calc();
            double n2_2 = 3.14 * GetG_3calc() * gasketFactor_m * internalDesignPressure;
            double n2_3 = n2_1 * n2_2;

            wM_1 = n1_2 + n2_3;

            return wM_1;
        }

        public double Get_Wm2()
        {
            wM_2 = 3.14 * GetB_3calc() * GetG_3calc() * minimumDesignSeatingStress_y;

            return wM_2;
        }


        public double GetSingleBoltLoad_Newton()
        {
            if (Get_Wm1() >= Get_Wm2())
            {
                singleBoltLoad_Newton = Get_Wm1() / numOfBolts;
            } else
            {
                singleBoltLoad_Newton = Get_Wm2() / numOfBolts;
            }

            return singleBoltLoad_Newton;
        }


        public double GetSingleBoltLoad_Lbf()
        {
            singleBoltLoad_Lbf = GetSingleBoltLoad_Newton() * 0.224809;

            return singleBoltLoad_Lbf;
        }

        public double GetBoltStressRequired_3calc()
        {
            double bSR = GetSingleBoltLoad_Lbf() / GetAs();

            return bSR;
        }

        public double GetPerCentOfYIELDStress_3calc()
        {
            double n1 = GetBoltStressRequired_3calc() / yieldStressPsi;
            double perCent = n1 * 100;

            return perCent;
        }


        public override double GetF()
        {
            return GetSingleBoltLoad_Lbf();
        }

    }
}
