using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeInsertDublicateException : Exception
    {
        public CoralTimeInsertDublicateException() { }

        public CoralTimeInsertDublicateException(string message) 
            : base(message) { }

        public CoralTimeInsertDublicateException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
