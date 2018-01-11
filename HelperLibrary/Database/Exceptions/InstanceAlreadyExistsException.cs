using System;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{
    [Serializable]
    public class InstanceAlreadyExistsException : Exception
    {
        public InstanceAlreadyExistsException() { }
        public InstanceAlreadyExistsException(string message) : base(message) { }
        public InstanceAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
        protected InstanceAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}