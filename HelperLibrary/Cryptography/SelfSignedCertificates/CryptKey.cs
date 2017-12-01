using System;

namespace HelperLibrary.Cryptography.SelfSignedCertificates
{
    public abstract class CryptKey : DisposeableObject
    {
        private readonly CryptContext _ctx;

        internal IntPtr Handle { get; }

        internal CryptKey(CryptContext ctx, IntPtr handle)
        {
            _ctx = ctx;
            Handle = handle;
        }

        public abstract KeyType Type { get; }

        protected override void CleanUp(bool viaDispose)
        {
            // keys are invalid once CryptContext is closed,
            // so the only time I try to close an individual key is if a user
            // explicitly disposes of the key.
            if (viaDispose)
                _ctx.DestroyKey(this);
        }
    }
}