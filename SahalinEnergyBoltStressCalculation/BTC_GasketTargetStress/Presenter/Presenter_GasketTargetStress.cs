﻿using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.CalculationClass;
using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.View;
using SahalinEnergyBoltStressCalculation.BTCalculation.Model;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
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


        // Переменная для работы с базой данных
        private WorkWithDataBaseBTC workWithDataBase = WorkWithDataBaseBTC.GetInstance();

        // Переменная для работы с View
        public Page_GasketTargetStress PageView { private get; set; }

        // Переменные со списком данных для определённых Bolt Grade и Bolt Size
        List<BoltGradeProperties> gradeData = new List<BoltGradeProperties>();
        List<BoltProperties> boltData = new List<BoltProperties>();

        // Переменные конкретного типа болта
        private BoltGradeProperties currentBoltGrade;
        private BoltProperties currentBolt;


        // Переменные, получаемые из View

        double yieldStressValueCustom;

        double customD;
        double customP;

        double frictionCoeffPresenter;
        double kCoeffPresenter;
        double numOfBoltsPresenter;
        double gasketOutsideDiameterPresenter;
        double gasketInsideDiameterPresenter;
        double targetAssemblyGasketStressPresenter;




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
            workWithDataBase.UpdateModelListsForGradeChange(filter);
            gradeData = workWithDataBase.GetBoltGradeProperties();
            boltData = workWithDataBase.GetBoltProperties();
        }

        // Вызов метода реакции у View на выбор Bolt Grade и передача в этот метод информации, выбран ли "Custom" Bolt Grade
        private void ReturnForChangingGrade(string customGradeOrNot)
        {
            PageView.ChangeUiWhenGradePicked(customGradeOrNot);
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
            workWithDataBase.UpdateDataBaseForSizeChange(size, grade);
            currentBolt = workWithDataBase.GetCurrentBoltProperties();
            currentBoltGrade = workWithDataBase.GetCurrentBoltGradeProperties();
        }

        // Вызов методов у View в качествер реакции на выбор bolt size
        private void ReturnForChangingSize(string size)
        {
            switch (size)
            {
                case "Custom":
                    PageView.ChangeUiWhenSizePicked("Custom");
                    break;
                default:
                    PageView.ChangeUiWhenSizePicked("not custom");
                    break;
            }
        }

        // Выбран bolt size
        // Возврат характеристик конкретного болта, выбранного пользователем
        public double[] GetBoltSizeProperties()
        {
            double currentThreadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
            double currentThreadPitch_P = currentBolt.ThreadPitch_P;

            var properties = new double[] {currentThreadMajorDiameter_D, currentThreadPitch_P};
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
            if (SetUpGrade(statusGrade, statusSize) == false)
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
            else if (CheckNumberOfBolts() == false)
            {
                // Число болтов не введено
                return;
            }
            else if (CheckGasketOutsideDiameter() == false)
            {
                // Gasket Outside Diameter не введён
                return;
            }
            else if (CheckGasketInsideDiameter() == false)
            {
                // Gasket Inside Diameter не введён
                return;
            }
            else if (CheckOutMoreIn_Diameter() == false)
            {
                // Внешний диаметр меньше или равен внутреннему
                return;
            }
            else if (CheckTargetAssemblyGasketStress() == false)
            {
                // Target Assembly Gasket Stress не введён
                return;
            }
            else
            {
                // Всё в порядке. Создаём необходимые объекты калькулятора, а также размера и grade болта
                // Передаём всё в метод View и вызываем у него в качествер реакциии на нажатие кнопки "Calculate"
                var objectCalculator = CreateCalculator(statusGrade, statusSize);
                string grade;
                string size;

                if (statusGrade == "Custom" || statusSize == "Custom")
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

                PageView.ChangeUIOnCalculation(objectCalculator, grade, size);
            }
        }

        // Создание объекта-калькулятора и установка в него необходимых данных
        private Calculator_GasketTargetStress CreateCalculator(string statusGrade, string statusSize)
        {
            Calculator_GasketTargetStress objectCalculator = new Calculator_GasketTargetStress();

            if (statusGrade == "Custom" || statusSize == "Custom")
            {
                objectCalculator.yieldStressPsi = yieldStressValueCustom;
            } else
            {
                objectCalculator.yieldStressPsi = currentBoltGrade.YieldStressPsi;
            }

            if (statusSize == "Custom")
            {
                objectCalculator.threadMajorDiameter_D = customD;
                objectCalculator.threadPitch_P = customP;
            } else
            {
                objectCalculator.threadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
                objectCalculator.threadPitch_P = currentBolt.ThreadPitch_P;
            }

            objectCalculator.fCoeff = frictionCoeffPresenter;
            objectCalculator.kCoeff = kCoeffPresenter;
            objectCalculator.numOfBolts = numOfBoltsPresenter;
            objectCalculator.gasketOutsideDiameter = gasketOutsideDiameterPresenter;
            objectCalculator.gasketInsideDiameter = gasketInsideDiameterPresenter;
            objectCalculator.targetAssemblyGasketStress = targetAssemblyGasketStressPresenter;

            return objectCalculator;

        }

        // Проверка, выбран ли bolt grade, вызов методов проверки ввода данных, которые от него зависят
        private bool SetUpGrade(string statusGrade, string statusSize)
        {
            bool res;
            switch (statusGrade)
            {
                // Проверка, введён ли Yield Stress и % от Yield Stress в случае, если bolt grade выбран "Custom"
                case "Pick bolt grade":
                    PageView.ShowErrorMessage("BoltGrade");
                    res = false;
                    break;
                case "Custom":
                    // Проверка, введён ли Yield Stress и % от Yield Stress в случае, если bolt grade выбран "Custom"
                    res = SetUpYield(statusGrade, statusSize);
                    break;
                default:
                    // Проверка, введён ли % от Yield Stress в случае, если bolt grade выбран из списка
                    res = SetUpYield(statusGrade, statusSize);
                    break;
            }
            return res;
        }

        // Считывание данных с полей свойств болта
        // Проверка, введены ли % YIELD stress и YIELD stress в случае, если выбран размер болта Custom и установка их
        // в специальные переменные в Presenter'е
        private bool SetUpYield(string grade, string size)
        {
            bool res;
            if (grade == "Custom" || size == "Custom") // Выбран кастомный grade
            {
                double help;

                string yieldValue = PageView.GetYieldStressCustom();

                if (Double.TryParse(yieldValue, out help) == true)
                { // Введен YIELD Stress
                    if (help != 0.0)
                    { // Не равен 0
                        yieldStressValueCustom = help;
                        res = true;
                    }
                    else
                    {
                        // Введён Yield Stress, равный 0
                        PageView.ShowErrorMessage("YieldStressNullOnly");
                        res = false;
                    }
                }
                else
                {
                    // Не введён Yield Stress при выбранном bolt grade "Custom"
                    PageView.ShowErrorMessage("Yield");
                    res = false;
                }
            }
            else // Выбран grade из списка
            {
                res = true;
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
                    PageView.ShowErrorMessage("BoltSize");
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

        // Получение данных с полей свойств болта
        // Проверка, введены ли свойства болта в случае, если выбран размер болта Custom и установка их
        // в специальные переменные в Presenter'е
        private bool setUpProperties(string statusSize)
        {

            bool checkingProp;
            double helpD;
            double helpP;
            string[] prop = PageView.GetProperties();
            if (Double.TryParse(prop[0], out helpD) == false || Double.TryParse(prop[1], out helpP) == false)
            {
                PageView.ShowErrorMessage("Properties");
                checkingProp = false;
            }
            else if (helpD == 0.0 || helpP == 0.0)
            {
                PageView.ShowErrorMessage("PropertiesNull");
                checkingProp = false;
            }
            else
            {
                customD = helpD;
                customP = helpP;
                checkingProp = true;
            }
            return checkingProp;
        }

        // Проверка, введён ли коэффициент трения и запись его в переменную
        private bool CheckFCoeff()
        {

            bool checkFC;
            double help;
            string helpString = PageView.GetFCoeff();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("FCoeff");
                checkFC = false;
            }
            else if (help < 0 || help > 1)
            {
                // Коэффициент трения не входит необходимый диапазон
                PageView.ShowErrorMessage("FCoeffLimits");
                checkFC = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение коэффициента трения во ViewModel
                frictionCoeffPresenter = help;
                checkFC = true;
            }

            return checkFC;
        }

        // Проверка, введён ли коэффициент К и запись его в переменную
        private bool CheckKCoeff()
        {

            bool checkKC;
            double help;
            string helpString = PageView.GetKCoeff();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("KCoeff");
                checkKC = false;
            }
            else if (help < 0)
            {
                // Коэффициент К не входит необходимый диапазон
                PageView.ShowErrorMessage("KCoeffLimits");
                checkKC = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение коэффициента К во ViewModel
                kCoeffPresenter = help;
                checkKC = true;
            }

            return checkKC;
        }

        // Проверка, введено ли число болтов и запись его в переменную
        private bool CheckNumberOfBolts()
        {

            bool checkNB;
            double help;
            string helpString = PageView.GetBoltNumbers();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("NumberOfBolts");
                checkNB = false;
            }
            else if (help <= 0)
            {
                // Число болтов не входит необходимый диапазон
                PageView.ShowErrorMessage("NumberOfBoltsLimits");
                checkNB = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение коэффициента К во ViewModel
                numOfBoltsPresenter = help;
                checkNB = true;
            }

            return checkNB;
        }

        // Проверка, введён ли Gasket Outside Diameter и запись его в переменную
        private bool CheckGasketOutsideDiameter()
        {

            bool checkGOD;
            double help;
            string helpString = PageView.GetGasketOutsideDiameter();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("GasketOutsideDiameter");
                checkGOD = false;
            }
            else if (help <= 0)
            {
                // Gasket Outside Diameter не входит необходимый диапазон
                PageView.ShowErrorMessage("GasketOutsideDiameterLimits");
                checkGOD = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение Gasket Outside Diameter в Presenter
                gasketOutsideDiameterPresenter = help;
                checkGOD = true;
            }

            return checkGOD;
        }

        // Проверка, введён ли Gasket Inside Diameter и запись его в переменную
        private bool CheckGasketInsideDiameter()
        {

            bool checkGID;
            double help;
            string helpString = PageView.GetGasketInsideDiameter();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("GasketInsideDiameter");
                checkGID = false;
            }
            else if (help <= 0)
            {
                // Gasket Inside Diameter не входит необходимый диапазон
                PageView.ShowErrorMessage("GasketInsideDiameterLimits");
                checkGID = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение Gasket Inside Diameter в Presenter
                gasketInsideDiameterPresenter = help;
                checkGID = true;
            }

            return checkGID;
        }

        // Проверка, больше ли Gasket Outside Diameter чем Gasket Inside Diameter и вывод ошибки, если нет
        private bool CheckOutMoreIn_Diameter()
        {

            bool checkOMI;

            if (gasketOutsideDiameterPresenter > gasketInsideDiameterPresenter)
            {
                checkOMI = true;
            }
            else
            {
                PageView.ShowErrorMessage("OutMoreThanIn_Diameter");
                checkOMI = false;
            }

            return checkOMI;
        }

        // Проверка, введён ли Target Assembly Gasket Stress и запись его в переменную
        private bool CheckTargetAssemblyGasketStress()
        {

            bool checkTAGS;
            double help;
            string helpString = PageView.GetTargetAssemblyGasketStress();
            if (Double.TryParse(helpString, out help) == false)
            {
                PageView.ShowErrorMessage("TargetAssemblyGasketStress");
                checkTAGS = false;
            }
            else if (help <= 0)
            {
                // Target Assembly Gasket Stress не входит необходимый диапазон
                PageView.ShowErrorMessage("TargetAssemblyGasketStressLimits");
                checkTAGS = false;
            }
            else
            {
                // Всё в порядке, устанавливаем нужное значение Target Assembly Gasket Stress в Presenter
                targetAssemblyGasketStressPresenter = help;
                checkTAGS = true;
            }

            return checkTAGS;
        }

    }
}
