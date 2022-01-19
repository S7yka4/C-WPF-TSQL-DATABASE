using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using WpfApp1.Managers;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        Guid WorkSpaceId;
        PCsManager PCsManager;
        MonitorsManager MonitorsManager;
        WorkSpacesManager WorkSpacesManager;

        public ConnectWindow(Guid Id,DataGrid WorkSpaceGrid)
        {
            InitializeComponent();
            WorkSpaceId = Id;
            PCsManager = new PCsManager(PCsGrid);
            MonitorsManager = new MonitorsManager(MonitorsGrid);
            WorkSpacesManager = new WorkSpacesManager(WorkSpaceGrid);
            PCsManager.GetUnconnectedTable();
            MonitorsManager.GetUnconnectedTable();
            
            

        }

        private void SetConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PCsGrid.SelectedItem != null)
                    WorkSpacesManager.SetPCConnection(WorkSpaceId, new Guid(((DataRowView)PCsGrid.SelectedItem).Row.ItemArray[0].ToString()));
                if (MonitorsGrid.SelectedItem != null)
                    WorkSpacesManager.SetMonConnection(WorkSpaceId, new Guid(((DataRowView)MonitorsGrid.SelectedItem).Row.ItemArray[0].ToString()));

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }

        }
    }
}
