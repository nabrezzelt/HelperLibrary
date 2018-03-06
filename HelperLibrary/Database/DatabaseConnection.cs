using System;
using System.Collections.Generic;
using System.Data;
using HelperLibrary.Database.Exceptions;
using MySql.Data.MySqlClient;

namespace HelperLibrary.Database
{
    public abstract class DatabaseConnection
    {
        private readonly Dictionary<int, MySqlCommand> _preparedStatements = new Dictionary<int, MySqlCommand>();

        private readonly MySqlConnection _connection;

        private readonly bool _autoPrepareAfterConnectionSuccessful;

        public event EventHandler<SqlQueryEventArgs> SqlQueryExecuted;
        public event EventHandler ConnectionSuccessful;

        public bool IsConnected => _connection?.State == ConnectionState.Open;

        protected DatabaseConnection(bool autoPrepareAfterConnectionSuccessful = true)
        {
            _connection = new MySqlConnection();

            ConnectionSuccessful += OnConnectionSuccessful;

            _autoPrepareAfterConnectionSuccessful = autoPrepareAfterConnectionSuccessful;
        }

        public void SetConnectionString(string host, string user, string password, string databaseName)
        {
            MySqlConnectionStringBuilder connectionBuilder = new MySqlConnectionStringBuilder
            {
                Server = host,
                UserID = user,
                Password = password,
                Database = databaseName
            };

            _connection.ConnectionString = connectionBuilder.ToString();            
        }

        private void OnConnectionSuccessful(object sender, EventArgs e)
        {
            if (_autoPrepareAfterConnectionSuccessful)
            {
                DoPrepareStatements();
            }
        }

        public void Connect()
        {
            try
            {
                _connection.Open();
                ConnectionSuccessful?.Invoke(this, new EventArgs());
            }
            catch (MySqlException e)
            {
                throw new CouldNotConnectException("Could not connect to Database!", e);
            }
        }

        public void InsertUpdateDelete(string query)
        {
            if (!IsConnected)
            {
                Connect();
            }

            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;


            try
            {
                cmd.ExecuteNonQuery();
                SqlQueryExecuted?.Invoke(this, new SqlQueryEventArgs(query, SqlQueryEventArgs.QueryType.InsertUpdateDelete));
            }
            catch (MySqlException e)
            {

                throw new SqlQueryFailedException("SQL-Query failed!", query, e);
            }
        }

        public MySqlDataReader Select(string query)
        {
            if (!IsConnected)
            {
                Connect();
            }

            MySqlDataReader reader;

            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            try
            {
                reader = cmd.ExecuteReader();
                SqlQueryExecuted?.Invoke(this, new SqlQueryEventArgs(query, SqlQueryEventArgs.QueryType.Select));
            }
            catch (MySqlException e)
            {
                throw new SqlQueryFailedException("SQL-Query failed!", query, e);
            }

            return reader;
        }

        public int GetLastId()
        {
            if (!IsConnected)
            {
                Connect();
            }

            MySqlDataReader reader = Select("SELECT LAST_INSERT_ID()");
            reader.Read();

            var id = reader.GetInt32(0);

            reader.Close();

            return id;
        }

        protected abstract void DoPrepareStatements();

        public void PrepareStatement(int index, string query)
        {
            var preparedStatement = _connection.CreateCommand();

            preparedStatement.CommandText = query;
            preparedStatement.Prepare();

            _preparedStatements.Add(index, preparedStatement);
        }

        public PreparedStatement GetPreparedStatement(int index)
        {
            return new PreparedStatement(GetPreparedStatementFormList(index), this);
        }

        private MySqlCommand GetPreparedStatementFormList(int index)
        {
            foreach (var preparedStatement in _preparedStatements)
            {
                if (preparedStatement.Key == index)
                    return preparedStatement.Value;
            }

            throw new PreparedStatementNotFoundException();
        }

        internal void InvokeSqlQueryExecuted(string query, SqlQueryEventArgs.QueryType queryType)
        {
            SqlQueryExecuted?.Invoke(this, new SqlQueryEventArgs(query, SqlQueryEventArgs.QueryType.PreparedInsertUpdateDelete));
        }
    }
}