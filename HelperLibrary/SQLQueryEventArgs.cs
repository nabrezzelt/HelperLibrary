using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Database
{
    public class SQLQueryEventArgs : EventArgs
    {
        private string _query;
        private QueryResult _result;

        public string Query { get => _query; set => _query = value; }
        public QueryResult Result { get => _result; set => _result = value; }

        public SQLQueryEventArgs(string query, QueryResult result)
        {
            Query = query;
            Result = result;            
        }

        public enum QueryResult
        {            
            Succeeded,
            Failed
        }
    }
}
