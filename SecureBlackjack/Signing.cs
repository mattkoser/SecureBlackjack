using System;
using System.Text;
using System.Security.Cryptography;

namespace SecureBlackjack
{
    class Signing
    {
        public byte[] HashAndSignBytes(String s, RSAParameters Key)
        {
            var Encoder = new UTF8Encoding();
            byte[] bytes = Encoder.GetBytes(s);
            byte[] signResult;

            try
            {
                // Create a new instance of RSACryptoServiceProvider using the
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                // Hash and sign the data. Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                signResult = RSAalg.SignData(bytes, new SHA1CryptoServiceProvider());
                return signResult;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public bool VerifySignedHash(String s, byte[] SignedData, RSAParameters Key)
        {
            var Encoder = new UTF8Encoding();
            byte[] bytes = Encoder.GetBytes(s);
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                // Verify the data using the signature.  Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return RSAalg.VerifyData(bytes, new SHA1CryptoServiceProvider(), SignedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
    }
}
