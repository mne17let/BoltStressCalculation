using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
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

        // Реализация Singleton

        private static WorkWithDataBaseBTC instance;

        // Реализация Singleton
        public static WorkWithDataBaseBTC GetInstance()
        {
            if (instance == null)
            {
                instance = new WorkWithDataBaseBTC();
            }
            return instance;
        }

        // Очистка коллекций с объектами из база данных, если они не пустые, а затем вызов методов для их заполнения по нужному фильтру
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

        // Заполнение списка объектами болтов из таблицы BoltSize, в зависимости от выбранного grade
        // Если grade выбран "Custom", то в список добавляются все болты из таблицы BoltSize
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

        // Заполнение списка объектами болтов из таблицы BoltGrade, в зависимости от выбранного grade
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

        // Присваивание переменным-объектам болта конкретного выбранного пользователем болта из списков
        public void UpdateDataBaseForSizeChange(string size, string grade)
        {
            UpdateModelListsForGradeChange(grade);

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

        // Возврат коллекции болтов из таблицы BoltGrade
        public List<BoltGradeProperties> GetBoltGradeProperties()
        {
            return gradeData;
        }

        // Возврат коллекции болтов из таблицы BoltSize
        public List<BoltProperties> GetBoltProperties()
        {
            return boltData;
        }

        // Возврат объекта болта из таблицы BoltGrade
        public BoltGradeProperties GetCurrentBoltGradeProperties()
        {
            return currentBoltGrade;
        }

        // Возврат объекта болта из таблицы BoltSize
        public BoltProperties GetCurrentBoltProperties()
        {
            return currentBolt;
        }

        // Загрузка всех данных из базы данных. Загружаю обе таблицы сразу, чтобы избежать многопоточности
        // Изначально фильтровал и загружал только нужные объекты, но первое подключение к базе данных было слишком ресурсоёмким
        // Возможно, стоит попробовать в начале просто "подключиться" к базе данных, тем самым, видимо, решив наиболее "долгую задачу",
        // а потом брать нужные объекты по фильтру. 
        public void LoadData()
        {
            dataBaseContextObject.BoltGradeProperties.Load();
            helpGradeList = dataBaseContextObject.BoltGradeProperties.Local.ToList();

            dataBaseContextObject.BoltProperties.Load();
            helpBoltSizeList = dataBaseContextObject.BoltProperties.Local.ToList();
        }
    }
}
