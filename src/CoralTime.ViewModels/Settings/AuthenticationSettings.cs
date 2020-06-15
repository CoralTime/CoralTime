﻿using System.Collections.Generic;

namespace CoralTime.ViewModels.Settings
{
    public class AuthenticationSettings
    {
        public bool EnableAzure { get; set; }

        public AzureSettings AzureSettings { get; set; }
        
        public string InstrumentationKey { get; set; }

        public Dictionary<string, string> Roles { get; set; }
    }

    public class AzureSettings
    {
        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string RedirectUrl { get; set; }
    }
}