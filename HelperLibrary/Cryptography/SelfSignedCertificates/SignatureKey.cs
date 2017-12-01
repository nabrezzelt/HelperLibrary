using System;

namespace HelperLibrary.Cryptography.SelfSignedCertificates
{
    public class SignatureKey : CryptKey
    {
        internal SignatureKey(CryptContext ctx, IntPtr handle) : base(ctx, handle) { }

        public override KeyType Type => KeyType.Signature;
    }
}
