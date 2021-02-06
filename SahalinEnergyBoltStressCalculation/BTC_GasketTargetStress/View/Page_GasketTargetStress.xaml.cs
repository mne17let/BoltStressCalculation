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


        public Page_GasketTargetStress()
        {
            InitializeComponent();
            InitFun();
        }

        // Функция для установки начальных параметров
        private void InitFun()
        {
            presenter_GasketTargetStress.PageView = this;

            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
            ComboBoxWithBoltSize.SelectionChanged += ListenerForBoltSizeComboBox;
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
                    SetHiddenVisibilityForYieldStress();
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
        private void SetHiddenVisibilityForYieldStress()
        {
            TextYeildStress.Visibility = Visibility.Hidden;
            TextBoxYieldStress.Visibility = Visibility.Hidden;
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
            TextBoxFor_P.IsReadOnly = false;
        }

        // Установка YieldStress в случае, если выбран не "Custom" grade болта и не "Custom" размер болта
        private void SetYieldStress()
        {
            if (presenter_GasketTargetStress.GetCurrentYieldStress() == 0.0)
            {
                return;
            }
            else
            {
                TextBoxYieldStress.Text = presenter_GasketTargetStress.GetCurrentYieldStress().ToString();
                SetVisibileYield();
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
            double d = properties[0];
            double p = properties[1];



           TextBoxFor_D.Text = d.ToString();

           TextBoxFor_P.Text = p.ToString();

        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Парсим данные и отдаём их в Presenter

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

                case "TargetAssemblyGasketStress":
                    MessageBox.Show("Введите Target Assembly Gasket Stress");
                    break;
                case "TargetAssemblyGasketStressLimits":
                    MessageBox.Show("Target Assembly Gasket Stress должен быть больше 0");
                    break;
            }
        }
    }
}
