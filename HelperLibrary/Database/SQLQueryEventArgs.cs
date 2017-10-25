using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Database
{
    public class SQLQueryEventArgs : EventArgs
    {
        public string Query;

        public QueryType Type;

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
