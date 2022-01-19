using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Entities;

namespace WpfApp1.Managers
{
    class MonitorGridManager
    {

        public DataGrid Grid;

        public MonitorGridManager(DataGrid grid)
        {
            Grid = grid;
        
        }

        public List<Guid> GetSelectedItems()
        {
            List<Guid> SelectedMonitors = new List<Guid>();
            for (int i = 0; i < Grid.SelectedItems.Count; i++)
                SelectedMonitors.Add(new Guid(((DataRowView)Grid.SelectedItems[i]).Row.ItemArray[0].ToString()));
            return SelectedMonitors;
        }

    }
    class MonitorsManager
    {
        DataSet ResultOfSQL;
        MonitorGridManager GridManager;
        SqlDataAdapter dAdapt = new SqlDataAdapter("", ConnectionManager.ConnectString);

        public MonitorsManager(DataGrid datagrid)
        {
      
            ResultOfSQL = new DataSet("Monitors");
            GridManager = new MonitorGridManager(datagrid);
        }

        public void GetFullTable()
        { 
            dAdapt.SelectCommand.CommandText = "SELECT * FROM Monitors";
            GetTable();
        }

        private void GetTable()
        {
            ResultOfSQL.Clear();
            try
            {
                dAdapt.Fill(ResultOfSQL, "Monitors");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            GridManager.Grid.ItemsSource = ResultOfSQL.Tables[0].DefaultView;
        }

        public void GetUnconnectedTable()
        {
           
            dAdapt.SelectCommand.CommandText = "SELECT Monitors.InventoryNumber, Monitors.Size,Monitors.Matrix" +
                            " FROM Monitors LEFT OUTER JOIN WorkSpaces ON Monitors.InventoryNumber = WorkSpaces.MonitorInventoryNumber" +
                            " WHERE WorkSpaces.Id IS NULL";
            GetTable();
        }

        public void Add(Monitor tmp)
        {
            string querry = "INSERT INTO Monitors VALUES (@InventoryNumber," +tmp.Size +  ", @Matrix)";
            using (var Connection = new SqlConnection(ConnectionManager.ConnectString))
            {
                SqlCommand ResultQuerry = new SqlCommand(querry, Connection);
                ResultQuerry.Parameters.Add("@InventoryNumber", SqlDbType.NChar);
                ResultQuerry.Parameters["@InventoryNumber"].Value = tmp.InventoryNumber.ToString();
                ResultQuerry.Parameters.Add("@Matrix", SqlDbType.Char);
                ResultQuerry.Parameters["@Matrix"].Value = tmp.Matrix;
                try
                {
                    Connection.Open();
                    ResultQuerry.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                GetFullTable();
            }
        }

        public void Delete(Guid id)
        {
            string Query = "DELETE FROM Monitors WHERE [InventoryNumber] = " + "'" + id.ToString() + "'";
            using (var Connection = new SqlConnection(ConnectionManager.ConnectString))
            {
                SqlCommand ResultQuerry = new SqlCommand(Query, Connection);
                try
                {
                    Connection.Open();
                    ResultQuerry.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                GetFullTable();
            }
        }

        public void DeleteSelectedItems(WorkSpacesManager WSM)
        {
            var Source = GridManager.GetSelectedItems();
            for (int i = 0; i < Source.Count; i++)
            {
                    Delete(Source[i]);
                if ((WSM != null)&&(WSM.GetConnectedWorkSpace(Source[i]) != Guid.Empty))
                    WSM.DisconnectMon(WSM.GetConnectedWorkSpace(Source[i]));
            }  
        }

        public string MakeQuery(string mins, string maxs, string matrix)
        {
            var tmp = new MonitorFilter(maxs,mins,matrix);
            string Result = "SELECT *  FROM Monitors WHERE PATINDEX(" + "'%" + tmp.matrix + "%'" + ",[Matrix]) > 0  AND [Size] >= " + tmp.minsize + " AND [Size] <= " + tmp.maxsize;
            return Result;
        }

        public void Filter(string mins,string maxs,string matrix)
        {
            string Query = MakeQuery(mins,maxs,matrix);
            dAdapt.SelectCommand.CommandText = Query;
            GetTable();
        }

        public void GetAveregeSize()
        {
            string Query = "SELECT AVG(Size) AS 'Средняя диагональ монитора' FROM Monitors";
            dAdapt.SelectCommand.CommandText = Query;
            ResultOfSQL.Clear();
            try
            {
                dAdapt.Fill(ResultOfSQL, "AverageSize");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            GridManager.Grid.ItemsSource = ResultOfSQL.Tables[1].DefaultView;
        }
    }

    class MonitorFilter
    {
        public int minsize;
        public string matrix;
        public int maxsize;

        public MonitorFilter(string maxs,string mins, string m)
        {
            if (maxs != "")
                maxsize = Convert.ToInt32(maxs);
            else
                maxsize = 1000;
            if (mins != "")
                minsize = Convert.ToInt32(mins);
            else
                minsize = 0;
            matrix = m;
        }
    }
}

