using Microsoft.Extensions.Configuration;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Attributes
{
    public class CheckServiceSecureHeaderFilter : BaseCheckSecureHeaderFilter
    {
        public CheckServiceSecureHeaderFilter(IConfiguration config) : base(config)
        {
        }

        protected override string GetSecureHeaderName()
        {
            return SecureHeaderServiceName;
        }

        protected override string GetSecureHeaderValue()
        {
            return _config["ServiceSecureHeaderValue"];
        }
    }
}