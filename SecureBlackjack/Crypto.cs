using System;
using System.Text;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    class Encryption //Symmetric Key Encryption / Decryption
    {
        static RSACryptoServiceProvider RSA;
        static RSAParameters rsaPubKey;
        static RSAParameters rsaPrivKey;
        public Encryption(RSAParameters privKey, RSAParameters pubKey)
        {
            rsaPrivKey = privKey;
            rsaPubKey = pubKey;
            RSA = new RSACryptoServiceProvider();
        }

        public String Encrypt(String s, RSAParameters RSAKey, bool DoOAEPPadding) //Takes in a String s and encrypts is using a key
        {
            byte[] plainText;
            byte[] encryptedData;
            plainText = Encoding.ASCII.GetBytes(s);
            String cipherText;

            try
            {
                using(RSA)  
                {  
                    RSA.ImportParameters(rsaPubKey);  
                        encryptedData = RSA.Encrypt(plainText, DoOAEPPadding);  
                }
                cipherText = Encoding.ASCII.GetString(encryptedData);
                return cipherText;
            }  
            catch (CryptographicException e)  
            {  
                Console.WriteLine(e.Message);  
                return null;  
            }
        }

        public String Decrypt(String s, RSAParameters RSAKey, bool DoOAEPPadding)  
        {  
            String decryptedText;
            byte[] encryptedData = Encoding.ASCII.GetBytes(s);

            try  
            {  
                byte[] decryptedData;
                using(RSA)
                {  
                    RSA.ImportParameters(rsaPrivKey);  
                        decryptedData = RSA.Decrypt(encryptedData, DoOAEPPadding);  
                }
                decryptedText = Encoding.ASCII.GetString(decryptedData); 
                return decryptedText;
            }  
            catch (CryptographicException e)  
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}