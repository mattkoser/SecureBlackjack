using System;
using System.IO;
using System.Security.Cryptography;

namespace SecureBlackjack
{
    class KeyStorage
    {
        public KeyStorage()
        {
            GenKey_SaveInContainer("key");
        }
        public static void GenKey_SaveInContainer(string ContainerName)
        {
            //Create key container with name
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = ContainerName;

            //Create a new instance of RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            // Display the key information to the console.  
            Console.WriteLine("Key added to container: \n  {0}", rsa.ToXmlString(true));
        }

        public static void GetKeyFromContainer(string ContainerName)
        {
            // Create the CspParameters object and set the key container
            // name used to store the RSA key pair.  
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = ContainerName;

            // Create a new instance of RSACryptoServiceProvider that accesses  
            // the key container MyKeyContainerName.  
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cp);

            // Display the key information to the console.  
            Console.WriteLine("Key retrieved from container : \n {0}", RSA.ToXmlString(true));
        }

        public static void DeleteKeyFromContainer(string ContainerName)
        {
            // Create the CspParameters object and set the key container
            // name used to store the RSA key pair.  
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = ContainerName;

            // Create a new instance of RSACryptoServiceProvider that accesses  
            // the key container.  
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            // Delete the key entry in the container.  
            rsa.PersistKeyInCsp = false;

            // Call Clear to release resources and delete the key from the container.  
            rsa.Clear();

            Console.WriteLine("Key deleted.");
        }
    }
}
