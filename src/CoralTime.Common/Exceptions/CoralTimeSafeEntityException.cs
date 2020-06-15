using CoralTime.ViewModels.Errors;
using System;
using System.Collections.Generic;

namespace CoralTime.Common.Exceptions
{

    public class CoralTimeSafeEntityException : Exception
    {
        public List<ErrorODataView> errors;

        public CoralTimeSafeEntityException() { }

        public CoralTimeSafeEntityException(string message) 
            : base(message) { }

        public CoralTimeSafeEntityException(string message, Exception ex) 
            : base(message, ex) { }
    }
}