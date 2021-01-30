using SahalinEnergyBoltStressCalculation.BTCalculation.CalculationBTClasses;
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
        // Переменная ViewModel
        private ViewModelCalculateBT viewModelAtCalculationBTC = ViewModelCalculateBT.GetInstance();

        // Переменные, отвечающие за то, выбран ли "Custom" size или grade болта
        private string statusGrade, statusSize;

        // Конструктор класса, который вызывается при создании страницы
        public PageBoltTorqueCalculation()
        {
            InitializeComponent();
            InitFun();
        }

        // Функция для установки начальных параметров
        public void InitFun()
        {
            viewModelAtCalculationBTC.PageCalculationBT = this;
            viewModelAtCalculationBTC.LoadDataFromDB();

            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
            CalculateButton.Click += ListenerForCalculateButton;



        }

        // Слушатель ComboBox с grade болтов
        public void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string gradeStringForViewModel = comboBoxItem.Content.ToString();
            viewModelAtCalculationBTC.UpdateViewModelWithComboBoxWithGrades(gradeStringForViewModel);

        }

        // Слушатель ComboBox с размерами болтов
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

        // Слушатель кнопки "Посчитать"
        public void ListenerForCalculateButton(object sender, RoutedEventArgs e)
        {
            ComboBoxItem itemSize = (ComboBoxItem)((ComboBox)ComboBoxWithBoltSize).SelectedItem;
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;

            statusGrade = itemGrade.Content.ToString();
            statusSize = itemSize.Content.ToString();

            viewModelAtCalculationBTC.BeginCalculation(statusGrade, statusSize);
        }



        // Изменения View, когда выбрал grade болта
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

        // Установка видимости для полей YieldStress и YieldStress-подписи
        public void SetVisibileYield()
        {
            TextYeildStress.Visibility = Visibility.Visible;
            TextBoxYieldStress.Visibility = Visibility.Visible;
        }

        // Скрытие полей YieldStress и YieldStress-подписи
        public void SetVisibilityForYieldStress()
        {
            TextYeildStress.Visibility = Visibility.Hidden;
            TextBoxYieldStress.Visibility = Visibility.Hidden;
        }

        // Очистка полей свойств болта, зависящих от его размера
        public void SetEmptyPropertiesWhenGradeChange()
        {
            TextBoxFor_D.Text = "";
            TextBoxFor_E.Text = "";
            TextBoxFor_H.Text = "";
            TextBoxFor_K.Text = "";
            TextBoxFor_P.Text = "";
        }

        // Установка параметра "Можно вписывать значения" для полей свойств болта, зависящих от его размера
        public void SetReadOnlyFalseForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = false;
            TextBoxFor_E.IsReadOnly = false;
            TextBoxFor_H.IsReadOnly = false;
            TextBoxFor_K.IsReadOnly = false;
            TextBoxFor_P.IsReadOnly = false;
        }

        // Обновление списка ComboBox с размерами после выбора grade болта
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

        // Изменение View после выбора размера болта
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

        // Установка YieldStress в случае, если выбран не "Custom" grade болта и не "Custom" размер болта
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

        // Установка параметра "Заблокировать возможность вписать данные" для полей свойств болта, зависящих от его размера
        public void SetIsReadOnlyForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = true;
            TextBoxFor_E.IsReadOnly = true;
            TextBoxFor_H.IsReadOnly = true;
            TextBoxFor_K.IsReadOnly = true;
            TextBoxFor_P.IsReadOnly = true;
        }

        // Получение свойств болта, зависящих от его размера и установка в текстовые поля
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

        // Проверка вводимых знаков и отклонение любых знаков, кроме
        // Цифр
        // Точки не в начале числа и только один раз
        // Точки после нуля и ничего кроме
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
            } else if (currentText == "0")
            {
                if (textSymbols.Text != ".")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            }
        }

        // Отклоняем ввод пробела в нужных полях ввода
        private void WithoutSpace(object sender, KeyEventArgs button)
        {
            if (button.Key == Key.Space)
            {
                button.Handled = true; // если пробел, отклоняем ввод
            }
        }

        // Отдаём "собранные" с полей YieldStress и YieldPercent данные
        public string[] GetYieldStressCustom()
        {
            string valueYield = TextBoxYieldStress.Text;
            string perCent = TextBoxForYieldPercent.Text;

            string[] arrString = new string[] { valueYield, perCent };
            return arrString;
        }

        // Отдаём "собранные" с полей ввода свойства болта
        public string[] GetProperties()
        {
            string d = TextBoxFor_D.Text;
            string e = TextBoxFor_E.Text;
            string h = TextBoxFor_H.Text;
            string k = TextBoxFor_K.Text;
            string p = TextBoxFor_P.Text;
            
            string[] properties = new string[] { d, e, h, k, p };
            return properties;
        }

        // Отдаём "собранный" с поля ввода коэффициент трения
        public string GetFCoeff()
        {
            string fCoeff = TextBoxForFrictionCoefficient.Text;
            return fCoeff;
        }

        // Показываем окно ошибки с нужным текстом
        public void ShowErrorMessage(string code)
        {
            switch (code)
            {
                case "Yield":
                    MessageBox.Show("Введите значение Yield Stress");
                    break;
                case "PerCent":
                    MessageBox.Show("Введите значение % Yield Stress");
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
                case "FCoeffLimits":
                    MessageBox.Show("Коэффициент трения не может быть меньше или равен 0 или больше 1");
                    break;
                case "PropertiesNull":
                    MessageBox.Show("Значения характеристик болта не могут быть равны 0");
                    break;
                case "YieldStressNull":
                    MessageBox.Show("Значения Yield Stress и % Yield Stress не могут быть равны 0");
                    break;
            }
        }

        // Изменения View в результате вычислительных операций
        public void ChangeUIOnCalculation(CalculateBTC objectCalculator, string grade, string size)
        {
            SigmaTextTest.Text = "Sigma=" + objectCalculator.GetSigma().ToString();
            AsTextTest.Text = "As=" +  objectCalculator.GetAs().ToString();
            BSTextTest.Text = "BS=" + grade;
            BGTextTest.Text = "BG=" + size;
            FTextTest.Text = "F=" + objectCalculator.GetF().ToString();
            TauText.Text = "Tau=" + objectCalculator.GetTau().ToString();
        }

        
    }

    
}
