﻿using System;
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
        FileSystemWatcher Communicator = new FileSystemWatcher();
        List<Player> Players = new List<Player>();
        Queue<Player> Order = new Queue<Player>();
        Deck deck = new Deck();
        List<Card> Hand = new List<Card>(); //The dealers hand
        int Current;
        bool DealerBust = false;
        public GameController()
        {
            Console.WriteLine("Controller client now running! Please note this window must be active as it functions as the \"server\" for this blackjack game.");
            WaitForPlayers();
        }


        //Will call the game loop once all players are in.
        private void WaitForPlayers()
        {
            FileSystemWatcher playerListen = new FileSystemWatcher();
            playerListen.Path = @"C:\Blackjack\CONTROLLER";
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
                    Deal(Players[i]); //Deal first card
                }
                Card next = deck.DrawCard();
                Console.WriteLine($"Dealer has been dealt a {next.Name} of {next.Suit}!");
                Hand.Add(next); //Dealers Card
                SendToAll("dealerhas " + next.Name + " " + next.Suit);
                Console.WriteLine("Dealing out second card!");
                for (int i = 0; i < Players.Count; i++)
                {
                    Deal(Players[i]); //Deal first card
                }
                next = deck.DrawCard();
                Console.WriteLine($"Dealer has been dealt a {next.Name} of {next.Suit}.");
                Hand.Add(next); //Dealers Card
                SendToAll("dealerfacedown ");
                for (int i = 0; i < Players.Count; i++) //Cycle through every player at the table and get their final hand
                {
                    Current = i;
                    ProcessTurn(Players[i]);
                }

                Thread.Sleep(500);

                int dv = GetHandValue(Hand);
                if (dv == 21) //End the loop early
                {
                    for (int i = 0; i < Players.Count; i++)
                    {
                        int playerVal = GetHandValue(Players[i].hand);
                        if (playerVal == 21)
                        {
                            Communicate(Players[i], "push ");
                        }
                        else
                        {
                            Communicate(Players[i], "lose ");
                        }
                    }
                    Console.WriteLine("Please press enter to begin a new round.");
                    Console.ReadKey();
                    ResetGame();
                    continue;
                }
                Console.WriteLine("All players have finished. It is now the dealers turn.");
                SendToAll($"dealerhand {Hand[1].Name} {Hand[1].Suit} {dv}"); //Reveal the flipped over card to all players

                while (dv <= 17) //"Automatic dealer" stands on 17, hits on under
                {

                    next = deck.DrawCard();
                    Hand.Add(next);
                    dv = GetHandValue(Hand);
                    SendToAll($"dealerdraw {next.Name} {next.Suit} {dv}");
                    if (dv >= 17)
                        break; //while loop one too many times
                    Thread.Sleep(200);
                }
                

                if (dv > 21)
                    DealerBust = true;


                Thread.Sleep(300); //Things are going WAYY too fast for a player to digest. the sleeps help it feel mroe natural;
                for (int i = 0; i < Players.Count; i++)
                {
                    int playerVal = GetHandValue(Players[i].hand);
                    int dealerVal = GetHandValue(Hand);
                    if (Players[i].Bust) //Players can't win if they bsut
                    {
                        Communicate(Players[i], "lose ");
                        continue;
                    }
                    if(DealerBust) //Dealer wins, all remaining players win
                    {
                        Communicate(Players[i], "win ");
                        continue;
                    }

                    if (playerVal == dealerVal)
                    {
                        Communicate(Players[i], "push ");
                    }
                    else if (playerVal > dealerVal)
                    {
                        Communicate(Players[i], "win ");
                    }
                    else if (dealerVal > playerVal)
                    {
                        Communicate(Players[i], "lose ");
                    }
                }
                Console.WriteLine("Please press enter to begin a new round.");
                Console.ReadKey();
                ResetGame();
            }

        }
        private void ResetGame()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Reset();
                Hand.Clear();
                DealerBust = false;
            }
        }

        private void SendToAll(String m)
        {
            for(int i = 0; i < Players.Count; i++)
            {
                Communicate(Players[i], m);
            }
        }

        private void ProcessTurn(Player p)
        {
            Console.WriteLine($"It is now {p.Name}'s turn.");
            FileSystemWatcher turnListen = new FileSystemWatcher();
            string watcherPath = @"C:\Blackjack\CONTROLLER\" + p.Name.ToUpper();
            turnListen.Path = watcherPath;
            turnListen.Filter = "*.txt";
            turnListen.EnableRaisingEvents = true;
            turnListen.Created += Turn;
            turnListen.IncludeSubdirectories = true;
            int playerValue = GetHandValue(p.GetHand());
            String dealerValue = $"{Hand[0].Val}+?"; //Players can only see the first card that the dealer has
            int realDealerValue = GetHandValue(Hand);
            Communicate(p, $"itsyourturn {playerValue} {dealerValue}");
            if(realDealerValue == 21)
            {
                Communicate(p, "dealerblackjack ");
                p.Done = true;
            }
            while (!p.Done)
                continue;
            turnListen.Dispose();

        }

        private int GetHandValue(List<Card> hand)
        {
            int value = 0;
            int ace = 0;
            for(int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Name.Equals("Ace"))
                {
                    ace++;
                }

                value += hand[i].Val;
            }
            for(int i = 0; i < ace; i++)
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
                    line = line.Remove(line.Length-2); // get rid of new line escape char 
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }


            line = line.ToLower();

            switch (line)
            {
                case "h":
                case "hit":
                    Deal(Players[Current]);
                    int playerValue = GetHandValue(Players[Current].GetHand());
                    String dealerValue = $"{Hand[0].Val}+?";
                    Players[Current].Bust = playerValue > 21;
                    if (Players[Current].Bust)
                    {
                        Communicate(Players[Current], $"bust {playerValue}");
                        Players[Current].Done = true;
                    }
                    else
                        Communicate(Players[Current], $"itsyourturn {playerValue} {dealerValue}");
                    break;
                case "s":
                case "stand":
                    Console.WriteLine("Player standing.");
                    Players[Current].Done = true;
                    Communicate(Players[Current], "stood ");
                    break;
                default:
                    Communicate(Players[Current], "invalid ");
                    break;
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
                    Console.WriteLine(line.Length);
                    line = line.Remove(line.Length-2); // get rid of new line escape char
                    Console.WriteLine($"{line} has been registered!");
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            String folder = @"C:\Blackjack";
            String otherFolder = @"C:\Blackjack\CONTROLLER";
            DirectoryInfo folderMaker = new DirectoryInfo(folder);
            DirectoryInfo otherMaker = new DirectoryInfo(otherFolder);
            try
            {
                folderMaker.CreateSubdirectory(line.ToUpper()); //Creates C:\Blackjack\NAME, folder where outgoing comms are placed
                otherMaker.CreateSubdirectory(line.ToUpper()); //Creates C:\Blackjack\NAME, where incoming comms are placed

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
            Encryption RSA = new Encryption();
            //ALAN - encrypt message for player p
            //RSA.Encrypt(message, RSA.ExportParameters(false), false);
            p.Count = p.Count + 1;
            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            Thread.Sleep(100);
            using(StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }

        }
    }
}
