using System;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class SQLQueryFailException : Exception
    {
        public string SQLQuery { get; }

        public SQLQueryFailException() { }

        public SQLQueryFailException(string message, string sqlQuery) : base(message)
        {
            SQLQuery = sqlQuery;
        }

        public SQLQueryFailException(string message, string sqlQuery, Exception inner) : base(message, inner)
        {
            SQLQuery = sqlQuery;
        }
    }
}
