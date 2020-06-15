using System.Collections.Generic;

namespace CoralTime.ViewModels.Azure
{
    public class Key
    {
        public string Kty { get; set; }
        public string Use { get; set; }
        public string Kid { get; set; }
        public string X5t { get; set; }
        public string N { get; set; }
        public string E { get; set; }
        public List<string> X5c { get; set; }
    }

    public class CertificateKeys
    {
        public List<Key> Keys { get; set; }
    }
}