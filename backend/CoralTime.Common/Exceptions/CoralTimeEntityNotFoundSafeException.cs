using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeEntityNotFoundException : Exception
    {
        public CoralTimeEntityNotFoundException() { }

        public CoralTimeEntityNotFoundException(string message) 
            : base(message) { }

        public CoralTimeEntityNotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
