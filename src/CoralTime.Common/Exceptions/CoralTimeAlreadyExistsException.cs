using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeAlreadyExistsException : Exception
    {
        public CoralTimeAlreadyExistsException() { }

        public CoralTimeAlreadyExistsException(string message) 
            : base(message) { }

        public CoralTimeAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
