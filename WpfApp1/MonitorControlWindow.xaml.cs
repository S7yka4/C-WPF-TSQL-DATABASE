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
using System.Windows.Shapes;
using WpfApp1.Managers;
using WpfApp1.Entities;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MonitorControlWindow.xaml
    /// </summary>
    public partial class MonitorControlWindow : Window
    {
        bool FilterFlag;
        bool AvgFlag;
        MonitorsManager Manager;
        public WorkSpacesManager WSManager;
        public MonitorControlWindow(WorkSpacesManager wsmanager=null)
        {   
            InitializeComponent();
            Manager = new MonitorsManager(MonitorsGrid);
            Manager.GetFullTable();
            FilterFlag = false;
            AvgFlag = false;
            WSManager = wsmanager;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Manager.DeleteSelectedItems(WSManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void AvgSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!AvgFlag)
                {
                    AvgFlag = true;
                    Manager.GetAveregeSize();
                    AvgSize.Content = "Вывести таблицу";
                }
                else
                {
                    AvgFlag = false;
                    AvgSize.Content = "Вывести среднюю диагональ";
                    Manager.GetFullTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!FilterFlag)
                {
                    FilterFlag = true;

                    FilterButton.Content = "Сбросить фильтр";
                    Manager.Filter(FilterSizeTextBox.Text, FilterSizeTextBox2.Text, FilterMatrixTextBox.Text);
                }
                else
                {
                    FilterFlag = false;
                    FilterButton.Content = "Применить фильтр";
                    Manager.GetFullTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Monitor tmp = new Monitor(Convert.ToInt32(SizeTextBox.Text), MatrixTextBox.Text);
                Manager.Add(tmp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
            }

        }

    }
}
