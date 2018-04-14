using Microsoft.Extensions.Configuration;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Attributes
{
    public class CheckSecureHeaderNotificationFilter : BaseCheckSecureHeaderFilter
    {
        public CheckSecureHeaderNotificationFilter(IConfiguration config) 
            : base(config) { }

        protected override string GetSecureHeaderName()
        {
            return SecureHeaderNameNotification; 
        }

        protected override string GetSecureHeaderValue()
        {
            return _config[SecureHeaderValueNotification];
        }
    }
}