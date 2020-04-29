using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    class Encryption //Symmetric Key Encryption / Decryption
    {
        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        public byte[] Encrypt(String s, RSAParameters RSAKey, bool DoOAEPPadding) //Takes in a String s and encrypts is using a key
        {
            byte[] plainText;
            byte[] encryptedData;
            plainText = Encoding.ASCII.GetBytes(s);

            try  
            {
                using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())  
                {  
                    RSA.ImportParameters(RSAKey);  
                        encryptedData = RSA.Encrypt(plainText, DoOAEPPadding);  
                }        
                return encryptedData;
            }  
            catch (CryptographicException e)  
            {  
                Console.WriteLine(e.Message);  
                return null;  
            }
        }

        public String Decrypt(byte[]Data, RSAParameters RSAKey, bool DoOAEPPadding)  
        {  
            String decryptedText;

            try  
            {  
                byte[] decryptedData;
                using(RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {  
                    RSA.ImportParameters(RSAKey);  
                        decryptedData = RSA.Decrypt(Data, DoOAEPPadding);  
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

    class Authentication
    {
        public void authecticate(String player, String privateKey)
        {
            Console.WriteLine("Enter Username: ");
        }
    }
}