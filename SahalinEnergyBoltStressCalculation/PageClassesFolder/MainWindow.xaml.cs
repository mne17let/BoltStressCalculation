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
using SahalinEnergyBoltStressCalculation.PageClassesFolder;

namespace SahalinEnergyBoltStressCalculation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PageCalculate2 pageCalculate2 = new PageCalculate2();
        PageBoltTorqueCalculation pageBoltTorqueCalculation = new PageBoltTorqueCalculation();
        


        public MainWindow()
        {
            InitializeComponent();
            InitFun();
        }

        public void InitFun()
        {
            MainFrame.Content = new PageBoltTorqueCalculation();
            ButtonOpenCalculateBTC.Click += OpenCalculateBTC;
            ButtonOpenCalculate2.Click += OpenCalculate2;
        }

        public void OpenCalculateBTC(object variable_name, RoutedEventArgs args_name)
        {
            MainFrame.Content = pageBoltTorqueCalculation;
        }

        public void OpenCalculate2(object variable_name, RoutedEventArgs args_name)
        {
            MainFrame.Content = pageCalculate2;
        }
    }
}
