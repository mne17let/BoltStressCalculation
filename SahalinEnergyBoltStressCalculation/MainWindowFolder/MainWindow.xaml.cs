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
using SahalinEnergyBoltStressCalculation.BTC_GasketTargetStress.View;
using SahalinEnergyBoltStressCalculation.BTC_PressureAndGasketType.View;

namespace SahalinEnergyBoltStressCalculation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PageBoltTorqueCalculation pageBoltTorqueCalculation = new PageBoltTorqueCalculation();
        Page_GasketTargetStress page_GasketTargetStress = new Page_GasketTargetStress();
        Page_PressureAndGasketType page_PressureAndGasketType = new Page_PressureAndGasketType();


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

            ContentControl ct = new ContentControl();
            ct.Content = "Torque calculation based on bolt material yield stress";
            ct.FontSize = 25;
            ct.FontWeight = FontWeights.Bold;

            InfoButton.Content = ct;
        }

        public void BTCGasketTargetStressButtonClick(object buttonIbject, RoutedEventArgs args_name)
        {
            MainFrame.Content = page_GasketTargetStress;

            Button btn = (Button)((Control)buttonIbject);


            btn.Style = (Style)this.FindResource("NavigationPickedButtonStyle");
            ButtonNavigation_BTC_SingleBoltStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
            ButtonNavigation_BTC_PressureAndGasketType.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");

            ContentControl ct = new ContentControl();
            ct.Content = "Torque calculation based on target assembly gasket stress";
            ct.FontSize = 25;
            ct.FontWeight = FontWeights.Bold;

            InfoButton.Content = ct;

        }

        public void BTCPressureAndGasketTypeButtonClick(object buttonIbject, RoutedEventArgs args_name)
        {
            MainFrame.Content = page_PressureAndGasketType;

            Button btn = (Button)((Control)buttonIbject);


            btn.Style = (Style)this.FindResource("NavigationPickedButtonStyle");
            ButtonNavigation_BTC_GasketTargetStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");
            ButtonNavigation_BTC_SingleBoltStress.Style = (Style)this.FindResource("NavigationUnpickedButtonStyle");

            ContentControl ct = new ContentControl();
            ct.Content = "Torque calculation based on pressure and minimum gasket seating stress";
            ct.FontSize = 25;
            ct.FontWeight = FontWeights.Bold;

            InfoButton.Content = ct;

        }




    }
}
