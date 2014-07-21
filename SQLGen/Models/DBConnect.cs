using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
namespace SQLGen.Models
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        public DBConnect(string server, string database, string username, string password)
        {
            this.server = server;
            this.database = database;
            this.username = username;
            this.password = password;
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "PORT=3306;" + "DATABASE=" +
            database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Close connection
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Insert statement
        public void Insert()
        {
        }

        //Update statement
        public void Update()
        {
        }

        //Delete statement
        public void Delete()
        {
        }

        //Select statement
        public DataTable GetTables(string schema)
        {
            DataTable allTables = new DataTable();
            if (this.OpenConnection() == true)
            {
                string defineSchema = "use " + schema + ";";
                string getTables = "SELECT * FROM information_schema.Tables where TABLE_SCHEMA = \'" + schema + "\';";
                MySqlCommand defineSchemaCmd = new MySqlCommand(defineSchema, connection);
                MySqlCommand getTablesCmd = new MySqlCommand(getTables, connection);
                //Create a data reader and Execute the command

                using (MySqlDataReader dataReader = getTablesCmd.ExecuteReader())
                {                    
                    allTables.Load(dataReader);
                    //Read the data and store them in the list
                    //close Data Reader
                    dataReader.Close();
                }
                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return allTables;
            }
            else
            {
                return allTables;
            }
        }

        public DataTable GetColumns(string table)
        {
            DataTable allColumns = new DataTable();
            if (this.OpenConnection() == true)
            {
                string getColumns = "SELECT * FROM information_schema.Columns where TABLE_NAME = \'" + table + "\';";
                MySqlCommand getTablesCmd = new MySqlCommand(getColumns, connection);
                //Create a data reader and Execute the command

                using (MySqlDataReader dataReader = getTablesCmd.ExecuteReader())
                {
                    allColumns.Load(dataReader);
                    //Read the data and store them in the list
                    //close Data Reader
                    dataReader.Close();
                }
                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return allColumns;
            }
            else
            {
                return allColumns;
            }
        }

        //Count statement
        public int Count()
        {
            return 0;
        }

        //Backup
        public void Backup()
        {
        }

        //Restore
        public void Restore()
        {
        }
    }
}
