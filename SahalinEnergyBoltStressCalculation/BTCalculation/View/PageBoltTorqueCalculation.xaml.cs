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
        
        private ViewModelCalculeteBT viewModelAtCalculationBTC = ViewModelCalculeteBT.GetInstance();

        private string statusGrade, statusSize;

        public PageBoltTorqueCalculation()
        {
            InitializeComponent();
            InitFun();
        }


        public void InitFun()
        {
            viewModelAtCalculationBTC.PageCalculationBT = this;
            viewModelAtCalculationBTC.LoadDataFromDB();

            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
            CalculateButton.Click += ListenerForCalculateButton;



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
            ComboBoxItem itemSize = (ComboBoxItem)((ComboBox)ComboBoxWithBoltSize).SelectedItem;
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;

            statusGrade = itemGrade.Content.ToString();
            statusSize = itemSize.Content.ToString();

            viewModelAtCalculationBTC.BeginCalculate(statusGrade, statusSize);
        }
            

        

        public void ChangeUiWhenGradePicked(string status)
        {
            switch (status)
            {
                case "Custom":
                    ComboBoxWithBoltSize.IsEnabled = true;
                    TextBoxYieldStress.IsReadOnly = false;
                    UpdateComboBoxWithSize();
                    SetVisibileYield();
                    break;
                default: 
                    TextBoxYieldStress.IsReadOnly = true;
                    ComboBoxWithBoltSize.IsEnabled = true;
                    UpdateComboBoxWithSize();
                    SetEmptyPropertiesWhenGradeChange();
                    SetVisibilityForYieldStress();
                    break;
            }
            
        }

        public void SetVisibileYield()
        {
            TextYeildStress.Visibility = Visibility.Visible;
            TextBoxYieldStress.Visibility = Visibility.Visible;
        }

        public void SetVisibilityForYieldStress()
        {
            TextYeildStress.Visibility = Visibility.Hidden;
            TextBoxYieldStress.Visibility = Visibility.Hidden;
        }

        public void SetEmptyPropertiesWhenGradeChange()
        {
            TextBoxFor_D.Text = "";
            TextBoxFor_E.Text = "";
            TextBoxFor_H.Text = "";
            TextBoxFor_K.Text = "";
            TextBoxFor_P.Text = "";
        }

        public void SetReadOnlyFalseForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = false;
            TextBoxFor_E.IsReadOnly = false;
            TextBoxFor_H.IsReadOnly = false;
            TextBoxFor_K.IsReadOnly = false;
            TextBoxFor_P.IsReadOnly = false;
        }

        public void UpdateComboBoxWithSize()
        {
            ComboBoxWithBoltSize.Items.Clear();

            ComboBoxItem localItem = new ComboBoxItem();
            localItem.MaxHeight = 0;
            localItem.Content = "Pick bolt size";
            ComboBoxWithBoltSize.Items.Add(localItem);
            ComboBoxWithBoltSize.SelectedIndex = 0;

            ComboBoxItem customItem = new ComboBoxItem();
            customItem.Content = "Custom";
            ComboBoxWithBoltSize.Items.Add(customItem);


            var arrayOfItems = viewModelAtCalculationBTC.GetArrayOfCurrentSizes();

            foreach (string s in arrayOfItems)
            {
                var cmbItem = new ComboBoxItem();
                cmbItem.Content = s;
                ComboBoxWithBoltSize.Items.Add(cmbItem);
            };
        }

        public void ChangeUiWhenSizePicked(string size)
        {
            switch (size)
            {
                case "Custom":
                    SetReadOnlyFalseForPropertiesTextBlocks();
                    SetEmptyPropertiesWhenGradeChange();
                    break;
                default:
                    SetSizeProperties();
                    SetIsReadOnlyForPropertiesTextBlocks();
                    SetYieldStress();
                    break;
            }
        }

        public void SetYieldStress()
        {
            var a = viewModelAtCalculationBTC.GetCurrentYieldStress();
            if (viewModelAtCalculationBTC.GetCurrentYieldStress() == 0.0)
            {
                return;
            } else
            {
                TextBoxYieldStress.Text = viewModelAtCalculationBTC.GetCurrentYieldStress().ToString();
                SetVisibileYield();
            }
        }

        public void SetIsReadOnlyForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = true;
            TextBoxFor_E.IsReadOnly = true;
            TextBoxFor_H.IsReadOnly = true;
            TextBoxFor_K.IsReadOnly = true;
            TextBoxFor_P.IsReadOnly = true;
        }

        public void SetSizeProperties()
        {
            double[] properties = viewModelAtCalculationBTC.GetBoltSizeProperties();
            double d = properties[0];
            double e = properties[1];
            double h = properties[2];
            double k = properties[3];
            double p = properties[4];

            TextBoxFor_D.Text = d.ToString();

            TextBoxFor_E.Text = e.ToString();

            TextBoxFor_H.Text = h.ToString();

            TextBoxFor_K.Text = k.ToString();

            TextBoxFor_P.Text = p.ToString();
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false && textSymbols.Text != ".")
            {
                textSymbols.Handled = true; // отклоняем ввод   
            } else if (textSymbols.Text == ".")
            {
                var arrText = currentText.ToArray();
                if(currentText == "")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                    return;
                }

                var arrSymbols = textSymbols.Text.ToArray();
                for (int i = 0; i < arrText.Length; i++)
                {
                    if (arrSymbols[0] == arrText[i])
                    {
                        textSymbols.Handled = true; // отклоняем ввод
                        return;
                    }
                }
            }
        }

        private void WithoutSpace(object sender, KeyEventArgs button)
        {
            if (button.Key == Key.Space)
            {
                button.Handled = true; // если пробел, отклоняем ввод
            }
        }

        public double[] GetYieldStressCustom()
        {
            double value;
            double perCent;
            var tryBool = Double.TryParse(TextBoxYieldStress.Text, out value);
            tryBool = Double.TryParse(TextBoxForYieldPercent.Text, out perCent);

            double[] yieldDouble = new double[] { value, perCent };
            return yieldDouble;
        }

        public double[] GetProperties()
        {
            double d;
            double e;
            double h;
            double k;
            double p;
            var trybool = Double.TryParse(TextBoxFor_D.Text, out d);
            trybool = Double.TryParse(TextBoxFor_E.Text, out e);
            trybool = Double.TryParse(TextBoxFor_H.Text, out h);
            trybool = Double.TryParse(TextBoxFor_K.Text, out k);
            trybool = Double.TryParse(TextBoxFor_P.Text, out p);

            double[] properties = new double[] { d, e, h, k, p };
            return properties;
        }

        public double GetFCoeff()
        {
            double fCoeff;
            bool tryBool = Double.TryParse(TextBoxForFrictionCoefficient.Text, out fCoeff);
            return fCoeff;
        }

        public void ShowErrorMessage(string code)
        {
            switch (code)
            {
                case "Yield":
                    MessageBox.Show("Введите значения для Yield Stress");
                    break;
                case "FrictionCoefficient":
                    MessageBox.Show("Введите значение коэффициента трения");
                    break;
                case "Properties":
                    MessageBox.Show("Введите значения всех характеристик болта");
                    break;
                case "BoltSize":
                    MessageBox.Show("Pick bolt size");
                    break;
                case "BoltGrade":
                    MessageBox.Show("Pick bolt grade");
                    break;
                case "FCoeff":
                    MessageBox.Show("Введите значение коэффициента трения");
                    break;
            }
        }

        public void ShowResult(string res)
        {
            ResultText.Text = res;
        }

        public void ShowInfoMessage(string s1, string s2)
        {
            MessageBox.Show(s1 + " size" + s2 + " grade" );
        }

        
    }

    
}
