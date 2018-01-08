using System;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{
    [Serializable]
    public class PreparedStatementNotFoundException : Exception
    {
        public PreparedStatementNotFoundException() { }
        public PreparedStatementNotFoundException(string message) : base(message) { }
        public PreparedStatementNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        protected PreparedStatementNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}