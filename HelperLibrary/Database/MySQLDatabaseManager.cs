using HelperLibrary.Database.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace HelperLibrary.Database
{
    public class MySqlDatabaseManager
    {
        #region Singleton/InstanceManagement
        public const string DefaultInstanceName = "Default";
        private static readonly Dictionary<string, MySqlDatabaseManager> Instances = new Dictionary<string, MySqlDatabaseManager>();

        public static MySqlDatabaseManager GetInstance(string instanceName)
        {
            return GetInstanceByName(instanceName) ?? throw new InstanceAlreadyExistsException($"Instance with name {instanceName} not found!");
        }

        public static MySqlDatabaseManager GetInstance()
        {
            return GetInstance(DefaultInstanceName);
        }

        private static MySqlDatabaseManager GetInstanceByName(string instanceName)
        {
            foreach (KeyValuePair<string, MySqlDatabaseManager> instance in Instances)
            {
                if (instance.Key == instanceName)
                {
                    return instance.Value;
                }
            }

            return null;
        }

        public static void CreateInstance(string instanceName)
        {
            if (GetInstanceByName(instanceName) != null)
                throw new InstanceAlreadyExistsException($"Instance with name {instanceName} already exists.");

            Instances.Add(instanceName, new MySqlDatabaseManager());
        }

        public static void CreateInstance()
        {
            CreateInstance(DefaultInstanceName);
        }
        #endregion

        private readonly MySqlConnection _connection;
        private MySqlCommand _prepareSqlCommand;
        private Dictionary<string, object> _bindedParams = new Dictionary<string, object>();

        public event EventHandler<SqlQueryEventArgs> SqlQueryExecuted;
        public event EventHandler ConnectionSuccessful;

        private bool _isConnectionStringSet;

        private MySqlDatabaseManager()
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
            MySqlConnectionStringBuilder connectionBuilder = new MySqlConnectionStringBuilder
            {
                Server = host,
                UserID = user,
                Password = password,
                Database = database
            };

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
                throw new MissingConnectionStringException("ConnectionString not set. Please use MySqlDatabaseManager.SetConnectionString() before.");
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

        ~MySqlDatabaseManager()
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
            _prepareSqlCommand = _connection.CreateCommand();
            _prepareSqlCommand.CommandText = query;
            _prepareSqlCommand.Prepare();
        }

        /// <summary>
        /// Bindes a Value to the prepared Query.
        /// </summary>
        /// <param name="parameterName">Parameter that binds the Value.</param>
        /// <param name="value">Value to bind.</param>
        /// <exception cref="QueryNotPreparedException" />
        public void BindValue(string parameterName, object value)
        {
            if (_prepareSqlCommand != null)
            {
                _prepareSqlCommand.Parameters.AddWithValue(parameterName, value);

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
        /// <exception cref="SqlQueryFailedException" />
        /// <exception cref="QueryNotPreparedException" />
        public MySqlDataReader ExecutePreparedSelect()
        {
            if (_prepareSqlCommand == null) throw new QueryNotPreparedException();

            try
            {
                MySqlDataReader reader = _prepareSqlCommand.ExecuteReader();
                SqlQueryExecuted?.Invoke(this, new SqlQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SqlQueryEventArgs.QueryType.PreparedSelect));

                return reader;
            }
            catch (Exception e)
            {
                throw new SqlQueryFailedException("Query failed!", ReplacePlaceholderInPreparedQuery(), e);
            }
        }

        /// <summary>
        /// Executes a Prepared statement.
        /// </summary>
        /// <returns>Result of the Query.</returns>
        /// <exception cref="SqlQueryFailedException" />
        /// <exception cref="QueryNotPreparedException" />
        public void ExecutePreparedInsertUpdateDelete()
        {
            if (!IsConnected())
            {
                Connect();
            }

            if (_prepareSqlCommand != null)
            {
                try
                {
                    _prepareSqlCommand.ExecuteNonQuery();
                    SqlQueryExecuted?.Invoke(this, new SqlQueryEventArgs(ReplacePlaceholderInPreparedQuery(), SqlQueryEventArgs.QueryType.PreparedInsertUpdateDelete));
                }
                catch (MySqlException e)
                {
                    throw new SqlQueryFailedException("SQL-Query failed", ReplacePlaceholderInPreparedQuery(), e);
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
        /// <exception cref="SqlQueryFailedException" />
        public void InsertUpdateDelete(string query)
        {
            if (!IsConnected())
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

        /// <summary>
        /// Gets the last ID of the auto_increment value.
        /// </summary>
        /// <returns>Last ID</returns>
        public int GetLastId()
        {
            if (!IsConnected())
            {
                Connect();
            }

            MySqlDataReader reader = Select("SELECT LAST_INSERT_ID()");
            reader.Read();

            var id = reader.GetInt32(0);

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
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            catch (ArgumentException ex)
            {
                throw new CouldNotConnectException("Connection Failed!", ex);
            }
            catch (MySqlException ex)
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
            string bindedQuery = _prepareSqlCommand.CommandText;

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

                if (entry.Value is double value)
                {
                    return bindedQuery.Replace(entry.Key, value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
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
