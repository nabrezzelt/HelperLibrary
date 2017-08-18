using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Database.Exceptions
{

    [Serializable]
    public class MissingConnectionStringException : Exception
    {
        public MissingConnectionStringException()
        {

        }

        public MissingConnectionStringException(string message) : base(message)
        {

        }

        public MissingConnectionStringException(string message, Exception inner) : base(message, inner)
        {

        }

        protected MissingConnectionStringException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }
}
