using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeForbiddenException : Exception
    {
        public CoralTimeForbiddenException() { }

        public CoralTimeForbiddenException(string message) 
            : base(message) { }

        public CoralTimeForbiddenException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
