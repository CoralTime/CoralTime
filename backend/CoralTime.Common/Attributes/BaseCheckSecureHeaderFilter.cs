using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace CoralTime.Common.Attributes
{
    public abstract class BaseCheckSecureHeaderFilter : ActionFilterAttribute
    {
        protected readonly IConfiguration _config;
        
        protected abstract string GetSecureHeaderName();

        protected abstract string GetSecureHeaderValue();

        protected BaseCheckSecureHeaderFilter(IConfiguration config)
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
    }
}