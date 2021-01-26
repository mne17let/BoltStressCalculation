﻿using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SahalinEnergyBoltStressCalculation.BTCalculation.Model
{
    class WorkWithDataBaseBTC
    {
        // Переменная базы данных
        private MyDataBaseContext dataBaseContextObject = new MyDataBaseContext();

        // Переменные списков нужных данных
        List<BoltGradeProperties> gradeData = new List<BoltGradeProperties>();
        List<BoltProperties> boltData = new List<BoltProperties>();

        // Переменные конкретного типа болта
        private BoltGradeProperties currentBoltGrade;
        private BoltProperties currentBolt;

        // Вспомогательные списки для загрузки данных
        List<BoltGradeProperties> helpGradeList = new List<BoltGradeProperties>();
        List<BoltProperties> helpBoltSizeList = new List<BoltProperties>();

        // Реализация Singleton
        private WorkWithDataBaseBTC() {}

        private static WorkWithDataBaseBTC instance;

        public static WorkWithDataBaseBTC GetInstance()
        {
            if (instance == null)
            {
                instance = new WorkWithDataBaseBTC();
            }
            return instance;
        }

        public void UpdateModelListsForGradeChange(string filter)
        {
            if (gradeData.Count != 0)
            {
                gradeData.Clear();
            }

            if (boltData.Count != 0)
            {
                boltData.Clear();
            }

            switch (filter)
            {
                case "Custom":
                    FillSizeData();
                    break;
                default:
                    FillGradeData(filter);
                    FillSizeData();
                    break;

            }
        }

        public void FillSizeData()
        {
            if (gradeData.Count != 0)
            {
                foreach (var size in helpBoltSizeList)
                {
                    foreach (var grade in gradeData)
                    {
                        if (size.ThreadMajorDiameter_D == grade.BoltDiameter)
                        {
                            boltData.Add(size);
                        }
                    }
                }
            } else
            {
                foreach (var size in helpBoltSizeList)
                {
                    boltData.Add(size);
                }
            }
        }

        public void FillGradeData(string filter)
        {
            foreach (var grade in helpGradeList)
            {
                if (grade.BoltGrade == filter)
                {
                    gradeData.Add(grade);
                }
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

        public List<BoltGradeProperties> GetBoltGradeProperties()
        {
            return gradeData;
        }

        public List<BoltProperties> GetBoltProperties()
        {
            return boltData;
        }

        public BoltGradeProperties GetCurrentBoltGradeProperties()
        {
            return currentBoltGrade;
        }

        public BoltProperties GetCurrentBoltProperties()
        {
            return currentBolt;
        }

        public void LoadData()
        {
            dataBaseContextObject.BoltGradeProperties.Load();
            helpGradeList = dataBaseContextObject.BoltGradeProperties.Local.ToList();

            dataBaseContextObject.BoltProperties.Load();
            helpBoltSizeList = dataBaseContextObject.BoltProperties.Local.ToList();
        }
    }
}
