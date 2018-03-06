using System;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class SqlQueryFailedException : Exception
    {
        public string SqlQuery { get; }

        public SqlQueryFailedException() { }

        public SqlQueryFailedException(string message, string sqlQuery) : base(message)
        {
            SqlQuery = sqlQuery;
        }

        public SqlQueryFailedException(string message, string sqlQuery, Exception inner) : base(message, inner)
        {
            SqlQuery = sqlQuery;
        }
    }
}
