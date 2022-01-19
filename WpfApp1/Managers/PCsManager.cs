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
    class PCGridManager
    {
        
        public DataGrid Grid;

        public PCGridManager(DataGrid grid)
        {
            Grid = grid;
        }

        public List<Guid> GetSelectedItems()
        {
            List<Guid> SelectedPCs = new List<Guid>();
            for (int i = 0; i < Grid.SelectedItems.Count; i++)
                SelectedPCs.Add(new Guid(((DataRowView)Grid.SelectedItems[i]).Row.ItemArray[0].ToString()));
            return SelectedPCs;
        }

    }

    class PCsManager
    {
        DataSet ResultOfSQL;
        PCGridManager GridManager;
        SqlDataAdapter dAdapt = new SqlDataAdapter("", ConnectionManager.ConnectString);

        public PCsManager(DataGrid datagrid)
        {
           
            ResultOfSQL = new DataSet("PCs");
            GridManager = new PCGridManager(datagrid);
        }
        public void GetFullTable()
        {
            dAdapt.SelectCommand.CommandText = "SELECT * FROM PCs";
            GetTable();
        }
        private void GetTable()
        {   
            ResultOfSQL.Clear();
            try
            {
                dAdapt.Fill(ResultOfSQL, "PCs");
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
            dAdapt.SelectCommand.CommandText ="SELECT PCs.InventoryNumber, PCs.FrequencyOfCore,PCs.[Type], PCs.SSD FROM PCs LEFT OUTER JOIN WorkSpaces ON PCs.InventoryNumber = WorkSpaces.PCInventoryNumber WHERE WorkSpaces.Id IS NULL";
            GetTable();
        }
        public void Add(PC tmp)
        {
            string querry = "INSERT INTO PCs VALUES ("+"N'"+tmp.InventoryNumber.ToString()+"'"+","+"N'"+tmp.Type+"'"+", @FrequencyOfCore, @SSD)";
            using (var Connection = new SqlConnection(ConnectionManager.ConnectString))
            {
                SqlCommand ResultQuerry = new SqlCommand(querry,Connection);
                ResultQuerry.Parameters.Add("@FrequencyOfCore", SqlDbType.Float);
                ResultQuerry.Parameters["@FrequencyOfCore"].Value = tmp.FrequencyOfCore;
                ResultQuerry.Parameters.Add("@SSD", SqlDbType.Bit);
                ResultQuerry.Parameters["@SSD"].Value = tmp.SSD;
                try
                {
                    Connection.Open();
                    ResultQuerry.ExecuteScalar();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                GetFullTable();
            }
        }

        private void Execute(string query)
        {
            using (var Connection = new SqlConnection(ConnectionManager.ConnectString))
            {
                SqlCommand ResultQuerry = new SqlCommand(query, Connection);
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
            string Query = "DELETE FROM PCs WHERE [InventoryNumber] = " + "'"+id.ToString()+ "'";
            Execute(Query);
        }
        public void DeleteSelectedItems(WorkSpacesManager WSM)
        {
            var Source = GridManager.GetSelectedItems();
            for (int i = 0; i < Source.Count; i++)
            {
                Delete(Source[i]);
                if((WSM != null)&&(WSM.GetConnectedWorkSpace(Source[i]) != Guid.Empty))
                    WSM.DisconnectPC(WSM.GetConnectedWorkSpace(Source[i]));
            }
        }
        public string MakeQuery(PCFilter tmp)
        {
            string Result= "SELECT *  FROM PCs WHERE PATINDEX(" + "N'%"+tmp.type+"%'"+",[Type]) > 0  AND [FrequencyOfCore] >= " + tmp.minfreq+ " AND [FrequencyOfCore] <= "+tmp.maxfreq;
            if (!tmp.notssd)
                if(tmp.ssd==true)
                    Result += " AND [SSD] = " + 1;
                else
                {
                    Result += " AND [SSD] = " + 0;
                }
            return Result;
        }
        public void Filter(string type,string fromfreq,string tofreq,bool? ssdcheck,bool? notssdcheck)
        {
            var Filter = new PCFilter(type, fromfreq, tofreq, ssdcheck, notssdcheck);
            string Query = MakeQuery(Filter);         
               dAdapt.SelectCommand.CommandText = Query;
            GetTable();
        }
    }

    class PCFilter
    {
        public string type;
        public double minfreq;
        public double maxfreq;
        public bool ssd;
        public bool notssd;

        public PCFilter(string type, string fromfreq, string tofreq, bool? ssdcheck, bool? notssdcheck)
        {
            this.type = type;
            if (fromfreq != "")
                minfreq = Convert.ToDouble(fromfreq);
            else
                minfreq = Double.MinValue;
            if (tofreq != "")
                maxfreq = Convert.ToDouble(tofreq);
            else
                maxfreq =Double.MaxValue;
            ssd = (bool)ssdcheck;
            notssd = (bool)notssdcheck;
        }
    }
}
