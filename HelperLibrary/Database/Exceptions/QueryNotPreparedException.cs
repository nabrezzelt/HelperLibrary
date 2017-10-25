using System;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class QueryNotPreparedException : Exception
    {
        public QueryNotPreparedException() { }
        public QueryNotPreparedException(string message) : base(message) { }
        public QueryNotPreparedException(string message, Exception inner) : base(message, inner) { }
        protected QueryNotPreparedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
