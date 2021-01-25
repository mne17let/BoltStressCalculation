using SahalinEnergyBoltStressCalculation.LogicClassesFolder;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder.CalculationOne;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SahalinEnergyBoltStressCalculation.PageClassesFolder
{
    /// <summary>
    /// Логика взаимодействия для PageBoltTorqueCalculation.xaml
    /// </summary>
    public partial class PageBoltTorqueCalculation : Page
    {
        
        private Array listBolts;
        private ViewModelCalculeteBT viewModelAtCalculationBTC = ViewModelCalculeteBT.GetInstance();

        public PageBoltTorqueCalculation()
        {
            InitializeComponent();
            InitFun();
        }


        public void InitFun()
        {
            viewModelAtCalculationBTC.PageCalculationBT = this;

            CalculateButton.Click += ListenerForCalculateButton;
            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            viewModelAtCalculationBTC.UpdateDataBase();


            
        }

        public void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string stringForViewModel = comboBoxItem.Content.ToString();
        }

        public void ListenerForBoltSizeComboBox()
        {

        }
        
        public void ListenerForCalculateButton(object sender, RoutedEventArgs e)
        {

            MyDataBaseContext dataBaseContextObject = new MyDataBaseContext();

            dataBaseContextObject.BoltProperties.Load();
            listBolts = dataBaseContextObject.BoltProperties.Local.ToArray();
            dataBaseContextObject.BoltGradeProperties.Where(p => p.BoltGrade == "A193 B8M2 class 2B").Load();
            var b = dataBaseContextObject.BoltGradeProperties.Local.ToList();
            

        public void ShowEmptyMessage(string code)
        {
            switch (code)
            {
                case "BoltSize":
                    MessageBox.Show("Pick bolt size");
                    break;
                case "BoltGrade":
                    MessageBox.Show("Pick bolt grade");
                    break;
            }
        }

        public void OnChangeGradeComboBox(string code)
        {

        }

        public void BlockYieldTextBox()
        {
            TextBoxYieldStress.IsReadOnly = true;
        }
    }
}
