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

    class WorkSpaceGridManager
    {
 
        public DataGrid Grid;

        public WorkSpaceGridManager(DataGrid grid)
        {
            Grid = grid;
        }


        public List<Guid> GetSelectedItems()
        {
            List<Guid> SelectedWorkSpaces = new List<Guid>();
            for (int i = 0; i < Grid.SelectedItems.Count; i++)
                SelectedWorkSpaces.Add(new Guid(((DataRowView)Grid.SelectedItems[i]).Row.ItemArray[0].ToString()));
            return SelectedWorkSpaces;
        }

    }
     public class WorkSpacesManager
     {
        DataSet ResultOfSQL;
        WorkSpaceGridManager GridManager;
        SqlDataAdapter dAdapt = new SqlDataAdapter("", ConnectionManager.ConnectString);
        bool monflag;
        bool pcflag;

        public bool MonFlag
        {
            get
            {
                return monflag;
            }
            set
            {
                monflag = value;
                GetFullTable();
            }
        }

        public bool PCFlag
        {
            get
            {
                return pcflag;
            }
            set
            {
                pcflag = value;
                GetFullTable();
            }
        }

        public DataGrid OG
        {
            get
            { return GridManager.Grid; }
            set
            {
                GridManager = new WorkSpaceGridManager(value);
            }
        }
       


        public WorkSpacesManager(DataGrid datagrid)
        {
            ResultOfSQL = new DataSet("WorkSpaces");
            GridManager = new WorkSpaceGridManager(datagrid);
            MonFlag = false;
            PCFlag = false;
        }

        public WorkSpacesManager()
        {
            ResultOfSQL = new DataSet("WorkSpaces");
           
        }


        public void GetFullTableSimple()
        {
            dAdapt.SelectCommand.CommandText = "SELECT * FROM WorkSpaces";
            GetTable();
        }

        public  void GetFullTable()
        {
            if (MonFlag && (!PCFlag))
                GetTableWithMon();
            else
                if (MonFlag && PCFlag)
                GetTableWithMonAndPc();
            else
                if (PCFlag && (!MonFlag))
                GetTableWithPc();
            else
                if ((!PCFlag) && (!MonFlag))
                GetFullTableSimple();
        }


        private void GetTable()
        {
            ResultOfSQL.Tables.Clear();
            ResultOfSQL.Clear();
            try
            {
                 dAdapt.Fill(ResultOfSQL, "WorkSpaces");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
                return;
            }
            GridManager.Grid.ItemsSource = ResultOfSQL.Tables[0].DefaultView;   
        }

        public void GetTableWithMonAndPc()
        {
             dAdapt.SelectCommand.CommandText = "SELECT  WS.[Id], WS.[EthernetPortNumber], WS.[TelephonePortNumber], " +
"coalesce(PC.InventoryNumber, '00000000-0000-0000-0000-000000000000') AS PC_Id, coalesce(PC.[Type],'-') AS PC_Type, coalesce(CAST(PC.FrequencyOfCore AS CHAR), '-') AS PC_Frquency, coalesce(CAST(PC.SSD AS CHAR), '-') AS SSD, " +
"coalesce(Mon.InventoryNumber, '00000000-0000-0000-0000-000000000000') AS Monitor_Id, coalesce(CAST(Mon.Size AS CHAR), '-') AS Monitor_Size,  coalesce( Mon.Matrix, '-') AS Mon_Matrix " +
"FROM WorkSpaces AS WS " +
"LEFT OUTER JOIN PCs AS PC ON WS.PCInventoryNumber = PC.InventoryNumber " +
"LEFT OUTER JOIN Monitors AS Mon ON WS.MonitorInventoryNumber = Mon.InventoryNumber";
            GetTable();
        }

        public void GetTableWithMon()
        {
             dAdapt.SelectCommand.CommandText = "SELECT  WS.[Id], WS.[EthernetPortNumber], WS.[TelephonePortNumber], " +
"coalesce(Mon.InventoryNumber, '00000000-0000-0000-0000-000000000000') AS Monitor_Id, coalesce(CAST(Mon.Size AS CHAR), '-') AS Monitor_Size, coalesce( Mon.Matrix, '-') AS Mon_Matrix " +
"FROM WorkSpaces AS WS " +
"LEFT OUTER JOIN Monitors AS Mon ON WS.MonitorInventoryNumber = Mon.InventoryNumber";
            GetTable();
        }

        public void GetTableWithPc()
        {
             dAdapt.SelectCommand.CommandText = "SELECT  WS.[Id], WS.[EthernetPortNumber], WS.[TelephonePortNumber], " +
"coalesce(PC.InventoryNumber, '00000000-0000-0000-0000-000000000000') AS PC_Id, coalesce(PC.[Type],'-') AS PC_Type, coalesce(CAST(PC.FrequencyOfCore AS CHAR), '-') AS PC_Frquency, coalesce(CAST(PC.SSD AS CHAR), '-') AS SSD " +
"FROM WorkSpaces AS WS " +
"LEFT OUTER JOIN PCs AS PC ON WS.PCInventoryNumber = PC.InventoryNumber";
            GetTable();
        }


        public void Add(WorkSpace tmp)
        {
            string querry = "INSERT INTO WorkSPaces VALUES (@Id,"  + tmp.EthernetPortNumber +','+ tmp.TelephonePortNumber + ',' + "'" + tmp.PCInventoryNumber.ToString() +"'"+ ','+"'"+ tmp.PCInventoryNumber.ToString()+"')";
            using (var Connection = new SqlConnection(ConnectionManager.ConnectString))
            {
                SqlCommand ResultQuerry = new SqlCommand(querry, Connection);
                ResultQuerry.Parameters.Add("@Id", SqlDbType.NChar);
                ResultQuerry.Parameters["@Id"].Value = tmp.Id.ToString();
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

        public void SetPCConnection(Guid Id, Guid PCid)
        {
            string querry = "Update WorkSpaces SET PCInventoryNumber = '"+PCid.ToString()+"' WHERE Id = '"+Id.ToString()+"'";
            Execute(querry);
        }
        public Guid GetConnectedWorkSpace(Guid Id)
        {
            string query = "SELECT Id FROM WorkSpaces WHERE PCInventoryNumber = '" + Id.ToString() + "' OR MonitorInventoryNumber = '" + Id.ToString() + "'";
            dAdapt.SelectCommand.CommandText = query;
            var ds = new DataSet("ConnectedWorkSpace");
            ds.Clear();
            dAdapt.Fill(ds, "ConnectedWorkSpace");
            if (ds.Tables[0].DefaultView.Table.Rows.Count > 0)
                return new Guid(ds.Tables[0].DefaultView.Table.Rows[0].ItemArray[0].ToString());
            else
                return Guid.Empty;
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
            }
            if(GridManager.Grid != null)
                GetFullTable();
        }
        public void  DisconnectPC(Guid Id)
        {
            string querry = "Update WorkSpaces SET PCInventoryNumber = '" + Guid.Empty.ToString() + "' WHERE Id = '" + Id.ToString() + "'";
            Execute(querry);
        }
        public void DisconnectMon(Guid Id)
        {
            string querry = "Update WorkSpaces SET MonitorInventoryNumber = '" + Guid.Empty.ToString() + "' WHERE Id = '" + Id.ToString() + "'";
            Execute(querry);

        }
        public void SetMonConnection(Guid Id, Guid Monid)
        {
            string querry = "Update WorkSpaces SET MonitorInventoryNumber = '" + Monid.ToString() + "' WHERE Id = '" + Id.ToString() + "'";
            Execute(querry);
        }

        public void Delete(Guid id)
        {
            string Query = "DELETE FROM WorkSpaces WHERE [Id] = " + "'" + id.ToString() + "'";
            Execute(Query);
        }
        
        public List<Guid> GetSelectedId()
        {
            List<Guid> SelectedWorkSpaces = new List<Guid>();
            for (int i = 0; i < GridManager.Grid.SelectedItems.Count; i++)
                SelectedWorkSpaces.Add(new WorkSpace(GridManager.Grid.SelectedItems[i] as WorkSpace).Id);
            return SelectedWorkSpaces;
        }

        public void DeleteSelectedItems()
        {
            List<Guid> Source = GridManager.GetSelectedItems();
            for (int i = 0; i < Source.Count; i++)
                Delete(Source[i]);
        }


     }
}
