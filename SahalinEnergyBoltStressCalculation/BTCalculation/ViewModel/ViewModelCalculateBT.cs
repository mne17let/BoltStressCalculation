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
        // Переменная базы данных
        private MyDataBaseContext dataBaseContextObject = new MyDataBaseContext();

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

        private ViewModelCalculeteBT() {}

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
                    ReturnForChangingGrade("Custom");
                    break;
                default:
                    UpdateDataBaseForGradeChange(grade);
                    ReturnForChangingGrade("not custom");
                    setArrayOfCurrentSizes();
                    break;
            }
        }

        public void setArrayOfCurrentSizes()
        {
            string[] sizesArrayForItems = new string[boltData.Count];

            for (int i = 0;i < boltData.Count;i++)
            {
                sizesArrayForItems[i] = boltData[i].BoltSize;
            }

            PageCalculationBT.UpdateComboBoxWithSize(sizesArrayForItems);
        }

        public void UpdateDataBaseForGradeChange(string filter)
        {
            dataBaseContextObject.BoltGradeProperties.Where(p => p.BoltGrade == filter).Load();
            var helpGradeList = dataBaseContextObject.BoltGradeProperties.Local.ToList();

            dataBaseContextObject.BoltProperties.Load();
            var helpBoltSizeList = dataBaseContextObject.BoltProperties.Local.ToList();

            List<string> boltSizeList = new List<string>();

            foreach (var i in helpGradeList)
            {
                foreach (var a in helpBoltSizeList)
                {
                    if(i.BoltDiameter == a.ThreadMajorDiameter_D)
                    {
                        gradeData.Add(i);
                        boltData.Add(a);
                    }
                }
            }

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
                    break;
                default:
                    UpdateDataBaseForSizeChange(size);
                    ReturnForChangingSize();
                    break;

            }
        }

        public void UpdateDataBaseForSizeChange(string size)
        {
            foreach (var i in boltData)
            {
                if (i.BoltSize == size)
                {
                    currentBolt = i;
                }
            }

            foreach (var a in gradeData)
            {
                if (a.BoltDiameter == currentBolt.ThreadMajorDiameter_D)
                {
                    currentBoltGrade = a;
                }
            }
        }

        public void ReturnForChangingSize()
        {
            double currentThreadMajorDiameter_D = currentBolt.ThreadMajorDiameter_D;
            double currentPitchDiameterOfThread_E = currentBolt.PitchDiameterOfThread_E;
            double currentHexSize_H = currentBolt.HexSize_H;
            double currentNutInternalChamfer_K = currentBolt.NutInternalChamfer_K;
            double currentThreadPitch_P = currentBolt.ThreadPitch_P;

            PageCalculationBT.ChangeUiWhenSizePicked(currentThreadMajorDiameter_D, currentPitchDiameterOfThread_E, currentHexSize_H,
                currentNutInternalChamfer_K, currentThreadPitch_P);
        }


    }
}
