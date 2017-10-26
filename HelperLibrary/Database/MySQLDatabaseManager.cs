using HelperLibrary.Database.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace HelperLibrary.Database
{
    public class  MySQLDatabaseManager
    {
        #region Singleton
        private static MySQLDatabaseManager _instance;

        public static MySQLDatabaseManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new MySQLDatabaseManager();
            return _instance;
        }
        #endregion

        private readonly SqlConnection _connection;
        private SqlCommand _prepareSQLCommand;
        private Dictionary<string, object> _bindedParams = new Dictionary<string, object>();

        public delegate void OnSQLQueryExcecuted(object sender, SQLQueryEventArgs e);
        public delegate void OnConnectionSuccessful(object sender, EventArgs e);
        public event OnSQLQueryExcecuted SQLQueryExcecuted;
        public event OnConnectionSuccessful ConnectionSuccessful;

        private bool _isConnectionStringSet;

        private MySQLDatabaseManager()
        {
            _connection = new SqlConnection();
        }

        /// <summary>
        /// Sets the connection string to login at the database.
        /// </summary>
        /// <param name="host">Host-Address of the Database</param>
        /// <param name="user">Use^^rname</param>
        /// <param name="password">Password</param>
        /// <param name="database">Databasename</param>
        /// <param name="port">Port for Databaseconnection (Default: 3306)</param>
        public void SetConnectionString(string host, string user, string password, string database, int port = 3306)
        {
            string connectionString = "Server=" + host + ";" +
                                      "Port=" + port + ";" +
                                      "Database=" + database + ";" +
                                      "uid=" + user + ";" +
                                      "pwd=" + password + ";";

            _connection.ConnectionString = connectionString;
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
            catch (SqlException e)
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
            _bindedParams = new Dictionary<string, object>();
            _prepareSQLCommand = _connection.CreateCommand();
            _prepareSQLCommand.CommandText = query;
            _prepareSQLCommand.Prepare();
        }

        /// <summary>
        /// Bindes a Value to the prepared Query.
        /// </summary>
        /// <param name="parameterName">Parameter that binds the Value.</param>
        /// <param name="value">Value to bind.</param>
        /// <exception cref="QueryNotPreparedException" />
        public void BindValue(string parameterName, object value)
        {
            if (_prepareSQLCommand != null)
            {
                _prepareSQLCommand.Parameters.AddWithValue(parameterName, value);

                if (_bindedParams.ContainsKey(parameterName))
                {
                    _bindedParams[parameterName] = value;
                }
                else
                {
                    _bindedParams.Add(parameterName, value);
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
        public SqlDataReader ExecutePreparedSelect()
        {
            if (_prepareSQLCommand == null) throw new QueryNotPreparedException();

            try
            {
                SqlDataReader reader = _prepareSQLCommand.ExecuteReader();
                SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SQLQueryEventArgs.QueryType.PreparedInsertUpdateDelete));

                return reader;
            }
            catch (Exception e)
            {
                throw new SQLQueryFailException("Query failed!", ReplacePlaceholderInPreparedQuery(), e);
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

            if (_prepareSQLCommand != null)
            {
                try
                {
                    _prepareSQLCommand.ExecuteNonQuery();
                    SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SQLQueryEventArgs.QueryType.PreparedInsertUpdateDelete));
                }
                catch (SqlException e)
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

            SqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;


            try
            {
                cmd.ExecuteNonQuery();
                SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryType.InsertUpdateDelete));
            }
            catch (SqlException e)
            {

                throw new SQLQueryFailException("SQL-Query failed!", query, e);
            }
        }

        /// <summary>
        /// Executes a given SQL-Query that return Rows.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public SqlDataReader Select(string query)
        {
            if (!IsConnected())
            {
                Connect();
            }

            SqlDataReader reader;

            SqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            try
            {
                reader = cmd.ExecuteReader();
                SQLQueryExcecuted?.Invoke(this, new SQLQueryEventArgs(query, SQLQueryEventArgs.QueryType.Select));
            }
            catch (SqlException e)
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

            SqlDataReader reader = _instance.Select("SELECT LAST_INSERT_ID()");
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
        /// <param name="host">Host-Address of the Database</param>
        /// <param name="database">Database-Name</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">Port for Databaseconnection (Default: 3306)</param>
        /// <returns>True if the Connectionw as successful or throw a <see cref="CouldNotConnectException"/>.</returns>
        /// <exception cref="CouldNotConnectException" />
        public static bool TestDatabaseConnection(string host, string user, string password, string database, int port = 3306)
        {
            var connectionString = "Server=" + host + ";" +
                                   "Port=" + port + ";" +
                                   "Database=" + database + ";" +
                                   "uid=" + user + ";" +
                                   "pwd=" + password + ";";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            catch (ArgumentException ex)
            {
                throw new CouldNotConnectException("Connection Failed!", ex);
            }
            catch (SqlException ex)
            {                
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042:
                        // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        throw new CouldNotConnectException("Unable to connect to any of the specified MySQL hosts (Check Server, Port).", ex);
                    case 0:
                        // Access denied (Check DB name,username,password)
                        throw new CouldNotConnectException("Access denied (Check Databasename, Username and Password).", ex);
                    default:
                        throw new CouldNotConnectException("Connection Failed!", ex);
                }
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return true;
        }

        public bool IsConnected()
        {
            return _connection?.State == ConnectionState.Open;
        }

        /// <summary>
        /// Binds a Values in the Query to log a prepared SQL-Query.
        /// </summary>
        /// <returns>Binded SQL-Query.</returns>
        private string ReplacePlaceholderInPreparedQuery()
        {
            string bindedQuery = _prepareSQLCommand.CommandText;

            foreach (KeyValuePair<string, object> entry in _bindedParams)
            {
                if (entry.Value is int)
                {
                    return bindedQuery.Replace(entry.Key, entry.Value.ToString());
                }

                if (entry.Value is string)
                {
                    return bindedQuery.Replace(entry.Key, "\"" + entry.Value + "\"");
                }

                if (entry.Value is double)
                {
                    return bindedQuery.Replace(entry.Key, ((double)entry.Value).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                }

                if (entry.Value is DateTime val)
                {
                    return bindedQuery.Replace(entry.Key, "\"" + val.ToString("yyyy-MM-dd hh:mm:ss") + "\"");
                }
            }

            return bindedQuery;
        }
    }
}
