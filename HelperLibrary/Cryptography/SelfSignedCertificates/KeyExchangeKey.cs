using System;

namespace HelperLibrary.Cryptography.SelfSignedCertificates
{
    public class KeyExchangeKey : CryptKey
    {
        internal KeyExchangeKey(CryptContext ctx, IntPtr handle) : base(ctx, handle) { }

        public override KeyType Type => KeyType.Exchange;
    }
}