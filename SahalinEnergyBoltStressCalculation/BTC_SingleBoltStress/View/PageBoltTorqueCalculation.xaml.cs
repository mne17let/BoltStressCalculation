﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SahalinEnergyBoltStressCalculation.BTCalculation.CalculationBTClasses;
using SahalinEnergyBoltStressCalculation.LogicClassesFolder;

namespace SahalinEnergyBoltStressCalculation.PageClassesFolder
{
    /// <summary>
    /// Логика взаимодействия для PageBoltTorqueCalculation.xaml
    /// </summary>
    public partial class PageBoltTorqueCalculation : Page
    {
        // Переменная ViewModel
        private ViewModelCalculateBT viewModelAtCalculationBTC = ViewModelCalculateBT.GetInstance();

        
        // Конструктор класса, который вызывается при создании страницы
        public PageBoltTorqueCalculation()
        {
            InitializeComponent();

            // Применяю стиль с нужным шрифтом
            Style = (Style)FindResource(typeof(Page));
            InitFun();
        }

        // Функция для установки начальных параметров
        private void InitFun()
        {
            viewModelAtCalculationBTC.PageCalculationBT = this;
            viewModelAtCalculationBTC.LoadDataFromDB();

            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
            CalculationButton_SingleBoltStress.Click += ListenerForCalculateButton;



        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Слушатели для кнопок и Combobox

        // Слушатель ComboBox с grade болтов
        private void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string gradeStringForViewModel = comboBoxItem.Content.ToString();
            viewModelAtCalculationBTC.UpdateViewModelWithComboBoxWithGrades(gradeStringForViewModel);

        }

        // Слушатель ComboBox с размерами болтов
        private void ListenerForBoltSizeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;

            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;

            // Вспомогательная переменная для считывания grade болта в данный момент
            string statusGrade;
            statusGrade = itemGrade.Content.ToString();

            if (comboBoxItem == null)
            {
                return;
            } else if (comboBoxItem.Content.ToString() == "Pick bolt size")
            {
                return;
            } else
            {
                string sizeStringForViewModel = comboBoxItem.Content.ToString();
                viewModelAtCalculationBTC.UpdateViewModelWithComboBoxWithSizes(sizeStringForViewModel, statusGrade);
            }
            
        }

        // Слушатель кнопки "Посчитать"
        private void ListenerForCalculateButton(object sender, RoutedEventArgs e)
        {
            ComboBoxItem itemSize = (ComboBoxItem)((ComboBox)ComboBoxWithBoltSize).SelectedItem;
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;


            // Переменные, отвечающие за то, выбран ли "Custom" size или grade болта (или вообще ничего не выбрано и стоит "Pick grade/size")
            string statusGrade, statusSize;
            
            statusGrade = itemGrade.Content.ToString();
            statusSize = itemSize.Content.ToString();

            viewModelAtCalculationBTC.BeginCalculation(statusGrade, statusSize);
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Изменения UI после выбора bolt grade

        // Изменения View, когда выбрал grade болта
        public void ChangeUiWhenGradePicked(string status)
        {
            switch (status)
            {
                case "Custom":
                    ClearYieldStressTextBox();
                    UpdateComboBoxWithSize();
                    SetIsReadOnlyFalseFor_TextBoxYieldStress();
                    SetEnabledComboBoxWithSizes();
                    break;
                default:
                    ClearYieldStressTextBox();
                    UpdateComboBoxWithSize();
                    SetEmptyPropertiesWhenGradeChange();
                    SetIsReadOnlyTrueFor_TextBoxYieldStress();
                    SetEnabledComboBoxWithSizes();
                    break;
            }
            
        }

        // Очистка TextBox с YieldStress
        private void ClearYieldStressTextBox()
        {
            TextBoxYieldStress.Text = "";
        }

        // Установка видимости для полей YieldStress и YieldStress-подписи
        private void SetIsReadOnlyFalseFor_TextBoxYieldStress()
        {
            TextBoxYieldStress.IsReadOnly = false;
        }

        // Скрытие полей YieldStress и YieldStress-подписи
        private void SetIsReadOnlyTrueFor_TextBoxYieldStress()
        {
            TextBoxYieldStress.IsReadOnly = true;
        }

        // Очистка полей свойств болта, зависящих от его размера
        private void SetEmptyPropertiesWhenGradeChange()
        {
            TextBoxFor_D.Text = "";
            TextBoxFor_E.Text = "";
            TextBoxFor_H.Text = "";
            TextBoxFor_P.Text = "";
        }

        // Обновление списка ComboBox с размерами после выбора grade болта
        private void UpdateComboBoxWithSize()
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

        // Делаю видимым ComboBox с размерами и доступным для выбора элемента
        private void SetEnabledComboBoxWithSizes()
        {
            ComboBoxWithBoltSize.IsEnabled = true;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Изменения UI после выбора bolt size

        // Изменение View после выбора размера болта
        public void ChangeUiWhenSizePicked(string size)
        {
            switch (size)
            {
                case "Custom":
                    ClearYieldStressTextBox();
                    SetReadOnlyFalseForPropertiesTextBlocks();
                    SetEmptyPropertiesWhenGradeChange();
                    SetIsReadOnlyFalseFor_TextBoxYieldStress();
                    break;
                default:
                    SetSizeProperties();
                    SetIsReadOnlyForPropertiesTextBlocks();
                    SetYieldStress();
                    break;
            }
        }

        // Установка параметра "Можно вписывать значения" для полей свойств болта, зависящих от его размера
        private void SetReadOnlyFalseForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = false;
            TextBoxFor_E.IsReadOnly = false;
            TextBoxFor_H.IsReadOnly = false;
            TextBoxFor_P.IsReadOnly = false;
        }

        // Установка YieldStress в случае, если выбран не "Custom" grade болта и не "Custom" размер болта
        private void SetYieldStress()
        {
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;


            // Переменная, отвечающая за то, выбран ли "Custom" grade болта (или вообще ничего не выбрано и стоит "Pick grade/size")
            string statusGrade;

            statusGrade = itemGrade.Content.ToString();


            var a = viewModelAtCalculationBTC.GetCurrentYieldStress();

            if (a == 0.0)
            {
                return;
            } else if (statusGrade == "Custom")
            {
                SetIsReadOnlyFalseFor_TextBoxYieldStress();
                return;
                // TextBoxYieldStress.Text = "";
            } else
            {
                TextBoxYieldStress.Text = a.ToString();
                SetIsReadOnlyTrueFor_TextBoxYieldStress();
            }
        }

        // Установка параметра "Заблокировать возможность вписать данные" для полей свойств болта, зависящих от его размера
        private void SetIsReadOnlyForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = true;
            TextBoxFor_E.IsReadOnly = true;
            TextBoxFor_H.IsReadOnly = true;
            TextBoxFor_P.IsReadOnly = true;
        }

        // Получение свойств болта, зависящих от его размера и установка в текстовые поля
        private void SetSizeProperties()
        {
            double[] properties = viewModelAtCalculationBTC.GetBoltSizeProperties();
            double d = Math.Round(properties[0], 4);
            double e = Math.Round(properties[1], 4);
            double h = Math.Round(properties[2], 4);
            double p = Math.Round(properties[3], 4);


            TextBoxFor_D.Text = d.ToString();

            TextBoxFor_E.Text = e.ToString();

            TextBoxFor_H.Text = h.ToString();

            TextBoxFor_P.Text = p.ToString();
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Блок кода для проверки вводимого текста

        // Проверка вводимых знаков и отклонение любых знаков, кроме
        // Цифр
        // Точки не в начале числа и только один раз
        // Точки после нуля и ничего кроме
        private void OnlyNumbers(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false && textSymbols.Text != ",")
            {
                textSymbols.Handled = true; // отклоняем ввод   
            } else if (textSymbols.Text == ",")
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
                if (textSymbols.Text != ",")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            }
        }

        // Отклоняем ввод числа больше 100, буквы, пробела или повторяющуюся точку, либо точки в начале в % YIELD Stress
        private void OnlyNumbersAndOnly100(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            double doubleResParse;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false && textSymbols.Text != ",")
            {
                textSymbols.Handled = true; // отклоняем ввод   
            }
            else if (textSymbols.Text == ",")
            {
                var arrText = currentText.ToArray();
                if (currentText == "")
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
            else if (currentText == "0")
            {
                if (textSymbols.Text != ",")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            } else if (Double.TryParse(currentText + textSymbols.Text, out doubleResParse) == true)
            {
                if (doubleResParse > 100)
                {
                    textSymbols.Handled = true;
                }
            }
        }

        // Показываем сразу же подсчёт коэффициента K в случае, если вводится коэффициент трения
        private void OnlyNumbersAndCountK(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            bool resultWriting = false;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false && textSymbols.Text != ",")
            {
                textSymbols.Handled = true; // отклоняем ввод   
            }
            else if (textSymbols.Text == ",")
            {
                var arrText = currentText.ToArray();
                if (currentText == "")
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
            else if (currentText == "0")
            {
                if (textSymbols.Text != ",")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            } else
            {
                var arrText = currentText.ToArray();
                if (arrText.Length == 5)
                {
                    resultWriting = false;
                } else
                {
                    resultWriting = true;
                }
                
            }


            string helpCurrentTextFCoeff = currentText + textSymbols.Text;
            double helpK;
            if (Double.TryParse(helpCurrentTextFCoeff, out helpK) == true && resultWriting == true){
                SetAutoK(helpK);
            }
        }

        // Вводим коэфффициент К "на ходу", добавляя 0,04
        private void SetAutoK(double fCoeff)
        {
            double k = fCoeff+ 0.04;
            TextBoxForKCoefficient.Text = k.ToString();
        }

        // Отклоняем ввод пробела в нужных полях ввода
        private void WithoutSpace(object sender, KeyEventArgs button)
        {
            if (button.Key == Key.Space)
            {
                button.Handled = true; // если пробел, отклоняем ввод
            }
        }

        // Обновляем коэффициент K в случае "стирания" числа
        private void WithoutSpaceAndMinus004(object sender, KeyEventArgs button)
        {
            if (button.Key == Key.Space)
            {
                button.Handled = true; // если пробел, отклоняем ввод
            } else if (button.Key == Key.Back)
            {
                string helpCurrentTextFCoeff = TextBoxForFrictionCoefficient.Text;
                int countCurrentLength = helpCurrentTextFCoeff.Length;
                if (countCurrentLength > 0)
                {
                    string deleteLast = helpCurrentTextFCoeff.Remove(countCurrentLength - 1);
                    double helpK;
                    if (Double.TryParse(deleteLast, out helpK) == true)
                    {
                        SetAutoK(helpK);
                    }
                    else
                    {
                        TextBoxForKCoefficient.Text = "";
                    }

                }
                

            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Изменения UI после нажатия кнопки "Посчитать"

        // Показываем окно ошибки с нужным текстом
        public void ShowErrorMessage(string code)
        {
            switch (code)
            {
                case "Yield":
                    MessageBox.Show("Please enter Yield strength");
                    break;
                case "PerCent":
                    MessageBox.Show("Please enter Percentage of yield strength");
                    break;
                case "Properties":
                    MessageBox.Show("Please enter bolt data");
                    break;
                case "BoltSize":
                    MessageBox.Show("Please select bolt size");
                    break;
                case "BoltGrade":
                    MessageBox.Show("Please select bolt grade");
                    break;
                case "FCoeff":
                    MessageBox.Show("Please enter Friction coefficient");
                    break;
                case "FCoeffLimits":
                    MessageBox.Show("The friction coefficient must be greater than or equal to 0 and less than or equal to 1");
                    break;
                case "PropertiesNull":
                    MessageBox.Show("Bolt characteristic value cannot be 0");
                    break;
                case "YieldStressNull":
                    MessageBox.Show("Yield strength must be greater than 0");
                    break;
                case "YieldStressPercentLimits":
                    MessageBox.Show("Yield strength percentage must be greater than or equal to 0");
                    break;
                case "YieldStressNullBoth":
                    MessageBox.Show("Yield strength must be greater than 0");
                    break;
                case "KCoeff":
                    MessageBox.Show("Please enter Factor K");
                    break;
                case "KCoeffLimits":
                    MessageBox.Show("Nut factor must be greater than or equal to 0");
                    break;
            }
        }

        // Изменения View в результате вычислительных операций
        public void ChangeUIOnCalculation(CalculateBTC objectCalculator, string grade, string size)
        {

            SetResultTablesVisibility();


            var f = Math.Round(objectCalculator.GetF(), 0);
            var tauAPI6AAnnexD = Math.Round(objectCalculator.GetTau_API6AAnnexD(), 0);
            var tauASMEPCC_1AppendixJ = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixJ(), 0);
            var tauASMEPCC_1AppendixK_Simplified = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixK_Simplified(), 0);


            var convertF = Math.Round(objectCalculator.GetF() * 4.4482, 0);
            var convertTauAPI6AAnnexD = Math.Round(objectCalculator.GetTau_API6AAnnexD() * 1.3558, 0);
            var convertTauASMEPCC_1AppendixJ = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixJ() * 1.3558, 0);
            var convertTauASMEPCC_1AppendixK_Simplified = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixK_Simplified() * 1.3558, 0);


            Text_TauAPI6AAnnexD.Text = "τ = " + tauAPI6AAnnexD.ToString() + " Lbf-Ft = " + convertTauAPI6AAnnexD.ToString() + " N-m";

            Text_TauASMEPCC_1AppendixJ.Text = "τ = " + tauASMEPCC_1AppendixJ.ToString() + " Lbf-Ft = " + convertTauASMEPCC_1AppendixJ.ToString() + " N-m";

            Text_TauASMEPCC_1AppendixK_Simplified.Text = "τ = " + tauASMEPCC_1AppendixK_Simplified.ToString() + " Lbf-Ft = " + convertTauASMEPCC_1AppendixK_Simplified.ToString() + " N-m";

            TextBlock_ForcePerBolt.Text = "F = " + f.ToString() + " Lbf = " + convertF.ToString() + " N";
        }

        // Делаю видимыми таблицы с результатами и невидимым инфобаннер
        private void SetResultTablesVisibility()
        {
            InfoBanner.Visibility = Visibility.Hidden;
            ResultGrid.Visibility = Visibility.Visible;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Парсим данные и отдаём их в Presenter

        // Отдаю "собранный" с поля ввода коэффициент К
        public string GetKCoeff()
        {
            string kCoeff = TextBoxForKCoefficient.Text;
            return kCoeff;
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
            string p = TextBoxFor_P.Text;

            string[] properties = new string[] { d, e, h, p};
            return properties;
        }

        // Отдаём "собранный" с поля ввода коэффициент трения
        public string GetFCoeff()
        {
            string fCoeff = TextBoxForFrictionCoefficient.Text;
            return fCoeff;
        }

    }

    
}
