using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class CouldNotConnectException : Exception
    {
        public CouldNotConnectException() { }
        public CouldNotConnectException(string message) : base(message) { }
        public CouldNotConnectException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotConnectException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
