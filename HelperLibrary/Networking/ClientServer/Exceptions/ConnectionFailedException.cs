using System;

namespace HelperLibrary.Networking.ClientServer.Exceptions
{

    [Serializable]
    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException() { }
        public ConnectionFailedException(string message) : base(message) { }
        public ConnectionFailedException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
