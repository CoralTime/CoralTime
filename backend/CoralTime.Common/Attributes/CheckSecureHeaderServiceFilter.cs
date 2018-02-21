using Microsoft.Extensions.Configuration;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Attributes
{
    public class CheckSecureHeaderServiceFilter : BaseCheckSecureHeaderFilter
    {
        public CheckSecureHeaderServiceFilter(IConfiguration config) 
            : base(config) { }

        protected override string GetSecureHeaderName()
        {
            return SecureHeaderNameService;
        }

        protected override string GetSecureHeaderValue()
        {
            return _config[SecureHeaderValueService];
        }
    }
}