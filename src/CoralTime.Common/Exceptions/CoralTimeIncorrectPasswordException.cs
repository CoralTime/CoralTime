using System;
using System.Collections.Generic;
using CoralTime.ViewModels.Errors;

namespace CoralTime.Common.Exceptions
{
    public class CoralTimeIncorrectPasswordException : Exception
    {
        public List<ErrorODataView> errors;

        public CoralTimeIncorrectPasswordException() { }
    }
}