using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SQLGen.Views;
using System.ComponentModel;
using System.Data;
using SQLGen.Models;
namespace SQLGen.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        DataTable _dt;
        Connection _conn;
        string _password;
        string _targetSchema;
        string _sourceSchema;
        ConnectionViewModel _connectionViewModel;
        DBConnect dc;
        StringBuilder script;
        public MainWindowViewModel()
        {
            _targetSchema = "crate_app_history";
            script = new StringBuilder();
            _connectionViewModel = new ConnectionViewModel();
            _conn = new Connection() { DataContext = _connectionViewModel };
            ShowConnectionDialog();
            _password = _conn.pb_password.Password;
            Initialize();

        }

        private void Initialize()
        {
            _sourceSchema = _connectionViewModel.Database;
            dc = new DBConnect(_connectionViewModel.Server, _sourceSchema, _connectionViewModel.Username, _password);
            _dt = dc.GetTables(_connectionViewModel.Database);
            GenerateInsertTriggers();
        }


        private void ShowConnectionDialog()
        {
            Window con = new Window();
            con.SizeToContent = SizeToContent.WidthAndHeight;
            con.Content = _conn;
            con.ShowDialog();
        }

       public object DataTable
        {
            get { return _dt.DefaultView; }
        }

        private List<String> GetTableNames(DataTable dt)
       {
           List<string> tableNames = new List<string>();
           foreach (DataRow row in dt.Rows)
           {
               string name = row["TABLE_NAME"].ToString();
               tableNames.Add(name);

           }
           return tableNames;
       }

        private List<string> GetColumnNames(string table)
        {
            List<string> columnNames = new List<string>();
            DataTable columnsTable = dc.GetColumns(table);
            foreach (DataRow row in columnsTable.Rows)
            {
                string name = row["COLUMN_NAME"].ToString();
                columnNames.Add(name);
            }
            return columnNames;
        }

        private void GenerateInsertTriggers()
        {
           
            List<string> tableNames = GetTableNames(_dt);
            script.AppendLine("use "+ _sourceSchema + ";");
            script.AppendLine("delimiter //");
            foreach (string table in tableNames)
            {

                List<string> columns = GetColumnNames(table);
                script.AppendLine("DROP TRIGGER IF EXISTS trg_insert_"+table+" //");
                script.Append("CREATE TRIGGER trg_insert_");
                script.Append(table);
                script.Append(" AFTER INSERT ON ");
                script.Append(_sourceSchema + "." + table);
                script.AppendLine();
                script.AppendLine("FOR EACH ROW");
                script.AppendLine("BEGIN");
                script.Append("INSERT INTO " + _targetSchema + "." + table + "_history (");

                foreach (string column in columns)
                {
                    script.Append(column + ",");
                }
                script.AppendLine(" event_date, event_type, user)");
                script.Append("VALUES (");

                foreach (string column in columns)
                {
                    script.Append("new." + column + ",");
                }

                script.AppendLine(" utc_timestamp(), 'INSERT', CURRENT_USER() );");
                script.AppendLine("END //");
                

            }
                
        }

        public string Query
        {
            get { return script.ToString(); }
            set { }
        }
        

    }
}
