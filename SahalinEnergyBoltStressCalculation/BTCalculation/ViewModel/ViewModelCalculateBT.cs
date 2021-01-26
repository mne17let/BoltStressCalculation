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

        public void BeginCalculate(string statusGrade, string statusSize)
        {
            switch (statusGrade)
            {
                case "Pick bolt grade":
                    PageCalculationBT.ShowErrorMessage("BoltGrade");
                    break;
                case "Custom":
                    ChooseWhenCustomGrade(statusSize);
                    break;
                default:
                    ChooseWhenDefaultGrade(statusSize, statusGrade);
                    break;
            }
        }

        public void ChooseWhenDefaultGrade(string statusSize, string statusGrade)
        {
            switch (statusSize)
            {
                case "Pick bolt size":
                    PageCalculationBT.ShowErrorMessage("BoltSize");
                    break;
                case "Custom":
                    if (CheckProperties() == false)
                    {
                        return;
                    }
                    else
                    {
                        WorkWithCalculationCustomSize();
                    }
                    break;
                default:
                    if (CheckYieldFields() == false)
                    {
                        return;
                    } else
                    {
                        WorkWithCalculationDefaultGradeSize(statusSize, statusGrade);
                    }
                    
                    break;
            }
        }

        public void ChooseWhenCustomGrade(string statusSize)
        {
            if (CheckYieldFields() == false)
            {
                return;
            }
            else
            {
                switch (statusSize)
                {
                    case "Pick bolt size":
                        PageCalculationBT.ShowErrorMessage("BoltSize");
                        break;
                    case "Custom":
                        if (CheckProperties() == false)
                        {
                            return;
                        }
                        else
                        {
                            WorkWithCalculationCustomGradeCustomSize();
                        }
                        break;
                    default:
                        WorkWithCalculationCustomGrade();
                        break;
                }
            }
        }

        public void WorkWithCalculationCustomGrade()
        {
            if (CheckFCoeff() == false)
            {
                return;
            } else
            {
                PageCalculationBT.ShowResult("Custom Grade, Def Size");
            }
        }

        public void WorkWithCalculationCustomGradeCustomSize()
        {
            if (CheckFCoeff() == false)
            {
                return;
            }
            else
            {
                PageCalculationBT.ShowResult("Custom Grade, Custom Size");
            }
                
        }

        public void WorkWithCalculationCustomSize()
        {
            if (CheckFCoeff() == false)
            {
                return;
            }
            else
            {
                PageCalculationBT.ShowResult("Def Grade, Custom Size");
            }
                
        }

        public void WorkWithCalculationDefaultGradeSize(string size, string grade)
        {
            if (CheckFCoeff() == false)
            {
                return;
            }
            else
            {
                PageCalculationBT.ShowResult("Def Grade, Def Size");
                PageCalculationBT.ShowInfoMessage(size, grade);
            }
                
        }

        public bool CheckYieldFields()
        {
            bool checkingYield;
            double[] yieldStrings = PageCalculationBT.GetYieldStressCustom();
            if (yieldStrings[0] == 0 || yieldStrings[1] == 0)
            {
                PageCalculationBT.ShowErrorMessage("Yield");
                checkingYield = false;
            } else
            {
                checkingYield = true;
            }
            return checkingYield;
        }

        public bool CheckProperties()
        {
            bool checkingProp;
            double[] prop = PageCalculationBT.GetProperties();
            if (prop[0] == 0 || prop[1] == 0 || prop[2] == 0 || prop[3] == 0 || prop[4] == 0)
            {
                PageCalculationBT.ShowErrorMessage("Properties");
                checkingProp = false;
            } else
            {
                checkingProp = true;
            }
            return checkingProp;
        }

        public bool CheckFCoeff()
        {
            bool checkFC;
            double fCoeff = PageCalculationBT.GetFCoeff();
            if (fCoeff == 0)
            {
                PageCalculationBT.ShowErrorMessage("FCoeff");
                checkFC = false;
            } else
            {
                checkFC = true;
            }
            return checkFC;
        }
    }
}
