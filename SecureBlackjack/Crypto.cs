using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    class Encryption //Symmetric Key Encryption / Decryption
    {
        static RSACryptoServiceProvider RSA;
        CspParameters key;
        public Encryption(CspParameters cp)
        {
            RSA = new RSACryptoServiceProvider(cp);
            key = cp;
        }

        public String Encrypt(String s, RSAParameters RSAKey, bool DoOAEPPadding) //Takes in a String s and encrypts is using a key
        {
            byte[] plainText;
            byte[] encryptedData;
            plainText = Encoding.ASCII.GetBytes(s);
            String cipherText;

            try
            {
                using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cp))  
                {  
                    RSA.ImportParameters(RSAKey);  
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
                using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cp))
                {  
                    RSA.ImportParameters(RSAKey);  
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