using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
using SahalinEnergyBoltStressCalculation.PageClassesFolder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
                    break;
                default:
                    UpdateDataBaseForGradeChange(grade);
                    ReturnForChangingGrades();
                    break;
            }
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

        public void ReturnForChangingGrades()
        {
            PageCalculationBT.BlockYieldTextBox();
        }
       
    }
}
