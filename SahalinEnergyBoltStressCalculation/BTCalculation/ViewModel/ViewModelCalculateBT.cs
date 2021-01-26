using SahalinEnergyBoltStressCalculation.BTCalculation.CalculationBTClasses;
using SahalinEnergyBoltStressCalculation.BTCalculation.Model;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
using SahalinEnergyBoltStressCalculation.PageClassesFolder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.LogicClassesFolder
{
    class ViewModelCalculeteBT
    {
        // Переменная модели
        private WorkWithDataBaseBTC workWithDataBaseBTCObject = WorkWithDataBaseBTC.GetInstance();

        //Переменная страницы с первым расчётом
        public PageBoltTorqueCalculation PageCalculationBT { private get; set; }


        // Переменная со списком данных для определённых Bolt Grade и Bolt Size
        List<BoltGradeProperties> gradeData = new List<BoltGradeProperties>();
        List<BoltProperties> boltData = new List<BoltProperties>();

        // Переменные конкретного типа болта
        private BoltGradeProperties currentBoltGrade;
        private BoltProperties currentBolt;

        // Переменные, получаемые из View
        double yieldStressValueCustom;
        double yieldStressPerCent;
        double customD;
        double customE;
        double customH;
        double customK;
        double customP;

        double frictionCoeffViewModel;


        // Реализация Singleton

        private static ViewModelCalculeteBT instance;

        private ViewModelCalculeteBT() { }

        public static ViewModelCalculeteBT GetInstance()
        {
            if (instance == null)
            {
                instance = new ViewModelCalculeteBT();
            }
            return instance;
        }

        public void UpdateViewModelWithComboBoxWithGrades(string grade)
        {
            switch (grade)
            {
                case "Custom":
                    UpdateViewModelLists(grade);
                    ReturnForChangingGrade("Custom");
                    break;
                default:
                    UpdateViewModelLists(grade);
                    ReturnForChangingGrade("not custom");
                    break;
            }
        }

        public void UpdateViewModelLists(string filter)
        {
            workWithDataBaseBTCObject.UpdateModelListsForGradeChange(filter);
            gradeData = workWithDataBaseBTCObject.GetBoltGradeProperties();
            boltData = workWithDataBaseBTCObject.GetBoltProperties();
        }



        public void ReturnForChangingGrade(string customGradeOrNot)
        {
            PageCalculationBT.ChangeUiWhenGradePicked(customGradeOrNot);
        }

        public void UpdateViewModelWithComboBoxWithSizes(string size)
        {
            switch (size)
            {
                case "Custom":
                    ReturnForChangingSize(size);
                    break;
                default:
                    UpdateViewModelCurrentBolt(size);
                    ReturnForChangingSize(size);
                    break;

            }
        }

        public void UpdateViewModelCurrentBolt(string size)
        {
            workWithDataBaseBTCObject.UpdateDataBaseForSizeChange(size);
            currentBolt = workWithDataBaseBTCObject.GetCurrentBoltProperties();
            currentBoltGrade = workWithDataBaseBTCObject.GetCurrentBoltGradeProperties();
        }

        public void ReturnForChangingSize(string size)
        {
            switch (size)
            {
                case "Custom":
                    PageCalculationBT.ChangeUiWhenSizePicked("Custom");
                    break;
                default:
                    PageCalculationBT.ChangeUiWhenSizePicked("not custom");
                    break;
            }
        }

        public string[] GetArrayOfCurrentSizes()
        {
            string[] sizesArrayForItems = new string[boltData.Count];

            for (int i = 0; i < boltData.Count; i++)
            {
                sizesArrayForItems[i] = boltData[i].BoltSize;
            }

            return sizesArrayForItems;
        }

        public double[] GetBoltSizeProperties()
        {
            double currentThreadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
            double currentPitchDiameterOfThread_E = currentBolt.PitchDiameterOfThread_E;
            double currentHexSize_H = currentBolt.HexSize_H;
            double currentNutInternalChamfer_K = currentBolt.NutInternalChamfer_K;
            double currentThreadPitch_P = currentBolt.ThreadPitch_P;

            var properties = new double[] {currentThreadMajorDiameter_D, currentPitchDiameterOfThread_E, currentHexSize_H,
            currentNutInternalChamfer_K, currentThreadPitch_P};
            return properties;
        }

        public double GetCurrentYieldStress()
        {
            if (currentBoltGrade == null)
            {
                return 0.0;
            } else
            {

                return currentBoltGrade.YieldStressPsi;
            }
        }

        public void LoadDataFromDB()
        {
            workWithDataBaseBTCObject.LoadData();
        }

        public bool SetUpGrade(string statusGrade)
        {
            bool res;
            switch (statusGrade)
            {
                case "Pick bolt grade":
                    PageCalculationBT.ShowErrorMessage("BoltGrade");
                    res = false;
                    break;
                case "Custom":
                    res = SetUpYield(statusGrade);
                    break;
                default:
                    res = SetUpYield(statusGrade);
                    break;
            }
            return res;
        }

        public bool SetUpSize(string statusSize)
        {
            bool res;
            switch (statusSize)
            {
                case "Pick bolt size":
                    PageCalculationBT.ShowErrorMessage("BoltSize");
                    res = false;
                    break;
                case "Custom":
                    res = setUpProperties(statusSize);
                    break;
                default:
                    res = setUpProperties(statusSize);
                    break;
            }
            return res;
        }

        public bool setUpProperties(string statusSize)
        {
            bool checkingProp;
            double helpD;
            double helpE;
            double helpH;
            double helpK;
            double helpP;
            string[] prop = PageCalculationBT.GetProperties();
            if (Double.TryParse(prop[0], out helpD) == false || Double.TryParse(prop[1], out helpE) == false
                || Double.TryParse(prop[2], out helpH) == false
                || Double.TryParse(prop[3], out helpK) == false || Double.TryParse(prop[4], out helpP) == false)
            {
                PageCalculationBT.ShowErrorMessage("Properties");
                checkingProp = false;
            }
            else
            {
                customD = helpD;
                customE = helpE;
                customH = helpH;
                customK = helpK;
                customP = helpP;
                checkingProp = true;
            }
            return checkingProp;
        }

        public bool SetUpYield(string grade)
        {
            bool res;
            if (grade == "Custom")
            {
                double help;
                string[] yieldValues = PageCalculationBT.GetYieldStressCustom();
                if (Double.TryParse(yieldValues[0], out help) == true)
                {
                    yieldStressValueCustom = help;
                    res = true;
                } else
                {
                    PageCalculationBT.ShowErrorMessage("Yield");
                    res = false;
                }

                if (Double.TryParse(yieldValues[1], out help) == true)
                {
                    yieldStressPerCent = help;
                    res = true;
                }
                else
                {
                    PageCalculationBT.ShowErrorMessage("PerCent");
                    res = false;
                }
            } else
            {
                string[] yieldValues = PageCalculationBT.GetYieldStressCustom();
                double help;
                if (Double.TryParse(yieldValues[1], out help) == true)
                {
                    yieldStressPerCent = help;
                    res = true;
                }
                else
                {
                    PageCalculationBT.ShowErrorMessage("PerCent");
                    res = false;
                }
            }
            return res;
        }


        public bool CheckFCoeff()
        {

            bool checkFC;
            double help;
            string helpString = PageCalculationBT.GetFCoeff();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageCalculationBT.ShowErrorMessage("FCoeff");
                checkFC = false;
            }
            else if (help < 0 || help > 1)
            {
                PageCalculationBT.ShowErrorMessage("FCoeffLimits");
                checkFC = false;
            }
            else
            {
                frictionCoeffViewModel = help;
                checkFC = true;
            }
            
            return checkFC;
        }

        public void BeginCalculation(string statusGrade, string statusSize)
        {
            if (SetUpGrade(statusGrade) == false)
            {
                return;
            } else if (SetUpSize(statusSize) == false)
            {
                return;
            } else if (CheckFCoeff() == false)
            {
                return;
            } else
            {
                var obj = Calculation(statusGrade, statusSize);
                string grade = currentBoltGrade.BoltGrade;
                string size = currentBolt.BoltSize;
                PageCalculationBT.ChangeUIOnCalculation(obj, grade, size);
            }
        }

        public CalculateBTC Calculation(string statusGrade, string starusSize)
        {
            CalculateBTC objectCalculator = new CalculateBTC();

            if (statusGrade == "Custom")
            {
                objectCalculator.yieldStressPsi = yieldStressValueCustom;
                objectCalculator.perCent = yieldStressPerCent;
            } else
            {
                objectCalculator.yieldStressPsi = currentBoltGrade.YieldStressPsi;
                objectCalculator.perCent = yieldStressPerCent;
            }

            if (starusSize == "Custom")
            {
                objectCalculator.threadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
                objectCalculator.pitchDiameterOfThread_E = currentBolt.PitchDiameterOfThread_E;
                objectCalculator.hexSize_H = currentBolt.HexSize_H;
                objectCalculator.nutInternalChamfer_K = currentBolt.NutInternalChamfer_K;
                objectCalculator.threadPitch_P = currentBolt.ThreadPitch_P;

            } else
            {
                objectCalculator.threadMajorDiameter_D = customD;
                objectCalculator.pitchDiameterOfThread_E = customE;
                objectCalculator.hexSize_H = customH;
                objectCalculator.nutInternalChamfer_K = customK;
                objectCalculator.threadPitch_P = customP;
            }

            objectCalculator.fCoeff = frictionCoeffViewModel;

            return objectCalculator;
            
        }
    }
}
