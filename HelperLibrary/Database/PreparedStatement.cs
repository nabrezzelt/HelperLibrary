using System;
using System.Collections.Generic;
using HelperLibrary.Database.Exceptions;
using MySql.Data.MySqlClient;

namespace HelperLibrary.Database
{
    public class PreparedStatement
    {
        private readonly MySqlCommand _preparedStatement;
        private readonly DatabaseConnection _connection;
        private readonly Dictionary<string, object> _bindedParams;

        internal PreparedStatement(MySqlCommand preparedStatement, DatabaseConnection connection)
        {
            _bindedParams = new Dictionary<string, object>();

            _preparedStatement = preparedStatement;
            _connection = connection;
        }

        public void BindValue(string parameterName, object value)
        {
            _preparedStatement.Parameters.AddWithValue(parameterName, value);

            if (_bindedParams.ContainsKey(parameterName))
            {
                _bindedParams[parameterName] = value;
            }
            else
            {
                _bindedParams.Add(parameterName, value);
            }
        }

        public MySqlDataReader ExecuteSelect()
        {                        
            try
            {
                MySqlDataReader reader = _preparedStatement.ExecuteReader();
                _connection.InvokeSqlQueryExecuted(ReplacePlaceholderInPreparedQuery(), SqlQueryEventArgs.QueryType.PreparedSelect);

                return reader;
            }
            catch (Exception e)
            {
                throw new SqlQueryFailedException("Query failed!", ReplacePlaceholderInPreparedQuery(), e);
            }
        }

        public void ExecuteInsertUpdateDelete()
        {            
            try
            {
                _preparedStatement.ExecuteNonQuery();
                _connection.InvokeSqlQueryExecuted(ReplacePlaceholderInPreparedQuery(), SqlQueryEventArgs.QueryType.PreparedInsertUpdateDelete);
            }
            catch (MySqlException e)
            {
                throw new SqlQueryFailedException("SQL-Query failed", ReplacePlaceholderInPreparedQuery(), e);
            }
        }

        private string ReplacePlaceholderInPreparedQuery()
        {
            string bindedQuery = _preparedStatement.CommandText;

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