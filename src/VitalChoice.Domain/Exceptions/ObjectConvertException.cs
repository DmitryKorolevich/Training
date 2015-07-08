using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Exceptions
{
    public class ObjectConvertException : Exception
    {
        public ObjectConvertException()
        {
            
        }

        public ObjectConvertException(string message)
            : base(message)
        {
            
        }

        public ObjectConvertException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
