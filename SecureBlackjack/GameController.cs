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
        static RSACryptoServiceProvider RSA;
        static RSAParameters rsaPrivKey;
        static RSAParameters rsaPubKey;
       
        static Encryption Encryptor;

        FileSystemWatcher Communicator = new FileSystemWatcher();
        List<Player> Players = new List<Player>();
        Deck deck = new Deck();
        List<Card> Hand = new List<Card>(); //The dealers hand
        int Current;
        bool DealerBust = false;
        const int MIN_BET = 1;
        const int MAX_BET = 100;

        public GameController(RSAParameters privKey, RSAParameters pubKey)
        {
            rsaPrivKey = privKey;
            rsaPubKey = pubKey;
            Console.WriteLine("Controller client now running! Please note this window must be active as it functions as the \"server\" for this blackjack game.");
            WaitForPlayers();
            RSA = new RSACryptoServiceProvider();
            Encryptor = new Encryption(rsaPrivKey, rsaPrivKey);
        }

        //Loop that waits for players to register
        //Will call the game loop once all players are in
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
            bool gameOver = false; //Eventually we may decide to implement a win condition. For now it will just play rounds indefiniately
            while(!gameOver)
            {
                SendToAll("start "); //Allow input from player consoles.

                for (int i = 0; i < Players.Count; i++) //Cycle through every player at the table and get their bet
                {
                    Current = i;
                    ProcessBet(Players[i]);
                }
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
                        if (playerVal == 21) //player also had blackjack so they tied the dealer
                        {
                            Communicate(Players[i], "push ");
                            DepositCredits(Players[i], Players[i].Bet); //In a push you get your original bet back
                        }
                        else
                        {
                            Communicate(Players[i], "lose "); //No credits on loss
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
                    int payout = 0;
                    if (Players[i].Bust) //Players can't win if they bust
                    {
                        Communicate(Players[i], "lose "); //No credits on loss
                        continue;
                    }
                    if(DealerBust) //Dealer busts, all remaining players win
                    {
                        Communicate(Players[i], "win ");
                        if (playerVal == 21) //3:2
                        {
                            payout = Players[i].Bet + (int)(Players[i].Bet * (3 / 2)); //3:2 + their original bet back
                        }
                        else //No blackjack, so 1:1
                        {
                            payout = (int)(Players[i].Bet * (2 / 1));
                        }
                    }
                    else if (playerVal == dealerVal) //1:1, original bet returned
                    {
                        Communicate(Players[i], "push ");
                        payout = Players[i].Bet; 
                    }
                    else if (playerVal > dealerVal)
                    {
                        Communicate(Players[i], "win ");
                        if (playerVal == 21) //3:2
                        {
                            payout = Players[i].Bet + (int)(Players[i].Bet * (3 / 2)); //3:2 + their original bet back
                        }
                        else //No blackjack, so 2:1
                        {
                            payout = (int)(Players[i].Bet * (2 / 1));
                        }
                    }
                    else if (dealerVal > playerVal)
                    {
                        Communicate(Players[i], "lose "); //No credits
                        continue;
                    }
                    Console.WriteLine($"Paying player {Players[i].Name} {payout}.");
                    DepositCredits(Players[i], payout);
                }
                Console.WriteLine("Please press enter to begin a new round.");
                Console.ReadKey();
                SendToAll("newround ");
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

        private void SendToAll(String m) //Sends a message to all players
        {
            for(int i = 0; i < Players.Count; i++)
            {
                Communicate(Players[i], m);
            }
        }
        private void ProcessBet(Player p)
        {
            int playerChips = p.Chips;
            if (playerChips <= 0)
            {
                Communicate(p, "outofchips ");
                Console.WriteLine($"{p.Name} has ran out of chips and has been given 100 more.");
                DepositCredits(p, 100);
            }
            Console.WriteLine($"It is now {p.Name}'s turn.");
            FileSystemWatcher betListen = new FileSystemWatcher();
            string watcherPath = @"C:\Blackjack\CONTROLLER\" + p.Name.ToUpper();
            betListen.Path = watcherPath;
            betListen.Filter = "*.txt";
            betListen.EnableRaisingEvents = true;
            betListen.Created += NewBet;
            betListen.IncludeSubdirectories = true;

            Communicate(p, $"inputbet {playerChips} {MIN_BET} {MAX_BET}");

            while (p.Bet == 0)
                continue;
            betListen.Dispose();
        }

        private void NewBet(object sender, FileSystemEventArgs e) //Gets turn from player
        {
            Thread.Sleep(50);
            String line = "";
            try
            {  
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    line = sr.ReadToEnd();
                    line = line.Remove(line.Length - 2); // get rid of new line escape char 
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }

            String data = Encryptor.Decrypt(line, rsaPrivKey, false);
            int bet;
            try
            {
                bet = Int32.Parse(line);
            }
            catch (Exception g) //Usually when the input is not a valid integer
            {
                Console.WriteLine($"Error parsing input from {Players[Current].Name}'s bet : {g.ToString()}");
                Communicate(Players[Current], $"badbet {MIN_BET} {MAX_BET}");
                return; //break out of the method and wait for a new input
            }
            if (bet <= MAX_BET && bet >= MIN_BET)
            {
                Withdraw(Players[Current], bet);
                Players[Current].Bet = bet;
            }
            else
            {
                Communicate(Players[Current], $"badbet {MIN_BET} {MAX_BET}");
            }
        }

        private void DepositCredits(Player p, int amt)
        {
            //Blackjack pays out 3:2, win pays 1:1. Therefore the highest bet is a blackjack MAX_BET. the server should not pay any values higher than that
            int payoutmax = (int)MAX_BET * (3 / 2) + MAX_BET;
            if(amt < MIN_BET || amt > payoutmax)
            {
                Console.WriteLine("Tried to desposit an invalid amount: " + amt);
                return;
            }
            else
            {
                p.Chips = p.Chips + amt;
                Communicate(p, $"deposit {amt} {p.Chips}");
            }
        }

        private void Withdraw(Player p, int amt)
        {
            Console.WriteLine(amt);
            if (amt < MIN_BET || amt > MAX_BET)
            {
                Console.WriteLine("Tried to withdraw an invalid amount: " + amt);
                return;
            }
            else
            {
                int temp = p.Chips;
                if (temp - amt < 0)
                {
                    Console.WriteLine("Tried to withdraw an invalid amount: " + amt);
                    return;
                }    
                p.Chips = p.Chips - amt;
                Communicate(p, $"withdraw {amt} {p.Chips}");
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
                    value = value - 10; //Adjust the ace's value to 1 . This is done for as many aces are in the hand.
            }
            return value;
        }
        private void Turn(object sender, FileSystemEventArgs e) //Gets turn from player
        {
            Thread.Sleep(50);
            String line = "";
            String data;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                    //line = line.Remove(line.Length-2); // get rid of new line escape char 
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }

            data = Encryptor.Decrypt(line, rsaPrivKey, false);
            data = data.ToLower();

            switch (data)
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
            Thread.Sleep(400);
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

            String folder = @"C:\Blackjack";
            String otherFolder = @"C:\Blackjack\CONTROLLER";
            DirectoryInfo folderMaker = new DirectoryInfo(folder);
            DirectoryInfo otherMaker = new DirectoryInfo(otherFolder);
            Console.WriteLine($"{data} has been registered!");
            Console.WriteLine("Decrypted data: " + data);
            try
            {
                folderMaker.CreateSubdirectory(data.ToUpper()); //Creates C:\Blackjack\NAME, folder where outgoing comms are placed
                otherMaker.CreateSubdirectory(data.ToUpper()); //Creates C:\Blackjack\NAME, where incoming comms are placed
            }
            catch (Exception f)
            {
                Console.WriteLine($"Error: {f.ToString()}.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Player newPlayer = new Player(data);
            Communicate(newPlayer, "good"); //Validate player name and send "bad" if their name does not fit criteria
            Players.Add(newPlayer); //only if name is "good"
        }

        private void Communicate(Player p, String message)
        {
            p.Count = p.Count + 1;
            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            Thread.Sleep(100);
            using(StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(data);
            }
        }
    }
}
