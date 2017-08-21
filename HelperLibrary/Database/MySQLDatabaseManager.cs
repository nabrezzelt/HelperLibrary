using HelperLibrary.Database.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace HelperLibrary.Database
{
    public class MySQLDatabaseManager
    {
        #region Singleton
        private static MySQLDatabaseManager _instance;

        public static MySQLDatabaseManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                _instance = new MySQLDatabaseManager();
                return _instance;
            }
        }
        #endregion

        private MySqlConnection _connection;
        private bool _isConnectionStringSet = false;
        private MySqlCommand prepareSQLCommand;
        private Dictionary<string, object> bindedParams = new Dictionary<string, object>();

        public delegate void OnSQLQueryExcecuted(object sender, SQLQueryEventArgs e);
        public delegate void OnConnectionSuccessful(object sender, EventArgs e);
        public event OnSQLQueryExcecuted SQLQueryExcecuted;
        public event OnConnectionSuccessful ConnectionSuccessful;

        public bool IsConnectionStringSet { get => _isConnectionStringSet; }

        private MySQLDatabaseManager()
        {
            _connection = new MySqlConnection();
        }

        /// <summary>
        /// Sets the connection string to login at the database.
        /// </summary>
        /// <param name="host">Host-Adress of the Database</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Databasename</param>
        public void SetConnectionString(string host, string user, string password, string database)
        {
            MySqlConnectionStringBuilder connectionBuilder = new MySqlConnectionStringBuilder();

            connectionBuilder.Server = host;
            connectionBuilder.UserID = user;
            connectionBuilder.Password = password;
            connectionBuilder.Database = database;

            _connection.ConnectionString = connectionBuilder.ToString();
            _isConnectionStringSet = true;
        }

        /// <summary>
        /// Connects to the Database.
        /// </summary>
        /// <exception cref="MissingConnectionStringException" />
        /// <exception cref="CouldNotConnectException" />
        public void Connect()
        {
            if (!_isConnectionStringSet)
            {
                throw new MissingConnectionStringException("ConnectionString not set. Please use MySQLDatabaseManager.SetConnectionString() before .");
            }

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

        ~MySQLDatabaseManager()
        {
            if (IsConnected())
                _connection.Close();
        }

        /// <summary>
        /// Prepares a Query for execution.
        /// </summary>
        /// <param name="query">SQL-Query to prepare.</param>
        public void PrepareQuery(string query)
        {
            bindedParams = new Dictionary<string, object>();
            prepareSQLCommand = _connection.CreateCommand();
            prepareSQLCommand.CommandText = query;
            prepareSQLCommand.Prepare();
        }

        /// <summary>
        /// Bindes a Value to the prepared Query.
        /// </summary>
        /// <param name="parameterName">Parameter that binds the Value.</param>
        /// <param name="value">Value to bind.</param>
        /// <exception cref="QueryNotPreparedException" />
        public void BindValue(string parameterName, object value)
        {
            if (prepareSQLCommand != null)
            {
                prepareSQLCommand.Parameters.AddWithValue(parameterName, value);

                if (bindedParams.ContainsKey(parameterName))
                {
                    bindedParams[parameterName] = value;
                }
                else
                {
                    bindedParams.Add(parameterName, value);
                }
            }
            else
            {
                throw new QueryNotPreparedException();
            }
        }

        /// <summary>
        /// Executes a Prepared statement.
        /// </summary>
        /// <returns>Result of the Query.</returns>
        /// <exception cref="SQLQueryFailException" />
        /// <exception cref="QueryNotPreparedException" />
        public MySqlDataReader ExecutePreparedSelect()
        {
            if (prepareSQLCommand != null)
            {
                try
                {
                    MySqlDataReader reader = prepareSQLCommand.ExecuteReader();
                    SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SQLQueryEventArgs.QueryType.PreparedInsertUpdateDelete));

                    return reader;
                }
                catch (Exception e)
                {
                    throw new SQLQueryFailException("Query failed!", ReplacePlaceholderInPreparedQuery(), e);
                }
            }
            else
            {
                throw new QueryNotPreparedException();
            }
        }

        /// <summary>
        /// Executes a Prepared statement.
        /// </summary>
        /// <returns>Result of the Query.</returns>
        /// <exception cref="SQLQueryFailException" />
        /// <exception cref="QueryNotPreparedException" />
        public void ExecutePreparedInsertUpdateDelete()
        {
            if (!IsConnected())
            {
                Connect();
            }

            if (prepareSQLCommand != null)
            {
                try
                {
                    prepareSQLCommand.ExecuteNonQuery();
                    SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SQLQueryEventArgs.QueryType.PreparedInsertUpdateDelete));
                }
                catch (MySqlException e)
                {
                    throw new SQLQueryFailException("SQL-Query failed", ReplacePlaceholderInPreparedQuery(), e);
                }
            }
            else
            {
                throw new QueryNotPreparedException();
            }
        }

        /// <summary>
        /// Executes a given SQL-Query that return no Rows.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="SQLQueryFailException" />
        public void InsertUpdateDelete(string query)
        {
            if (!IsConnected())
            {
                Connect();
            }

            MySqlCommand cmd;
            cmd = _connection.CreateCommand();
            cmd.CommandText = query;


            try
            {
                cmd.ExecuteNonQuery();
                SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryType.InsertUpdateDelete));
            }
            catch (MySqlException e)
            {

                throw new SQLQueryFailException("SQL-Query failed!", query, e);
            }
        }

        /// <summary>
        /// Executes a given SQL-Query that return Rows.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public MySqlDataReader Select(string query)
        {
            if (!IsConnected())
            {
                Connect();
            }

            MySqlDataReader reader = null;
            MySqlCommand cmd;

            cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            try
            {
                reader = cmd.ExecuteReader();
                SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryType.Select));
            }
            catch (MySqlException e)
            {
                throw new SQLQueryFailException("SQL-Query failed!", query, e);
            }

            return reader;
        }

        /// <summary>
        /// Gets the last ID of the auto_increment value.
        /// </summary>
        /// <returns>Last ID</returns>
        public int GetLastID()
        {
            if (!IsConnected())
            {
                Connect();
            }

            MySqlDataReader reader = _instance.Select("SELECT LAST_INSERT_ID()");
            reader.Read();

            int id = reader.GetInt32(0);

            reader.Close();

            return id;
        }

        /// <summary>
        /// Escapes a string to insert it in Database-Querys and have a protection against SQL-Injections.
        /// </summary>
        /// <param name="queryParam">Parameter to escape</param>
        /// <returns>Escaped Parameter</returns>
        public static string EscapeString(string queryParam)
        {
            return Regex.Replace(queryParam, @"[\x00'""\b\n\r\t\cZ\\%]",
                delegate (Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }

        /// <summary>
        /// Test if connection is possible with given login-data.
        /// </summary>
        /// <param name="server">Address of Database</param>
        /// <param name="database">Database-Name</param>
        /// <param name="user">Username</param>
        /// <param name="passwd">Password</param>
        /// <returns>True if the Connectionw as successful or throw a <see cref="CouldNotConnectException"/>.</returns>
        /// <exception cref="CouldNotConnectException" />
        public static bool TestDatabaseConnection(string server, string database, string user, string passwd)
        {
            var connectionString = "Server=" + server + ";Port=3306;Database=" + database + ";uid=" + user + ";pwd=" + passwd + ";";
            bool isConnected = false;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                isConnected = true;
            }
            catch (ArgumentException ex)
            {
                throw new CouldNotConnectException("Connection Failed!", ex);
            }
            catch (MySqlException ex)
            {
                isConnected = false;
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042:
                        // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        throw new CouldNotConnectException("Unable to connect to any of the specified MySQL hosts (Check Server,Port).", ex);
                        break;
                    case 0:
                        // Access denied (Check DB name,username,password)
                        throw new CouldNotConnectException("Access denied (Check Databasename, Username and Password).", ex);
                        break;
                    default:
                        throw new CouldNotConnectException("Connection Failed!", ex);
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return isConnected;
        }

        public bool IsConnected()
        {
            if (_connection != null)
                return (_connection.State == System.Data.ConnectionState.Open);

            return false;
        }

        /// <summary>
        /// Binds a Values in the Query to log a prepared SQL-Query.
        /// </summary>
        /// <returns>Binded SQL-Query.</returns>
        private string ReplacePlaceholderInPreparedQuery()
        {
            string bindedQuery = prepareSQLCommand.CommandText;

            foreach (KeyValuePair<string, object> entry in bindedParams)
            {
                if (entry.Value is int)
                {
                    bindedQuery.Replace(entry.Key, entry.Value.ToString());
                }
                else if (entry.Value is string)
                {
                    bindedQuery.Replace(entry.Key, "\"" + entry.Value.ToString() + "\"");
                }
                else if (entry.Value is double)
                {
                    bindedQuery.Replace(entry.Key, ((double)entry.Value).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (entry.Value is DateTime)
                {
                    DateTime val = (DateTime)entry.Value;
                    bindedQuery.Replace(entry.Key, "\"" + (val.ToString("yyyy-MM-dd hh:mm:ss")) + "\"");
                }
            }

            return bindedQuery;
        }
    }
}
