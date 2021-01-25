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
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;


            
        }

        public void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string gradeStringForViewModel = comboBoxItem.Content.ToString();
            viewModelAtCalculationBTC.UpdateViewModelWithComboBoxWithGrades(gradeStringForViewModel);
        }

        public void ListenerForBoltSizeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            if (comboBoxItem == null)
            {
                return;
            } else if (comboBoxItem.Content.ToString() == "Pick bolt size")
            {
                return;
            } else
            {
                string sizeStringForViewModel = comboBoxItem.Content.ToString();
                viewModelAtCalculationBTC.UpdateViewModelWithComboBoxWithSizes(sizeStringForViewModel);
            }
            
        }

        public void ListenerForCalculateButton(object sender, RoutedEventArgs e)
        {
        }
            

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

        public void ChangeUiWhenGradePicked(string status)
        {
            switch (status)
            {
                case "Custom":
                    ComboBoxWithBoltSize.IsEnabled = true;
                    break;
                default:
                    TextBoxYieldStress.IsReadOnly = true;
                    ComboBoxWithBoltSize.IsEnabled = true;
                    break;
            }
            
        }

        public void UpdateComboBoxWithSize(string[] arrayOfItem)
        {
            ComboBoxWithBoltSize.Items.Clear();

            ComboBoxItem localItem = new ComboBoxItem();
            localItem.MaxHeight = 0;
            localItem.Content = "Pick bolt size";
            ComboBoxWithBoltSize.SelectedIndex = 0;

            ComboBoxWithBoltSize.Items.Add(localItem);
            
            foreach (string s in arrayOfItem)
            {
                ComboBoxWithBoltSize.Items.Add(s);
            };
        }

        public void ChangeUiWhenSizePicked(double d, double e, double h, double k, double p)
        {
            TextBoxFor_D.Text = d.ToString();
            TextBoxFor_D.IsReadOnly = true;

            TextBoxFor_E.Text = e.ToString();
            TextBoxFor_E.IsReadOnly = true;

            TextBoxFor_H.Text = h.ToString();
            TextBoxFor_H.IsReadOnly = true;

            TextBoxFor_K.Text = k.ToString();
            TextBoxFor_K.IsReadOnly = true;

            TextBoxFor_P.Text = p.ToString();
            TextBoxFor_P.IsReadOnly = true;
        }
    }
}
