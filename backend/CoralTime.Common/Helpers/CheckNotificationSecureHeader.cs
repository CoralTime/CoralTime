using CoralTime.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public class CheckSecureHeader : Attribute, IActionFilter
    {
        private string SecureHeaderName { get; }

        private string SecureHeaderValue { get; }

        public CheckSecureHeader(string secureHeaderName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvName}.json", optional: true);

            var configuration = builder.Build();

            SecureHeaderName = secureHeaderName;
            SecureHeaderValue = configuration["SecureHeaderValue"];
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasNotificationSecureHeader = context.HttpContext.Request.Headers.TryGetValue(SecureHeaderName, out var requestSecureHeaderValue);
            if (!hasNotificationSecureHeader)
            {
                throw new CoralTimeDangerException($"Request doesn't contains {SecureHeaderName}.");
            }

            if (!requestSecureHeaderValue.Contains(SecureHeaderValue))
            {
                throw new CoralTimeDangerException($"A {SecureHeaderName} has bad value.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
