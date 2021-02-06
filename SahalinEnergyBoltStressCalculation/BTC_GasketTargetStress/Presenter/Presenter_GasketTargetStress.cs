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

    }
}
