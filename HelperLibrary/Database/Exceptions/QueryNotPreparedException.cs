using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
