using SahalinEnergyBoltStressCalculation.BTCalculation.CalculationBTClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.CalculationClass
{
    public class Calculator_GasketTargetStress: CalculateBTC
    {

        public double numOfBolts;
        public double gasketOutsideDiameter;
        public double gasketInsideDiameter;
        public double targetAssemblyGasketStress;


        private double aG;
        private double sbsel;
        private double tauGasketTargetStress;


        public double GetAG()
        {
            double n1 = Math.PI;
            double n2 = Math.Pow(gasketOutsideDiameter * 0.039, 2) - Math.Pow(gasketInsideDiameter * 0.039, 2);

            aG = n1 * n2;

            return aG;
        }

        public double GetSbsel()
        {
            double n1 = targetAssemblyGasketStress * GetAG();
            double n2 = numOfBolts * GetAs();

            sbsel = n1 / n2;

            return sbsel;
        }


        public double GetTauGasketTargetStress()
        {
            double n1 = GetSbsel() * kCoeff * GetAs() * threadMajorDiameter_D;

            tauGasketTargetStress = n1 / 12.0;

            return tauGasketTargetStress;
        }

        public double GetPercentOfYIELDStress()
        {
            double n1 = GetSbsel() / yieldStressPsi;
            double n2 = n1 * 100.0;

            return n2;
        }
    }
}
