using System;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeDangerException : Exception
    {
        private const string DefaultMessage = "Oops. Server error happened";

        public CoralTimeDangerException() : base(DefaultMessage) { }

        public CoralTimeDangerException(string message) : base(message) { }

        public CoralTimeDangerException(string message, Exception ex) : base(message, ex) { }
    }
}