using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;

namespace CoralTime.Common.Attributes
{
    public class BaseCheckSecureHeaderFilter : ActionFilterAttribute
    {
        protected readonly IConfiguration _config;

        public BaseCheckSecureHeaderFilter(IConfiguration config)
        {
            _config = config;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasNotificationSecureHeader = context.HttpContext.Request.Headers.TryGetValue(GetSecureHeaderName(), out var requestSecureHeaderValue);
            if (!(hasNotificationSecureHeader && (requestSecureHeaderValue == GetSecureHeaderValue())))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        protected virtual string GetSecureHeaderName()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetSecureHeaderValue()
        {
            throw new NotImplementedException();
        }
    }
}