using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTCalculation.CalculationBTClasses
{
    public class CalculateBTC
    {
        public double perCent;
        public double boltDiameter;
        public double yieldStressPsi;
        public double threadMajorDiameter_D;
        public double pitchDiameterOfThread_E;
        public double hexSize_H;
        public double nutInternalChamfer_K;
        public double threadPitch_P;
        public double numberOfThreadsPerInch;
        public double nutWidth;
        public double fCoeff;
        private double sigma;
        private double aS;
        private double f;
        private double tau;

        public CalculateBTC() { }
        public double GetF()
        {
            double sigmaLocal = GetSigma();
            double aSLocal = GetAs();

            f = sigmaLocal * aSLocal;
            return f;
        }

        public double GetAs()
        {
            double n1 = 0.9743 * threadPitch_P;
            double n2 = threadMajorDiameter_D - n1;
            double n3 = Math.Pow(n2, 2.0);
            double n4 = Math.PI/ 4.0;

            aS = n3 * n4;
            return aS;
        }

        

        public double GetSigma()
        {
            double turnFromPerCent = perCent / 100.0;
            sigma = yieldStressPsi * turnFromPerCent;
            return sigma;
        }

        public double GetTau()
        {
            double n1_1 = Math.PI * fCoeff * pitchDiameterOfThread_E;
            double n1_2 = Math.Cos(Math.PI / 6.0);
            double n1_3 = (n1_1 / n1_2) + threadPitch_P;
            double n1_4 = GetF() * pitchDiameterOfThread_E * n1_3;


            double n2_1 = threadPitch_P * fCoeff;
            double n2_2 = Math.Cos(Math.PI / 6.0);
            double n2_3 = n2_1 / n2_2;
            double n2_4 = Math.PI * pitchDiameterOfThread_E;
            double n2_5 = n2_4 - n2_3;
            double n2_6 = 2 * n2_5;


            double n3_1 = hexSize_H + threadMajorDiameter_D + nutInternalChamfer_K;
            double n3_2 = n3_1 / 4.0;
            double n3_3 = GetF() * fCoeff * n3_2;

            double res1 = n1_4 / n2_6;
            double res2 = res1 + n3_3;

            tau = res2 / 12.0;

            return tau;
        }
    }
}
