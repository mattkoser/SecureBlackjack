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
        Queue<Player> Order = new Queue<Player>();
        Deck deck = new Deck();
        List<Card> Hand = new List<Card>(); //The dealers hand
        public GameController()
        {
            Console.WriteLine("Hello controller");
            WaitForPlayers();
        }


        //Will call the game loop once all players are in.
        private void WaitForPlayers()
        {
            FileSystemWatcher playerListen = new FileSystemWatcher();
            playerListen.Path = @"C:\Blackjack\Controller";
            playerListen.Filter = "*.txt";
            playerListen.EnableRaisingEvents = true;
            playerListen.Created += NewPlayer;
            playerListen.IncludeSubdirectories = true;
            Console.WriteLine("Waiting for players to register! When you are ready to start the game, press Enter.");
            Console.ReadKey();
            playerListen.Dispose();
            GameLoop();

        }

        private void GameLoop()
        {
            bool gameOver = false;
            while(!gameOver)
            {
                Console.WriteLine("Dealing a card to all players at table!");
                for(int i = 0; i < Players.Count; i++) //Create the turn order
                {
                    Order.Enqueue(Players[i]);
                    Deal(Players[i]); //Deal first card
                }
                Card next = deck.DrawCard();
                Console.WriteLine($"Dealer has been dealt a {next.Name} of {next.Suit}!");
                Hand.Add(next); //Dealers Card
                for (int i = 0; i < Players.Count; i++)
                {
                    Communicate(Players[i], "dealerhas " + next.Name + " " + next.Suit);
                }
                Console.WriteLine("Dealing out second card!");
                for (int i = 0; i < Players.Count; i++)
                {
                    Deal(Players[i]); //Deal first card
                }
                next = deck.DrawCard();
                Console.WriteLine($"Dealer has been dealt a {next.Name} of {next.Suit}.");
                Hand.Add(next); //Dealers Card
                for (int i = 0; i < Players.Count; i++)
                {
                    Communicate(Players[i], "dealerfacedown ");
                }
                for (int i = 0; i < Players.Count; i++)
                {
                    ProcessTurn(Players[i]);
                    
                }
            }

        }

        private void ProcessTurn(Player p)
        {
            FileSystemWatcher turnListen = new FileSystemWatcher();
            turnListen.Path = @"C:\Blackjack\Controller";
            turnListen.Filter = "*.txt";
            turnListen.EnableRaisingEvents = true;
            turnListen.Created += Turn;
            turnListen.IncludeSubdirectories = true;
            int playerValue = GetHandValue(p.GetHand());
            String dealerValue = $"{Hand[0].Val}+?"; //Players can only see the first card that the dealer has
            Communicate(p, $"itsyourturn {playerValue} {dealerValue}");
            Console.ReadKey();
        }

        private int GetHandValue(List<Card> hand)
        {
            int value = 0;
            int ace = 0;
            for(int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Name == "Ace")
                    ace++;
                value += hand[i].Val;
            }
            for(int i = 0; i <= ace; i++)
            {
                if (value > 21)
                    value = value - 10; //Adjust the ace's value to 1. This is done for as many aces are in the hand.
            }
            return value;
        }

        private void Turn(object sender, FileSystemEventArgs e) //Gets turn from player
        {
            Thread.Sleep(50);
            String line = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                    line = line.Remove(line.Length); // get rid of new line escape char 
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            
        }

            private void Deal(Player p)
        {
            Card next = deck.DrawCard();
            p.DealCard(next);
            Console.WriteLine($"{p.Name} has been dealt a {next.Name} of {next.Suit}!");
            Communicate(p, "deal " + next.Name + " " + next.Suit);
        }

        private void BeginTurn(Player p)
        {
        }

        private void NewPlayer(object sender, FileSystemEventArgs e)
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
                    Console.WriteLine(line.Length);
                    line = line.Remove(line.Length-2); // get rid of new line escape char 
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            String folder = @"C:\Blackjack";
            DirectoryInfo folderMaker = new DirectoryInfo(folder);

            try
            {
                folderMaker.CreateSubdirectory(line.ToUpper());

            }
            catch (Exception f)
            {
                Console.WriteLine($"Error: {f.ToString()}.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Player newPlayer = new Player(line);
            Players.Add(newPlayer);
        }

        private void Communicate(Player p, String message)
        {
            Console.WriteLine($"Player's message recieved count is {p.Count}");
            Encryption RSA = new Encryption();
            //ALAN - encrypt message for player p
            //RSA.Encrypt(message, RSA.ExportParameters(false), false);
            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            using(StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            p.Count = p.Count + 1;
        }
    }
}
