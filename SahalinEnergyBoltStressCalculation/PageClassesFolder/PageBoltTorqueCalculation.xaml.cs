using System;
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
        MyDataBaseContext dataBaseContextObject = new MyDataBaseContext();
        Array listBolts;

        public PageBoltTorqueCalculation()
        {
            InitializeComponent();
            InitFun();
        }


        public void InitFun()
        {
            CalculateButton.Click += ReturnTableData;
            dataBaseContextObject.Bolts.Load();
            listBolts = dataBaseContextObject.Bolts.Local.ToArray();
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
                    }
                    else
                    {
                        d++;
                    }
                }
                if (d == listBolts.Length)
                {
                    MessageBox.Show("Ничего не найдено");
                }
            }

        }
    }
}
