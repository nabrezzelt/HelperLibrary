using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace HelperLibrary.Database
{
    public class MySQLDatabaseManager
    {
        #region Singleton
        private static MySQLDatabaseManager _instance;

        public static MySQLDatabaseManager GetInstance()
        {
            if (_instance != null)
                return _instance;
            else
                return new MySQLDatabaseManager();
        }
        #endregion

        private MySqlConnection _connection;
        private bool _isConnectionStringSet = false;

        public delegate void SQLQueryExcecuted(object sender, SQLQueryEventArgs e);
        public event SQLQueryExcecuted OnSQLQueryExcecuted;

        public bool IsConnectionStringSet { get => _isConnectionStringSet; set => _isConnectionStringSet = value; }

        private MySQLDatabaseManager()
        {
            _connection = new MySqlConnection();
        }

        public void SetConnectionString(string host, string user, string password, string database)
        {
            _connection.ConnectionString = "server=" + host + ";uid=" + user + ";password=" + password + ";database=" + database + ";";
            _isConnectionStringSet = true;
        }

        public void Connect()
        {
            if(!_isConnectionStringSet)
            {
                throw new Exception("ConnectionString not set. Please use MySQLDatabaseManager.SetConnectionString() first.");
            }

            _connection.Open();
        }

        ~MySQLDatabaseManager()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
                _connection.Close();
        }


        public MySqlDataReader Select(string query)
        {
            MySqlDataReader reader = null;
            MySqlCommand cmd;

            cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            reader = cmd.ExecuteReader();
            OnSQLQueryExcecuted(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryResult.Succeeded));

            return reader;
        }

        public void InsertUpdateDelete(string query)
        {
            MySqlCommand cmd;
            cmd = _connection.CreateCommand();
            cmd.CommandText = query;
           
            cmd.ExecuteNonQuery();
            OnSQLQueryExcecuted(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryResult.Succeeded));
        }

        public int GetLastID()
        {            
            if(_connection.State == System.Data.ConnectionState.Open)
            {
                MySqlDataReader reader = _instance.Select("SELECT LAST_INSERT_ID()");
                reader.Read();

                int id = reader.GetInt32(0);

                reader.Close();

                return id;
            }

            throw new Exception("Not connected!");
        }      
    }
}
