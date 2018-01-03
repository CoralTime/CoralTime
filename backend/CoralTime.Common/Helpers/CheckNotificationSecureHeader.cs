using CoralTime.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace CoralTime.Common.Helpers
{
    public class CheckSecureHeader : Attribute, IActionFilter
    {
        private string HeaderName{ get; }

        public CheckSecureHeader(string headerName)
        {
            HeaderName = headerName;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasNotificationSecureHeader = context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var secureHeaderValue);
            if (!hasNotificationSecureHeader)
            {
                throw new CoralTimeDangerException($"Request doesn't contains {HeaderName}.");
            }

            if (!secureHeaderValue.Contains(Constants.Constants.SecureHeaderValue))
            {
                throw new CoralTimeDangerException($"{HeaderName} has bad value.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
