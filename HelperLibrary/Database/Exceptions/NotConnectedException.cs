using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class NotConnectedException : Exception
    {
        public NotConnectedException() { }
        public NotConnectedException(string message) : base(message) { }
        public NotConnectedException(string message, Exception inner) : base(message, inner) { }
        protected NotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
