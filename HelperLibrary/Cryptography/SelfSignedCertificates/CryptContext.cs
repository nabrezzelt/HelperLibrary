using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;


namespace HelperLibrary.Cryptography.SelfSignedCertificates
{
    public class CryptContext : DisposeableObject
    {
        private IntPtr _handle = IntPtr.Zero;

        public IntPtr Handle => _handle;

        public string ContainerName { get; set; }

        public string ProviderName { get; set; }

        public int ProviderType { get; set; }

        public int Flags { get; set; }


        /// <summary>
        /// By default, sets up to create a new randomly named key container
        /// </summary>
        public CryptContext()
        {
            ContainerName = Guid.NewGuid().ToString();
            ProviderType = 1; // default RSA provider
            Flags = 8; // create new keyset
        }

        public void Open()
        {
            ThrowIfDisposed();
            if (!Win32Native.CryptAcquireContext(out _handle, ContainerName, ProviderName, ProviderType, Flags))
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();
        }

        public X509Certificate2 CreateSelfSignedCertificate(SelfSignedCertProperties properties)
        {
            ThrowIfDisposedOrNotOpen();

            GenerateKeyExchangeKey(properties.IsPrivateKeyExportable, properties.KeyBitLength);
            //GenerateSignatureKey(properties.IsPrivateKeyExportable, properties.KeyBitLength);

            byte[] asnName = properties.Name.RawData;
            GCHandle asnNameHandle = GCHandle.Alloc(asnName, GCHandleType.Pinned);

            var kpi = new Win32Native.CryptKeyProviderInformation
            {
                ContainerName = this.ContainerName,
                KeySpec = (int) KeyType.Exchange,
                ProviderType = 1 // default RSA provider
            };

            IntPtr certContext = Win32Native.CertCreateSelfSignCertificate(
                _handle,
                new Win32Native.CryptoApiBlob(asnName.Length, asnNameHandle.AddrOfPinnedObject()),
                0, kpi, IntPtr.Zero,
                ToSystemTime(properties.ValidFrom),
                ToSystemTime(properties.ValidTo),
                IntPtr.Zero);

            asnNameHandle.Free();

            if (IntPtr.Zero == certContext)
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();

            X509Certificate2 cert = new X509Certificate2(certContext); // dups the context (increasing it's refcount)

            if (!Win32Native.CertFreeCertificateContext(certContext))
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();

            return cert;
        }

        private Win32Native.SystemTime ToSystemTime(DateTime dateTime)
        {
            long fileTime = dateTime.ToFileTime();
            var systemTime = new Win32Native.SystemTime();
            if (!Win32Native.FileTimeToSystemTime(ref fileTime, systemTime))
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();
            return systemTime;
        }

        public KeyExchangeKey GenerateKeyExchangeKey(bool exportable, int keyBitLength)
        {
            ThrowIfDisposedOrNotOpen();

            uint flags = (exportable ? 1U : 0U) | ((uint)keyBitLength) << 16;

            IntPtr keyHandle;
            bool result = Win32Native.CryptGenKey(_handle, (int)KeyType.Exchange, flags, out keyHandle);
            if (!result)
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();

            return new KeyExchangeKey(this, keyHandle);
        }

        private SignatureKey GenerateSignatureKey(bool exportable, int keyBitLength)
        {
            ThrowIfDisposedOrNotOpen();

            uint flags = (exportable ? 1U : 0U) | (uint)keyBitLength << 16;

            bool result = Win32Native.CryptGenKey(_handle, (int)KeyType.Signature, flags, out var keyHandle);
            if (!result)
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();

            return new SignatureKey(this, keyHandle);
        }

        internal void DestroyKey(CryptKey key)
        {
            ThrowIfDisposedOrNotOpen();
            if (!Win32Native.CryptDestroyKey(key.Handle))
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();
        }

        protected override void CleanUp(bool viaDispose)
        {
            if (_handle == IntPtr.Zero)
                return;

            if (Win32Native.CryptReleaseContext(_handle, 0))
                return;

            // only throw exceptions if we're NOT in a finalizer
            if (viaDispose)
                Win32ErrorHelper.ThrowExceptionIfGetLastErrorIsNotZero();
        }

        private void ThrowIfDisposedOrNotOpen()
        {
            ThrowIfDisposed();
            ThrowIfNotOpen();
        }

        private void ThrowIfNotOpen()
        {
            if (IntPtr.Zero == _handle)
                throw new InvalidOperationException("You must call CryptContext.Open first.");
        }            
    }
}
