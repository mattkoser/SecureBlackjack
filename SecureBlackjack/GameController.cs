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
        string Directory = @"C:\Blackjack\Controller"; //Directory to hold communications
        List<Player> Players = new List<Player>(); //List of players

        FileSystemWatcher Communicator = new FileSystemWatcher(); //Used for filesystem communication
        
        public GameController()
        {
            Console.WriteLine("Hello controller");
            WaitForPlayers();
        }

        //Adds a player to the ArrayList of players
        //Creates a new directory for player communication
        public void AddPlayer(Player p)
        {
            Players.Add(p);

        }

        //Will call the game loop once all players are in.
        //Adendum: Possibly no longer needed
        void WaitForPlayers()
        {
            FileSystemWatcher playerListen = new FileSystemWatcher();
            playerListen.Path = @"C:\Blackjack\Controller";
            
            playerListen.EnableRaisingEvents = true;
            playerListen.Created += NewPlayer;
            playerListen.IncludeSubdirectories = true;
            Console.ReadKey();

        }

        //Deprecated
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

        //Communicator for Controller to Player
        //Needs to be encrypted
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
