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
    class ViewModelCalculateBT
    {
        // Реализация Singleton

        private static ViewModelCalculateBT instance;

        private ViewModelCalculateBT() { }

        public static ViewModelCalculateBT GetInstance()
        {
            if (instance == null)
            {
                instance = new ViewModelCalculateBT();
            }
            return instance;
        }

        // Переменная модели
        private WorkWithDataBaseBTC workWithDataBaseBTCObject = WorkWithDataBaseBTC.GetInstance();

        //Переменная страницы с первым расчётом
        public PageBoltTorqueCalculation PageCalculationBT { private get; set; }


        // Переменные со списком данных для определённых Bolt Grade и Bolt Size
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
        double customNOTPI;
        double customNutWidth;

        double frictionCoeffViewModel;
        double kCoeffViewModel;



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Обработка выбора bolt grade

        // Выбран Bolt Grade в ComboBox
        // Обновление списков ViewModel с объектами болтов из таблиц BoltSize и BoltGrade и вызов методов у View с передачей туда параметров
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

        // Обновление списков ViewModel с объектами болтов из таблиц BoltSize и BoltGrade
        private void UpdateViewModelLists(string filter)
        {
            workWithDataBaseBTCObject.UpdateModelListsForGradeChange(filter);
            gradeData = workWithDataBaseBTCObject.GetBoltGradeProperties();
            boltData = workWithDataBaseBTCObject.GetBoltProperties();
        }

        // Вызов метода реакции у View на выбор Bolt Grade и передача в этот метод информации, выбран ли "Custom" Bolt Grade
        private void ReturnForChangingGrade(string customGradeOrNot)
        {
            PageCalculationBT.ChangeUiWhenGradePicked(customGradeOrNot);
        }

        // Возврат массива со значениями размеров (bolt size) в зависимости от выбранного bolt grade
        public string[] GetArrayOfCurrentSizes()
        {
            string[] sizesArrayForItems = new string[boltData.Count];

            for (int i = 0; i < boltData.Count; i++)
            {
                sizesArrayForItems[i] = boltData[i].BoltSize;
            }

            return sizesArrayForItems;
        }





        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Обработка выбора bolt size

        // Выбран BoltSize
        // Обновление ViewModel в зависимости от выбранного размера болта и вызов методов реакции у View
        public void UpdateViewModelWithComboBoxWithSizes(string size, string grade)
        {
            switch (size)
            {
                case "Custom":
                    ReturnForChangingSize(size);
                    break;
                default:
                    UpdateViewModelCurrentBolt(size, grade);
                    ReturnForChangingSize(size);
                    break;

            }
        }

        // Присвоение переменным-объектам конкретного болта из таблиц BoltSize и BoltGrade
        private void UpdateViewModelCurrentBolt(string size, string grade)
        {
            workWithDataBaseBTCObject.UpdateDataBaseForSizeChange(size, grade);
            currentBolt = workWithDataBaseBTCObject.GetCurrentBoltProperties();
            currentBoltGrade = workWithDataBaseBTCObject.GetCurrentBoltGradeProperties();
        }

        // Вызов методов у View в качествер реакции на выбор bolt size
        private void ReturnForChangingSize(string size)
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

        // Выбран bolt size
        // Возврат характеристик конкретного болта, выбранного пользователем
        public double[] GetBoltSizeProperties()
        {
            double currentThreadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
            double currentPitchDiameterOfThread_E = currentBolt.PitchDiameterOfThread_E;
            double currentHexSize_H = currentBolt.HexSize_H;
            double currentNutInternalChamfer_K = currentBolt.NutInternalChamfer_K;
            double currentThreadPitch_P = currentBolt.ThreadPitch_P;
            double currentNumberOfThreadsPerInch_NOTPI = currentBolt.NumberOfThreadsPerInch;
            double currentNutWidth = currentBolt.NutWidth;

            var properties = new double[] {currentThreadMajorDiameter_D, currentPitchDiameterOfThread_E, currentHexSize_H,
            currentNutInternalChamfer_K, currentThreadPitch_P, currentNumberOfThreadsPerInch_NOTPI, currentNutWidth};
            return properties;
        }

        // Выбран bolt size и bolt grade (оба - не custom)
        // Возврат конкретного значения YieldStressPsi для выбранного пользователем болта 
        public double GetCurrentYieldStress()
        {
            if (currentBoltGrade == null)
            {
                return 0.0;
            }
            else
            {

                return currentBoltGrade.YieldStressPsi;
            }
        }





        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Обработка нажатия кнопки "посчитать"

        // Метод-организатор расчёта в Presenter
        public void BeginCalculation(string statusGrade, string statusSize)
        {
            if (SetUpGrade(statusGrade) == false)
            {
                // Bolt Grade не выбран
                return;
            }
            else if (SetUpSize(statusSize) == false)
            {
                // Размер болта не выбран
                return;
            }
            else if (CheckFCoeff() == false)
            {
                // Коэффициент трения не введён
                return;
            }
            else if (CheckKCoeff() == false)
            {
                // Коэффициент К не введён
                return;
            }
            else
            {
                // Всё в порядке. Создаём необходимые объекты калькулятора, а также размера и grade болта
                // Передаём всё в метод View и вызываем у него в качествер реакциии на нажатие кнопки "Calculate"
                var objectCalculator = CreateCalculator(statusGrade, statusSize);
                string grade;
                string size;

                if (statusGrade == "Custom")
                {
                    grade = "Custom";
                }
                else
                {
                    grade = currentBoltGrade.BoltGrade;
                }

                if (statusSize == "Custom")
                {
                    size = "Custom";
                }
                else
                {
                    size = currentBolt.BoltSize;
                }

                PageCalculationBT.ChangeUIOnCalculation(objectCalculator, grade, size);
            }
        }

        // Проверка, выбран ли bolt grade, вызов методов проверки ввода данных, которые от него зависят
        private bool SetUpGrade(string statusGrade)
        {
            bool res;
            switch (statusGrade)
            {
                // Проверка, введён ли Yield Stress и % от Yield Stress в случае, если bolt grade выбран "Custom"
                case "Pick bolt grade":
                    PageCalculationBT.ShowErrorMessage("BoltGrade");
                    res = false;
                    break;
                case "Custom":
                    // Проверка, введён ли Yield Stress и % от Yield Stress в случае, если bolt grade выбран "Custom"
                    res = SetUpYield(statusGrade);
                    break;
                default:
                    // Проверка, введён ли Yield Stress и % от Yield Stress в случае, если bolt grade выбран из списка
                    res = SetUpYield(statusGrade);
                    break;
            }
            return res;
        }

        // Проверка, выбран ли bolt size, вызов методов проверки ввода данных, которые от него зависят
        private bool SetUpSize(string statusSize)
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

        // Считывание данных с полей свойств болта
        // Проверка, введены ли свойства болта в случае, если выбран размер болта Custom и установка их
        // в специальные переменные в Presenter'е
        private bool setUpProperties(string statusSize)
        {

            bool checkingProp;
            double helpD;
            double helpE;
            double helpH;
            double helpK;
            double helpP;
            double helpNOTPI;
            double helpNutWidth;
            string[] prop = PageCalculationBT.GetProperties();
            if (Double.TryParse(prop[0], out helpD) == false || Double.TryParse(prop[1], out helpE) == false
                || Double.TryParse(prop[2], out helpH) == false
                || Double.TryParse(prop[3], out helpK) == false || Double.TryParse(prop[4], out helpP) == false
                || Double.TryParse(prop[5], out helpNOTPI) == false || Double.TryParse(prop[6], out helpNutWidth) == false)
            {
                PageCalculationBT.ShowErrorMessage("Properties");
                checkingProp = false;
            }
            else if (helpD == 0.0 || helpE == 0.0 || helpH == 0.0 || helpK == 0.0 || helpP == 0.0 || helpNOTPI == 0.0 || helpNutWidth == 0.0)
            {
                PageCalculationBT.ShowErrorMessage("PropertiesNull");
                checkingProp = false;
            }
            else
            {
                customD = helpD;
                customE = helpE;
                customH = helpH;
                customK = helpK;
                customP = helpP;
                customNOTPI = helpNOTPI;
                customNutWidth = helpNutWidth;
                checkingProp = true;
            }
            return checkingProp;
        }

        // Считывание данных с полей свойств болта
        // Проверка, введены ли b в случае, если выбран размер болта Custom и установка их
        // в специальные переменные в Presenter'е
        private bool SetUpYield(string grade)
        {
            bool res;
            if (grade == "Custom") // Выбран кастомный grade
            {
                double help1;
                double help2;
                string[] yieldValues = PageCalculationBT.GetYieldStressCustom();
                if (Double.TryParse(yieldValues[0], out help1) == true && Double.TryParse(yieldValues[1], out help2) == true)
                {
                    if (help1 != 0.0 && help2 != 0.0)
                    {   // Введены YIELD Stress и его %
                        yieldStressValueCustom = help1;
                        yieldStressPerCent = help2;
                        res = true;
                    }
                    else if (help1 == 0.0)
                    {
                        // Введён Yield Stress, равный 0
                        PageCalculationBT.ShowErrorMessage("YieldStressNull");
                        res = false;
                    }
                    else if (help2 == 0.0)
                    {
                        // Введён % Yield Stress, равный 0
                        PageCalculationBT.ShowErrorMessage("YieldStressNull");
                        res = false;
                    }
                    else
                    {
                        // Введён Yield Stress И % Yield Stress, равный 0
                        PageCalculationBT.ShowErrorMessage("YieldStressNull");
                        res = false;
                    }
                }
                else if (Double.TryParse(yieldValues[0], out help1) == false && Double.TryParse(yieldValues[1], out help2) == true)
                {
                    // Не введён Yield Stress при выбранном bolt grade "Custom"
                    PageCalculationBT.ShowErrorMessage("Yield");
                    res = false;
                }
                else if (Double.TryParse(yieldValues[0], out help1) == true && Double.TryParse(yieldValues[1], out help2) == false)
                {
                    // Не введён % YieldStress при выбранном bolt grade "Custom"
                    PageCalculationBT.ShowErrorMessage("PerCent");
                    res = false;
                }
                else
                {
                    // Не введён % YieldStress И YieldStress при выбранном bolt grade "Custom"
                    PageCalculationBT.ShowErrorMessage("Yield");
                    res = false;
                }
            }
            else // Выбран grade из списка
            {
                string[] yieldValues = PageCalculationBT.GetYieldStressCustom();
                double help;
                if (Double.TryParse(yieldValues[1], out help) == true)
                {
                    if (help == 0)
                    {
                        // Введён % Yield Stress, равный 0
                        PageCalculationBT.ShowErrorMessage("YieldStressNull");
                        res = false;
                    }
                    else
                    {
                        yieldStressPerCent = help;
                        res = true;
                    }
                }
                else
                {
                    // Не введён % YieldStress при выбранном bolt grade из списка
                    PageCalculationBT.ShowErrorMessage("PerCent");
                    res = false;
                }
            }
            return res;
        }

        // Проверка, введён ли коэффициент трения и запись его в переменную
        private bool CheckFCoeff()
        {
            
            bool checkFC;
            double help;
            string helpString = PageCalculationBT.GetFCoeff();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageCalculationBT.ShowErrorMessage("FCoeff");
                checkFC = false;
            }
            else if (help <= 0 || help > 1)
            {
                // Коэффициент трения не входит необходимый диапазон
                PageCalculationBT.ShowErrorMessage("FCoeffLimits");
                checkFC = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение коэффициента трения во ViewModel
                frictionCoeffViewModel = help;
                checkFC = true;
            }

            return checkFC;
        }

        // Проверка, введён ли коэффициент К и запись его в переменную
        private bool CheckKCoeff()
        {
            
            bool checkKC;
            double help;
            string helpString = PageCalculationBT.GetKCoeff();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageCalculationBT.ShowErrorMessage("KCoeff");
                checkKC = false;
            }
            else if (help <= 0)
            {
                // Коэффициент К не входит необходимый диапазон
                PageCalculationBT.ShowErrorMessage("KCoeffLimits");
                checkKC = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение коэффициента К во ViewModel
                kCoeffViewModel = help;
                checkKC = true;
            }

            return checkKC;
        }

        // Создание объекта-калькулятора и установка в него необходимых данных
        private CalculateBTC CreateCalculator(string statusGrade, string statusSize)
        {
            // Создание объекта-калькулятора API6A и установка в него необходимых параметров

            CalculateBTC objectCalculator = new CalculateBTC();

            if (statusGrade == "Custom")
            {
                objectCalculator.yieldStressPsi = yieldStressValueCustom;
                objectCalculator.perCent = yieldStressPerCent;
            }
            else
            {
                objectCalculator.yieldStressPsi = currentBoltGrade.YieldStressPsi;
                objectCalculator.perCent = yieldStressPerCent;
            }

            if (statusSize == "Custom")
            {
                objectCalculator.threadMajorDiameter_D = customD;
                objectCalculator.pitchDiameterOfThread_E = customE;
                objectCalculator.hexSize_H = customH;
                objectCalculator.nutInternalChamfer_K = customK;
                objectCalculator.threadPitch_P = customP;
                objectCalculator.numberOfThreadsPerInch = customNOTPI;
                objectCalculator.nutWidth = customNutWidth;
            }
            else
            {
                objectCalculator.threadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
                objectCalculator.pitchDiameterOfThread_E = currentBolt.PitchDiameterOfThread_E;
                objectCalculator.hexSize_H = currentBolt.HexSize_H;
                objectCalculator.nutInternalChamfer_K = currentBolt.NutInternalChamfer_K;
                objectCalculator.threadPitch_P = currentBolt.ThreadPitch_P;
                objectCalculator.numberOfThreadsPerInch = currentBolt.NumberOfThreadsPerInch;
                objectCalculator.nutWidth = currentBolt.NutWidth;
            }

            objectCalculator.fCoeff = frictionCoeffViewModel;
            objectCalculator.kCoeff = kCoeffViewModel;

            return objectCalculator;

        }




        // Подключение к базе данных и загрузка из неё двух таблиц
        public void LoadDataFromDB()
        {
            workWithDataBaseBTCObject.LoadData();
        }

    }
}
