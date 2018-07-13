using System;
using System.Dynamic;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class SqlQueryFailedException : Exception
    {
        public string SqlQuery { get; }

        public string CallerMethodName { get; }
        
        public int CallerLineNumber { get; }

        public SqlQueryFailedException() { }

        public SqlQueryFailedException(string message, string sqlQuery, [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = null) : base(message)
        {
            SqlQuery = sqlQuery;
            CallerMethodName = callerMethodName;
            CallerLineNumber = callerLineNumber;
        }

        public SqlQueryFailedException(string message, string sqlQuery, Exception inner,
            [CallerLineNumber] int callerLineNumber = 0, [CallerMemberName] string callerMethodName = null) : base(message, inner)
        {
            SqlQuery = sqlQuery;
            CallerMethodName = callerMethodName;
            CallerLineNumber = callerLineNumber;
        }
    }
}
