using System;
using System.Security.Cryptography.X509Certificates;
using HelperLibrary.Cryptography.SelfSignedCertificates;
using HelperLibrary.Logging;

namespace SelfsignedCertificateGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("distinguished-name: ");
            var distinguishedFor = Console.ReadLine();
            Console.WriteLine("Generating RSA-Keys...");

            using (var cryptContext = new CryptContext())
            {
                cryptContext.Open();

                /* Generate Certificate with default settings:
                   
                    DateTime today = DateTime.Today;
                    ValidFrom = today.AddDays(-1);
                    ValidTo = today.AddYears(10);
                    Name = new X500DistinguishedName("cn=self");
                    KeyBitLength = 4096;
                
                   X509Certificate2 certificate = cryptContext.CreateSelfSignedCertificate(new SelfSignedCertProperties()); 
                */

                // Generate Certificate with custom setting
                X509Certificate2 certificate = cryptContext.CreateSelfSignedCertificate(
                    new SelfSignedCertProperties
                    {
                        IsPrivateKeyExportable = true,
                        KeyBitLength = 4096,
                        Name = new X500DistinguishedName("cn=" + (distinguishedFor == "" ? "localhost" : distinguishedFor)),
                        ValidFrom = DateTime.Today.AddDays(-1),
                        ValidTo = DateTime.Today.AddYears(1)
                    });

                Log.DisplaySelfCertDetails(certificate);                
                Console.ReadLine();
            }
        }
    }
}
