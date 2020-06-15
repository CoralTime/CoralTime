using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeUnauthorizedException : Exception
    {
        private const string DefaultMessage = "Oops. Member is unauthorized";

        public CoralTimeUnauthorizedException() 
            : base(DefaultMessage) { }

        public CoralTimeUnauthorizedException(string message) 
            : base(message) { }

        public CoralTimeUnauthorizedException(string message, Exception ex) 
            : base(message, ex) { }
    }
}