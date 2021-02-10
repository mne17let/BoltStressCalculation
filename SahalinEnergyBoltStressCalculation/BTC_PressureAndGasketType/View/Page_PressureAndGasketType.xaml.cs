﻿using SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.CalculationClass;
using SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.Presenter;
using SahalinEnergyBoltStressCalculation.MainWindowFolder;
using System;
using System.Collections.Generic;
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

namespace SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.View
{
    /// <summary>
    /// Логика взаимодействия для Page_PressureAndGasketType.xaml
    /// </summary>
    public partial class Page_PressureAndGasketType : Page
    {
        // Переменная главного окна
        public MainWindow mainWindow;

        // Переменная Presenter'а
        private Presenter_PressureAndGasketType presenter_PressureAndGaskerType = Presenter_PressureAndGasketType.GetInstance();

        public Page_PressureAndGasketType()
        {
            InitializeComponent();
            InitFun();
        }

        private void InitFun()
        {
            presenter_PressureAndGaskerType.PageView = this;
            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
            CalculationButton_PressureAndGasketType.Click += ListenerForCalculationButton;
            TableButton_PressureAndGasketType.Click += OpenWindowWithTable;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Слушатели для кнопок и Combobox
        private void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string gradeStatusForPresenter = comboBoxItem.Content.ToString();

            presenter_PressureAndGaskerType.UpdateViewModelWithComboBoxWithGrades(gradeStatusForPresenter);
        }

        // Слушатель ComboBox с размерами болтов
        private void ListenerForBoltSizeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;


            // Вспомогательная переменная для считывания grade болта в данный момент

            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;

            string statusGrade;
            statusGrade = itemGrade.Content.ToString();

            if (comboBoxItem == null)
            {
                return;
            }
            else if (comboBoxItem.Content.ToString() == "Pick bolt size")
            {
                return;
            }
            else
            {
                string sizeStringForViewModel = comboBoxItem.Content.ToString();

                presenter_PressureAndGaskerType.UpdateViewModelWithComboBoxWithSizes(sizeStringForViewModel, statusGrade);
            }

        }

        // Слушатель кнопки "Посчитать"
        private void ListenerForCalculationButton(object sender, RoutedEventArgs e)
        {
            ComboBoxItem itemSize = (ComboBoxItem)((ComboBox)ComboBoxWithBoltSize).SelectedItem;
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;


            // Переменные, отвечающие за то, выбран ли "Custom" size или grade болта (или вообще ничего не выбрано и стоит "Pick grade/size")
            string statusGrade, statusSize;

            statusGrade = itemGrade.Content.ToString();
            statusSize = itemSize.Content.ToString();

            presenter_PressureAndGaskerType.BeginCalculation(statusGrade, statusSize);
        }

        // Посылаем объекту главного окна команду "Открыть окно с таблицей"
        private void OpenWindowWithTable(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowWindow();
        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Блок кода для проверки вводимого текста

        // Проверка вводимых знаков и отклонение любых знаков, кроме цифр
        // точки не в начале числа и только один раз, точки после нуля и ничего кроме неё
        private void OnlyNumbersOrCommaOneTime(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
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

                var arrSymbols = char.Parse(textSymbols.Text);
                for (int i = 0; i < arrText.Length; i++)
                {
                    if (arrSymbols == arrText[i])
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
            }
        }

        // Только целые числа
        private void OnlyIntegerNumbers(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false)
            {
                textSymbols.Handled = true; // отклоняем ввод   
            }
            else if (currentText == "")
            {
                if (textSymbols.Text == "0")
                {
                    textSymbols.Handled = true;
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

                if (currentText == "")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                    return;
                }

                var arrText = currentText.ToArray();
                var charSymbol = char.Parse(textSymbols.Text);
                for (int i = 0; i < arrText.Length; i++)
                {
                    if (charSymbol == arrText[i])
                    {
                        textSymbols.Handled = true; // отклоняем ввод
                        return;
                    }
                }
            }
            else if (currentText == "0")
            {
                if (textSymbols.Text != ".")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            }
            else
            {
                resultWriting = true;
            }

            string helpCurrentTextFCoeff = currentText + textSymbols.Text;
            double helpK;
            if (Double.TryParse(helpCurrentTextFCoeff, out helpK) == true && resultWriting == true)
            {
                SetAutoK(helpK);
            }
        }

        // Вводим коэфффициент К "на ходу", добавляя 0,04
        private void SetAutoK(double fCoeff)
        {
            double k = fCoeff + 0.04;
            TextBoxFor_KCoefficient.Text = k.ToString();
        }

        // Обновляем коэффициент K в случае "стирания" коэффициента трения f
        private void WithoutSpaceAndMinus004(object sender, KeyEventArgs button)
        {
            if (button.Key == Key.Space)
            {
                button.Handled = true; // если пробел, отклоняем ввод
            }
            else if (button.Key == Key.Back)
            {
                string helpCurrentTextFCoeff = TextBoxFor_FrictionCoefficient.Text;
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
                        TextBoxFor_KCoefficient.Text = "";
                    }

                }


            }

        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Изменения UI после выбора bolt grade

        // Изменения View, когда выбрал grade болта
        public void ChangeUiWhenGradePicked(string status)
        {
            switch (status)
            {
                case "Custom":
                    TextBoxYieldStress.IsReadOnly = false;
                    UpdateComboBoxWithSize();
                    SetVisibileYield();
                    TextBoxYieldStress.Text = "";
                    SetVisibleAndEnabledComboBoxWithSizes();
                    break;
                default:
                    TextBoxYieldStress.IsReadOnly = true;
                    ComboBoxWithBoltSize.IsEnabled = true;
                    UpdateComboBoxWithSize();
                    SetEmptyPropertiesWhenGradeChange();
                    SetCollapsedVisibilityForYieldStress();
                    SetVisibleAndEnabledComboBoxWithSizes();
                    break;
            }

        }

        // Установка видимости для полей YieldStress и YieldStress-подписи
        private void SetVisibileYield()
        {
            TextYeildStress.Visibility = Visibility.Visible;
            TextBoxYieldStress.Visibility = Visibility.Visible;
        }

        // Скрытие полей YieldStress и YieldStress-подписи
        private void SetCollapsedVisibilityForYieldStress()
        {
            TextYeildStress.Visibility = Visibility.Collapsed;
            TextBoxYieldStress.Visibility = Visibility.Collapsed;
        }

        // Очистка полей свойств болта, зависящих от его размера
        private void SetEmptyPropertiesWhenGradeChange()
        {
            TextBoxFor_D.Text = "";
            TextBoxFor_E.Text = "";
            TextBoxFor_H.Text = "";
            TextBoxFor_K.Text = "";
            TextBoxFor_P.Text = "";
        }

        // Обновление списка ComboBox с размерами после выбора grade болта
        private void UpdateComboBoxWithSize()
        {
            ComboBoxWithBoltSize.Items.Clear();

            ComboBoxItem pickBoltSizeItem = new ComboBoxItem();
            pickBoltSizeItem.MaxHeight = 0;
            pickBoltSizeItem.Content = "Pick bolt size";
            ComboBoxWithBoltSize.Items.Add(pickBoltSizeItem);
            ComboBoxWithBoltSize.SelectedIndex = 0;

            ComboBoxItem customItem = new ComboBoxItem();
            customItem.Content = "Custom";
            ComboBoxWithBoltSize.Items.Add(customItem);


            var arrayOfItems = presenter_PressureAndGaskerType.GetArrayOfCurrentSizes();

            foreach (string s in arrayOfItems)
            {
                var cmbItem = new ComboBoxItem();
                cmbItem.Content = s;
                ComboBoxWithBoltSize.Items.Add(cmbItem);
            };
        }

        // Делаю видимым ComboBox с размерами и доступным для выбора элемента
        public void SetVisibleAndEnabledComboBoxWithSizes()
        {
            ComboBoxWithBoltSize.Visibility = Visibility.Visible;
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

        // Установка параметра "Можно вписывать значения" для полей свойств болта, зависящих от его размера
        private void SetReadOnlyFalseForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = false;
            TextBoxFor_E.IsReadOnly = false;
            TextBoxFor_H.IsReadOnly = false;
            TextBoxFor_K.IsReadOnly = false;
            TextBoxFor_P.IsReadOnly = false;
        }

        // Установка YieldStress в случае, если выбран не "Custom" grade болта и не "Custom" размер болта
        private void SetYieldStress()
        {
            if (presenter_PressureAndGaskerType.GetCurrentYieldStress() == 0.0)
            {
                return;
            }
            else
            {
                TextBoxYieldStress.Text = presenter_PressureAndGaskerType.GetCurrentYieldStress().ToString();
                SetVisibileYield();
            }
        }

        // Установка параметра "Заблокировать возможность вписать данные" для полей свойств болта, зависящих от его размера
        private void SetIsReadOnlyForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = true;
            TextBoxFor_E.IsReadOnly = true;
            TextBoxFor_H.IsReadOnly = true;
            TextBoxFor_K.IsReadOnly = true;
            TextBoxFor_P.IsReadOnly = true;
        }

        // Получение свойств болта, зависящих от его размера и установка в текстовые поля
        private void SetSizeProperties()
        {
            double[] properties = presenter_PressureAndGaskerType.GetBoltSizeProperties();
            double d = Math.Round(properties[0], 4);
            double e = Math.Round(properties[1], 4);
            double h = Math.Round(properties[2], 4);
            double k = Math.Round(properties[3], 4);
            double p = Math.Round(properties[4], 4);


            TextBoxFor_D.Text = d.ToString();

            TextBoxFor_E.Text = e.ToString();

            TextBoxFor_H.Text = h.ToString();

            TextBoxFor_K.Text = k.ToString();

            TextBoxFor_P.Text = p.ToString();

        }





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Парсим данные и отдаём их в Presenter

        // Отдаём "собранные" с полей YieldStress и YieldPercent данные
        public string GetYieldStressCustom()
        {
            string valueYield = TextBoxYieldStress.Text;

            return valueYield;
        }

        // Отдаём "собранные" с полей ввода свойства болта
        public string[] GetPropertiesCustom()
        {
            string d = TextBoxFor_D.Text;
            string e = TextBoxFor_E.Text;
            string h = TextBoxFor_H.Text;
            string k = TextBoxFor_K.Text;
            string p = TextBoxFor_P.Text;

            string[] properties = new string[] { d, e, h, k, p};
            return properties;
        }

        // Отдаём "собранный" с поля ввода коэффициент трения
        public string GetFCoeff()
        {
            string fCoeff = TextBoxFor_FrictionCoefficient.Text;
            return fCoeff;
        }

        // Отдаю "собранный" с поля ввода коэффициент К
        public string GetKCoeff()
        {
            string kCoeff = TextBoxFor_KCoefficient.Text;
            return kCoeff;
        }

        // Отдаём "собанное" с поля ввода число болтов
        public string GetBoltNumbers()
        {
            string numOfBolts = TextBoxFor_NumberOfBolts.Text;
            return numOfBolts;
        }

        // Отдаём "собанный" с поля ввода Gasket Outside Diameter
        public string GetGasketOutsideDiameter()
        {
            string gOD = TextBoxFor_GasketOutsideDiameter.Text;
            return gOD;
        }

        // Отдаём "собанный" с поля ввода Gasket Outside Diameter
        public string GetGasketInsideDiameter()
        {
            string gID = TextBoxFor_GasketInsideDiameter.Text;
            return gID;
        }

        // Отдаём "собанный" с поля ввода Internal Design Pressure
        public string GetInternalDesignPressure()
        {
            string iDP = TextBoxFor_InternalDesignPressure.Text;
            return iDP;
        }

        // Отдаём "собанный" с поля ввода Gasket factor m
        public string GetGasketFactor_m()
        {
            string gFm = TextBoxFor_GasketFactor.Text;
            return gFm;
        }

        // Отдаём "собанный" с поля ввода Minimum design seating stress y
        public string GetMinimumDesignSeatingStress_y()
        {
            string mDSS = TextBoxFor_MinimumDesignSeatingStress.Text;
            return mDSS;
        }

        // Отдаём "собанный" с поля ввода Basic gasket seating width b0
        public string GetBasicGasketSeatingWidth_b0()
        {
            string bGSW = TextBoxFor_BasicGasketSeatingWidth.Text;
            return bGSW;
        }

        // Отдаём "собанный" с поля ввода Gasket width N
        public string GetGasketWidth_N()
        {
            string gW_N = TextBoxFor_GasketWidth_N.Text;
            return gW_N;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Изменения UI после нажатия кнопки "Посчитать"

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
                case "YieldStressNullOnly":
                    MessageBox.Show("Значение Yield Stress не может быть равным 0");
                    break;


                case "KCoeff":
                    MessageBox.Show("Введите коэффициент К");
                    break;
                case "KCoeffLimits":
                    MessageBox.Show("Коэффициент К не может быть равен 0");
                    break;

                case "NumberOfBolts":
                    MessageBox.Show("Введите число болтов");
                    break;
                case "NumberOfBoltsLimits":
                    MessageBox.Show("Число болтов должно быть больше 0");
                    break;

                case "GasketOutsideDiameter":
                    MessageBox.Show("Введите Gasket Outside Diameter");
                    break;
                case "GasketOutsideDiameterLimits":
                    MessageBox.Show("Gasket Outside Diameter должен быть больше 0");
                    break;

                case "GasketInsideDiameter":
                    MessageBox.Show("Введите Gasket Inside Diameter");
                    break;
                case "GasketInsideDiameterLimits":
                    MessageBox.Show("Gasket Inside Diameter должен быть больше 0");
                    break;

                case "InternalDesignPressure":
                    MessageBox.Show("Введите Internal Design Pressure");
                    break;
                case "InternalDesignPressureLimits":
                    MessageBox.Show("Internal Design Pressure должен быть больше 0");
                    break;

                case "GasketFactor_m":
                    MessageBox.Show("Введите Gasket Factor m");
                    break;
                case "GasketFactor_m_Limits":
                    MessageBox.Show("Gasket Factor не может быть меньше 0");
                    break;

                case "MinimumDesignSeatingStress_y":
                    MessageBox.Show("Введите Minimum Design Seating Stress");
                    break;
                case "MinimumDesignSeatingStress_y_Limits":
                    MessageBox.Show("Minimum Design Seating Stress не может быть меньше 0");
                    break;

                case "BasicGasketSeatingWidth_b0":
                    MessageBox.Show("Введите Basic Gasket Seating Width b0");
                    break;
                case "BasicGasketSeatingWidth_b0_Limits":
                    MessageBox.Show("Basic Gasket Seating Width b0 должен быть больше 0");
                    break;

                case "GasketWidth_N":
                    MessageBox.Show("Введите Gasket Width");
                    break;
                case "GasketWidth_N_Limits":
                    MessageBox.Show("Gasket Width должен быть больше 0");
                    break;
            }
        }

        // Изменения View в результате вычислительных операций
        public void ChangeUIOnCalculation(Calculator_PressureAndGasketType objectCalculator, string grade, string size)
        {

            SetResultTablesVisibility();

            var wm1 = Math.Round(objectCalculator.Get_Wm1(), 0);
            var wm2 = Math.Round(objectCalculator.Get_Wm2(), 0);
            var singleBoltLoad_Newton = Math.Round(objectCalculator.GetSingleBoltLoad_Newton(), 0);
            var singleBoltLoad_Lbf = Math.Round(objectCalculator.GetSingleBoltLoad_Lbf(), 0);
            var bSR = Math.Round(objectCalculator.GetBoltStressRequired_3calc(), 0);
            var perCent = Math.Round(objectCalculator.GetPerCentOfYIELDStress_3calc(), 1);

            var tau1_Lbf_ft = Math.Round(objectCalculator.GetTau_API6AAnnexD(), 0);
            var tau1_Newton_m = Math.Round(objectCalculator.GetTau_API6AAnnexD() * 1.3558, 0);

            var tau2_Lbf_ft = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixJ(), 0);
            var tau2_Newton_m = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixJ() * 1.3558, 0);

            var tau3_Lbf_ft = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixK_Simplified(), 0);
            var tau3_Newton_m = Math.Round(objectCalculator.GetTau_ASMEPCC_1AppendixK_Simplified() * 1.3558, 0);



            TextBlock_Wm1.Text = "Wm1 = " + wm1.ToString() + " N";
            TextBlock_Wm2.Text = "Wm2 = " + wm2.ToString() + " N";

            TextBlock_SingleBoltLoad.Text = singleBoltLoad_Newton.ToString() + " N = " + singleBoltLoad_Lbf.ToString() + " Lbf";
            TextBlock_BoltStressRequired.Text = bSR.ToString() + " psi";
            TextBlock_PerCentOfYIELDStress.Text = perCent.ToString() + " %";


            TextBlock_TorqueMoment_API6AAnnexD.Text = "τ = " + tau1_Lbf_ft.ToString() + " Lbf-Ft = " + tau1_Newton_m.ToString() + " N-m";
            TextBlock_TorqueMoment_AppendixJ.Text = "τ = " + tau2_Lbf_ft.ToString() + " Lbf-Ft = " + tau2_Newton_m.ToString() + " N-m";
            TextBlock_TorqueMoment_AppendixK.Text = "τ = " + tau3_Lbf_ft.ToString() + " Lbf-Ft = " + tau3_Newton_m.ToString() + " N-m";

        }

        // Делаю видимыми таблицы с результатами и невидимым инфобаннер
        private void SetResultTablesVisibility()
        {
            InfoBanner.Visibility = Visibility.Hidden;
            Table_Wm1.Visibility = Visibility.Visible;
            Table_Wm2.Visibility = Visibility.Visible;
            Table_ThreeRes.Visibility = Visibility.Visible;
            Table_TorqueMoment_API6AAnnexD.Visibility = Visibility;
            Table_TorqueMoment_AppendixJ.Visibility = Visibility.Visible;
            Table_TorqueMoment_AppendixK.Visibility = Visibility.Visible;
        }






    }
}
