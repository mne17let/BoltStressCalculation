﻿using System;
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
        PageBoltTorqueCalculation pageBoltTorqueCalculation = new PageBoltTorqueCalculation();
        


        public MainWindow()
        {
            InitializeComponent();
            InitFun();
        }

        public void InitFun()
        {
            MainFrame.Content = pageBoltTorqueCalculation;
            ButtonNavigation_BTC_SingleBoltStress.Click += BTCSingleBoltStressButtonClick;

            ButtonNavigation_BTC_GasketTargetStress.Click += BTCGasketTargetStressButtonClick;

            ButtonNavigation_BTC_PressureAndGasketType.Click += BTCPressureAndGasketTypeButtonClick;
        }

        public void BTCSingleBoltStressButtonClick(object buttonIbject, RoutedEventArgs args_name)
        {
            MainFrame.Content = pageBoltTorqueCalculation;

            Button btn = (Button)((Control)buttonIbject);

            
            btn.Style = (Style)this.FindResource("NavigationPickedButtonStyle");

            ButtonNavigation_BTC_GasketTargetStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
            ButtonNavigation_BTC_PressureAndGasketType.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
        }
        

        public void BTCGasketTargetStressButtonClick(object buttonIbject, RoutedEventArgs args_name)
        {
            
            Button btn = (Button)((Control)buttonIbject);


            btn.Style = (Style)this.FindResource("NavigationPickedButtonStyle");

            ButtonNavigation_BTC_SingleBoltStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
            ButtonNavigation_BTC_PressureAndGasketType.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");

        }

        public void BTCPressureAndGasketTypeButtonClick(object buttonIbject, RoutedEventArgs args_name)
        {
            Button btn = (Button)((Control)buttonIbject);


            btn.Style = (Style)this.FindResource("NavigationPickedButtonStyle");

            ButtonNavigation_BTC_GasketTargetStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
            ButtonNavigation_BTC_SingleBoltStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");

        }




    }
}
