using System;

namespace HelperLibrary.Database
{
    public class SqlQueryEventArgs : EventArgs
    {
        public string Query;

        public QueryType Type;

        public SqlQueryEventArgs(string query, QueryType type)
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
