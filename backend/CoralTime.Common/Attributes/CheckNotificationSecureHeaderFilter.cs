using Microsoft.Extensions.Configuration;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Attributes
{
    public class CheckNotificationSecureHeaderFilter : BaseCheckSecureHeaderFilter
    {
        public CheckNotificationSecureHeaderFilter(IConfiguration config) : base(config)
        {
        }

        protected override string GetSecureHeaderName()
        {
            return SecureHeaderNotificationName;
        }

        protected override string GetSecureHeaderValue()
        {
            return _config["NotificationSecureHeaderValue"];
        }
    }
}