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
using WpfApp1.Managers;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PCControlWindow PCWindow;
        MonitorControlWindow MonitorWindow;
        WorkSpacesControlWindow WorkSpaceWindow;
        WorkSpacesManager WSManager;
        public MainWindow()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            WSManager = new WorkSpacesManager();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PCWindow != null)
            {
                PCWindow.Close();
                PCWindow = null;
            }
            PCWindow = new PCControlWindow(WSManager);
            PCWindow.Show();
          
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void MonitorButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonitorWindow != null)
            {
                MonitorWindow.Close();
                MonitorWindow = null;
               
            }
            
            MonitorWindow = new MonitorControlWindow(WSManager);
            MonitorWindow.Show();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (WorkSpaceWindow != null)
            {
                WorkSpaceWindow.Close();
                WorkSpaceWindow = null;
                
            }
            WorkSpaceWindow = new WorkSpacesControlWindow(WSManager);
            WorkSpaceWindow.Show();
            WSManager = WorkSpaceWindow.Manager;
            if(PCWindow!=null)
                PCWindow.WSManager = WorkSpaceWindow.Manager;
            if(MonitorWindow!=null)
                MonitorWindow.WSManager = WorkSpaceWindow.Manager;
        }
    }
}
