using System;
using System.Text;
using System.Security.Cryptography;

namespace SecureBlackjack
{
    class Signing
    {
        public String HashAndSignBytes(String s, RSAParameters Key)
        {
            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            String result;
            byte[] bytes = ByteConverter.GetBytes(s);
            byte[] signResult;

            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportParameters(Key); //Imports passed key
                signResult = RSA.SignData(bytes, new SHA256CryptoServiceProvider());
                result = Convert.ToBase64String(signResult);
                byte[] signedmsg = Convert.FromBase64String(result);
                return result;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public bool VerifySignedHash(String[] s, RSAParameters Key)
        {
            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            String signed = s[s.Length - 1];
            String text = "";

            for (int i = 0; i < s.Length - 1; i++)
            {
                if(i+1 == s.Length-1) // next iteration ends loop
                {
                    text += s[i];
                }
                else
                {
                    text += s[i] + ' '; //rebuilds the string as it was before the Split()
                }
            }
            byte[] bytes = ByteConverter.GetBytes(text);
            byte[] signedmsg = new byte[] { };
            try {
                signedmsg = Convert.FromBase64String(signed);
            }
            catch(Exception f)
            {
                return false;
            }

            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportParameters(Key); //use the public key of person sending message
                return RSA.VerifyData(bytes, new SHA256CryptoServiceProvider(), signedmsg);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
    }
}
