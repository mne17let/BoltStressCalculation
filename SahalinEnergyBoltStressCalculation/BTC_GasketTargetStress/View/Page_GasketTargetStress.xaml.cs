using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.CalculationClass;
using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.Presenter;
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

namespace SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.View
{
    /// <summary>
    /// Логика взаимодействия для Page_GasketTargetStress.xaml
    /// </summary>
    public partial class Page_GasketTargetStress : Page
    {
        // Переменная Presenter'а
        private Presenter_GasketTargetStress presenter_GasketTargetStress = Presenter_GasketTargetStress.GetInstance();

        // Переменная главного окна
        public MainWindow mainWindowCalculationTwo;


        public Page_GasketTargetStress()
        {
            InitializeComponent();

            // Применяю стиль с нужным шрифтом
            Style = (Style)FindResource(typeof(Page));

            InitFun();
        }

        // Функция для установки начальных параметров
        private void InitFun()
        {
            presenter_GasketTargetStress.PageView = this;

            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
            CalculationButton_GasketTargetStress.Click += ListenerForCalculationButton;
            TableButton_GasketTargetStress.Click += OpenWindowWithTable;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Слушатели для кнопок и Combobox
        private void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {
            var comboBoxItem = (ComboBoxItem)((ComboBox)viewObject).SelectedItem;
            string gradeStatusForPresenter = comboBoxItem.Content.ToString();

            presenter_GasketTargetStress.UpdateViewModelWithComboBoxWithGrades(gradeStatusForPresenter);
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
            }
            else if (comboBoxItem.Content.ToString() == "Pick bolt size")
            {
                return;
            }
            else
            {
                string sizeStringForViewModel = comboBoxItem.Content.ToString();
                
                presenter_GasketTargetStress.UpdateViewModelWithComboBoxWithSizes(sizeStringForViewModel, statusGrade);
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

            presenter_GasketTargetStress.BeginCalculation(statusGrade, statusSize);
        }

        // Посылаем объекту главного окна команду "Открыть окно с таблицей"
        private void OpenWindowWithTable(object sender, RoutedEventArgs e)
        {
            mainWindowCalculationTwo.ShowWindowCalculationTwo();
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

        private void OnlyIntegerNumbers(object sender, TextCompositionEventArgs textSymbols)
        {
            var currentText = (string)((TextBox)sender).Text;

            int resParse;
            if (Int32.TryParse(textSymbols.Text, out resParse) == false)
            {
                textSymbols.Handled = true; // отклоняем ввод   
            } else if (currentText  == "")
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

        // Отклоняем ввод числа больше 100, буквы, пробела или повторяющуюся точку, либо точки в начале в % YIELD Stress
        private void OnlyNumbersAndMax100(object sender, TextCompositionEventArgs textSymbols)
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
                if (textSymbols.Text != ",")
                {
                    textSymbols.Handled = true; // отклоняем ввод
                }
            }
            else if (Double.TryParse(currentText + textSymbols.Text, out doubleResParse) == true)
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
                var arrText = currentText.ToArray();
                if (arrText.Length == 5)
                {
                    resultWriting = false;
                }
                else
                {
                    resultWriting = true;
                }
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
            TextBoxForKCoefficient.Text = k.ToString();
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
        // Изменения UI после выбора bolt grade

        // Изменения View, когда выбрал grade болта
        public void ChangeUiWhenGradePicked(string status)
        {
            switch (status)
            {
                case "Custom":
                    UpdateComboBoxWithSize();
                    SetIsReadOnlyFalseForYieldStress();
                    SetEnabledComboBoxWithSizes();
                    ClearYieldStressTextBox();
                    break;
                default:
                    UpdateComboBoxWithSize();
                    SetEmptyPropertiesWhenGradeChange();
                    SetIsReadOnlyTrueForYieldStress();
                    ClearYieldStressTextBox();
                    SetEnabledComboBoxWithSizes();
                    break;
            }

        }

        // Установка видимости для полей YieldStress и YieldStress-подписи
        private void SetIsReadOnlyFalseForYieldStress()
        {
            TextBoxYieldStress.IsReadOnly = false;
        }

        // Скрытие полей YieldStress и YieldStress-подписи
        private void SetIsReadOnlyTrueForYieldStress()
        {
            TextBoxYieldStress.IsReadOnly = true;
        }

        // Очистка TextBox с YieldStress
        private void ClearYieldStressTextBox()
        {
            TextBoxYieldStress.Text = "";
        }

        // Очистка полей свойств болта, зависящих от его размера
        private void SetEmptyPropertiesWhenGradeChange()
        {
            TextBoxFor_D.Text = "";
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


            var arrayOfItems = presenter_GasketTargetStress.GetArrayOfCurrentSizes();

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
                    SetReadOnlyFalseForPropertiesTextBlocks();
                    SetEmptyPropertiesWhenGradeChange();
                    SetIsReadOnlyFalseForYieldStress();
                    ClearYieldStressTextBox();
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
            TextBoxFor_P.IsReadOnly = false;
        }

        // Установка YieldStress в случае, если выбран не "Custom" grade болта и не "Custom" размер болта
        private void SetYieldStress()
        {
            ComboBoxItem itemGrade = (ComboBoxItem)((ComboBox)ComboBoxWithGrades).SelectedItem;


            // Переменная, отвечающая за то, выбран ли "Custom" grade болта (или вообще ничего не выбрано и стоит "Pick grade/size")
            string statusGrade;

            statusGrade = itemGrade.Content.ToString();


            var a = presenter_GasketTargetStress.GetCurrentYieldStress();

            if (a == 0.0)
            {
                return;
            }
            else if (statusGrade == "Custom")
            {
                SetIsReadOnlyFalseForYieldStress();
                return;
                // TextBoxYieldStress.Text = "";
            }
            else
            {
                TextBoxYieldStress.Text = a.ToString();
                SetIsReadOnlyTrueForYieldStress();
            }
        }

        // Установка параметра "Заблокировать возможность вписать данные" для полей свойств болта, зависящих от его размера
        private void SetIsReadOnlyForPropertiesTextBlocks()
        {
            TextBoxFor_D.IsReadOnly = true;
            TextBoxFor_P.IsReadOnly = true;
        }

        // Получение свойств болта, зависящих от его размера и установка в текстовые поля
        private void SetSizeProperties()
        {
            double[] properties = presenter_GasketTargetStress.GetBoltSizeProperties();
            double d = Math.Round(properties[0], 4);
            double p = Math.Round(properties[1], 4);




           TextBoxFor_D.Text = d.ToString();

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
        public string[] GetProperties()
        {
            string d = TextBoxFor_D.Text;
            string p = TextBoxFor_P.Text;

            string[] properties = new string[] { d, p };
            return properties;
        }

        // Отдаём "собранный" с поля ввода коэффициент трения
        public string GetFCoeff()
        {
            string fCoeff = TextBoxForFrictionCoefficient.Text;
            return fCoeff;
        }

        // Отдаю "собранный" с поля ввода коэффициент К
        public string GetKCoeff()
        {
            string kCoeff = TextBoxForKCoefficient.Text;
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

        // Отдаём "собанный" с поля ввода Target Assembly Gasket Stress
        public string GetTargetAssemblyGasketStress()
        {
            string tAGS = TextBoxFor_TargetAssemblyGasketStress.Text;
            return tAGS;
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
                case "YieldStressNullOnly":
                    MessageBox.Show("Yield strength must be greater than 0");
                    break;


                case "KCoeff":
                    MessageBox.Show("Please enter Nut factor K");
                    break;
                case "KCoeffLimits":
                    MessageBox.Show("Nut factor must be greater than or equal to 0");
                    break;

                case "NumberOfBolts":
                    MessageBox.Show("Please enter Number of bolts");
                    break;
                case "NumberOfBoltsLimits":
                    MessageBox.Show("Number of bolts must be greater than 0");
                    break;

                case "GasketOutsideDiameter":
                    MessageBox.Show("Please enter Gasket outside diameter");
                    break;
                case "GasketOutsideDiameterLimits":
                    MessageBox.Show("Gasket outside diameter must be greater than 0");
                    break;

                case "GasketInsideDiameter":
                    MessageBox.Show("Please enter Gasket inside diameter");
                    break;
                case "GasketInsideDiameterLimits":
                    MessageBox.Show("Gasket inside diameter must be greater than 0");
                    break;

                case "OutMoreThanIn_Diameter":
                    MessageBox.Show("Gasket outside diameter must be greater than gasket inside diameter");
                    break;

                case "TargetAssemblyGasketStress":
                    MessageBox.Show("Please enter Target assembly gasket stress");
                    break;
                case "TargetAssemblyGasketStressLimits":
                    MessageBox.Show("Target assembly gasket stress must be greater than 0");
                    break;
            }
        }

        // Изменения View в результате вычислительных операций
        public void ChangeUIOnCalculation(Calculator_GasketTargetStress objectCalculator, string grade, string size)
        {

            SetResultTablesVisibility();


            var bSR = Math.Round(objectCalculator.GetSbsel(), 0);
            var perCent = Math.Round(objectCalculator.GetPercentOfYIELDStress(), 0);
            var tau = Math.Round(objectCalculator.GetTauGasketTargetStress(), 0);


            var convertTau = Math.Round(objectCalculator.GetTauGasketTargetStress() * 1.3558, 0);


            TextBlock_BoltStressRequired.Text = bSR.ToString() + " psi";

            TextBlock_PerCentOfYIELDStress.Text = perCent.ToString() + " %";

            TextBlock_TorqueMoment.Text = "τ = " + tau.ToString() + " Lbf-Ft = " + convertTau.ToString() + " N-m";

            CheckConditionAndShowErrorOrNot(objectCalculator.GetPercentOfYIELDStress());

        }

        // Делаю видимыми таблицы с результатами и невидимым инфобаннер
        private void SetResultTablesVisibility()
        {
            InfoBanner.Visibility = Visibility.Hidden;
            ResultGrid.Visibility = Visibility.Visible;
        }

        // Вывожу ошибку проверки условия, если значение не входит в лимит. Либо скрываю поле ошибки, если всё в порядке.
        private void CheckConditionAndShowErrorOrNot(double perCentForCheck)
        {
            if (perCentForCheck > 80)
            {
                TextBlock_ConditionResult.Visibility = Visibility.Visible;
                TextBlock_ConditionResult.Text = "Bolt stress upper limit control failed [>80%]";
            } else if (perCentForCheck < 20)
            {
                TextBlock_ConditionResult.Visibility = Visibility.Visible;
                TextBlock_ConditionResult.Text = "Bolt stress lower limit control failed [<20%]";
            }
            else
            {
                TextBlock_ConditionResult.Visibility = Visibility.Collapsed;
            }
        }
    }
}
