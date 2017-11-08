using System;

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
