using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeUpdateException : Exception
    {
        public CoralTimeUpdateException() { }

        public CoralTimeUpdateException(string message) 
            : base(message) { }

        public CoralTimeUpdateException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
