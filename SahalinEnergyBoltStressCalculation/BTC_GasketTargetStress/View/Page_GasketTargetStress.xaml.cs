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




        public Page_GasketTargetStress()
        {
            InitializeComponent();
            InitFun();
        }

        // Функция для установки начальных параметров
        private void InitFun()
        {
            ComboBoxWithGrades.SelectionChanged += ListenerForGradeComboBox;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Слушатели для кнопок и Combobox
        private void ListenerForGradeComboBox(object viewObject, RoutedEventArgs someArgs)
        {

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Блок кода для проверки вводимого текста

        // Проверка вводимых знаков и отклонение любых знаков, кроме
        // Цифр
        // Точки не в начале числа и только один раз
        // Точки после нуля и ничего кроме неё
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
    }
}
