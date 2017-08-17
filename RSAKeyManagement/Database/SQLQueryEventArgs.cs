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
        private QueryType _type;

        public string Query { get => _query; set => _query = value; }
        public QueryType Type { get => _type; set => _type = value; }

        public SQLQueryEventArgs(string query, QueryType type)
        {
            Query = query;
            Type = type;            
        }

        public enum QueryType
        {            
            Select,
            InsertUpdateDelete,
            PreparedSelect,
            PreparedInsertUpdateDelete
        }
    }
}
