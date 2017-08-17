using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class SQLQueryFailException : Exception
    {
        private string _sqlQuery;

        public string SQLQuery { get => _sqlQuery; }

        public SQLQueryFailException() { }

        public SQLQueryFailException(string message, string sqlQuery) : base(message)
        {
            _sqlQuery = sqlQuery;
        }

        public SQLQueryFailException(string message, string sqlQuery, Exception inner) : base(message, inner)
        {
            _sqlQuery = sqlQuery;
        }
        protected SQLQueryFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        
    }
}
