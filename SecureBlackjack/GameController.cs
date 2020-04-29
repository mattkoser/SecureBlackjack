using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    class GameController
    {
        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        string Directory = @"C:\Blackjack\Controller";
        FileSystemWatcher Communicator = new FileSystemWatcher();
        List<Player> Players = new List<Player>();
        public GameController()
        {
            Console.WriteLine("Hello controller");
            WaitForPlayers();
        }


        //Will call the game loop once all players are in.
        void WaitForPlayers()
        {
            FileSystemWatcher playerListen = new FileSystemWatcher();
            playerListen.Path = @"C:\Blackjack\Controller";
            
            playerListen.EnableRaisingEvents = true;
            playerListen.Created += NewPlayer;
            playerListen.IncludeSubdirectories = true;
            Console.ReadKey();

        }

        static void NewPlayer(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(50);
            String line = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                    Console.WriteLine("Message recieved: " + line);
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            DirectoryInfo folderMaker = new DirectoryInfo(@"C:\Blackjack" + "\\" + line.ToUpper());

            try
            {
                if (folderMaker.Exists)
                {
                    Console.WriteLine("There is already a user with that name! Please try again.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                folderMaker.Create();

            }
            catch (Exception f)
            {
                Console.WriteLine($"Error: {f.ToString()}.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.ReadKey();
        }

        private void Communicate(Player p, String message)
        {
            Encryption RSA = new Encryption();
            //ALAN - encrypt message for player p
            RSA.Encrypt(message, RSA.ExportParameters(false), false);
            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            using(StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            p.Count = p.Count++;
        }
    }
}
