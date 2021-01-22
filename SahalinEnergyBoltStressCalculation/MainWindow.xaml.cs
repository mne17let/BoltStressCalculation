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
using SahalinEnergyBoltStressCalculation;
using System.Data.Entity;


namespace SahalinEnergyBoltStressCalculation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitFun();
        }


        public void InitFun()
        {
            CalculateButton.Click += ReturnTableData;
        }

        public void ReturnTableData(object sender, RoutedEventArgs e)
        {
            if (EnterBoltSizeText.Text == "")
            {
                MessageBox.Show("Введите размер болта");
                return;
            }
            else
            {
                MyDataBaseContext dataBaseContextObject = new MyDataBaseContext();
                dataBaseContextObject.Bolts.Load();

                Array listBolts = dataBaseContextObject.Bolts.Local.ToArray();

                var d = 0;

                foreach (Bolt i in listBolts)
                {
                    if (EnterBoltSizeText.Text == i.BoltSize)
                    {
                        TextBoxFor_D.Text = i.ThreadMajorDiameter_D.ToString();
                        TextBoxFor_E.Text = i.PitchDiameterOfThread_E.ToString();
                        TextBoxFor_H.Text = i.HexSize_H.ToString();
                        TextBoxFor_K.Text = i.NutInternalChamfer_K.ToString();
                        TextBoxFor_P.Text = i.ThreadPitch_P.ToString();
                        return;
                    } else
                    {
                        d++;
                    }
                }
                if (d == 2)
                {
                    MessageBox.Show("Ничего не найдено");
                }
            }

        }
    }
}
