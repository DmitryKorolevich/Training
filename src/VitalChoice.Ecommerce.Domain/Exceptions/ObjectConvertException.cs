using System;

namespace VitalChoice.Ecommerce.Domain.Exceptions
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
