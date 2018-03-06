using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HelperLibrary.Cryptography
{
    public class HashManager
    {
        private static readonly Random Random = new Random();
        public static string HashSha512(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text can not be empty.", nameof(text));
            }

            byte[] textBytes = TextToBytes(text);
            return HashSha512(textBytes);
        }

        public static string HashSha512(byte[] data)
        {
            using (var sha = new SHA512Managed())
            {
                byte[] hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static string HashSha384(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text can not be empty.", nameof(text));
            }

            byte[] textBytes = TextToBytes(text);
            return HashSha384(textBytes);
        }

        public static string HashSha384(byte[] data)
        {
            using (var sha = new SHA384Managed())
            {
                byte[] hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static string HashSha256(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text can not be empty.", nameof(text));
            }

            byte[] textBytes = TextToBytes(text);
            return HashSha256(textBytes);
        }

        public static string HashSha256(byte[] data)
        {
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static string HashSha1(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text can not be empty.", nameof(text));
            }

            byte[] textBytes = TextToBytes(text);
            return HashSha1(textBytes);
        }

        public static string HashSha1(byte[] data)
        {
            using (var sha = new SHA1Managed())
            {
                byte[] hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static string HashMd5(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text can not be empty.", nameof(text));
            }

            byte[] textBytes = TextToBytes(text);
            return HashMd5(textBytes);
        }

        public static string HashMd5(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        private static byte[] TextToBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static string GenerateSecureRandomToken(int bytes = 32, bool includeGuid = true)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                StringBuilder randomToken = new StringBuilder();
                byte[] tokenData;

                if (includeGuid)
                {
                    tokenData = new byte[16];
                    rng.GetBytes(tokenData);

                    byte[] randomGuid = new Guid(tokenData).ToByteArray();
                    randomToken.Append(Convert.ToBase64String(randomGuid));
                }

                tokenData = new byte[bytes];
                rng.GetBytes(tokenData);

                randomToken.Append(Convert.ToBase64String(tokenData));

                return randomToken.ToString()
                    .Replace("=", String.Empty)
                    .Replace("+", String.Empty)
                    .Replace("/", String.Empty);
            }
        }

        public static string GenerateRandomStringFromChars(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            return new string(Enumerable.Repeat(chars, length)
                               .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

    }
}
