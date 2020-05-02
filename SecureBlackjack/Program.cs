using System;
using System.IO;
using System.Threading;

namespace SecureBlackjack
{
    class Client
    {
        static int count = 0;
        static string name;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine("<Title>");
            Console.WriteLine("Welcome to Secure Blackjack!");
            Console.WriteLine("Enter Username or CONTROLLER: ");


            name = Console.ReadLine();
            if (name.ToUpper().Equals("CONTROLLER"))
            {
                GameController controller = new GameController();
                Environment.Exit(0);
            }
            Thread.Sleep(400);
            Communicate(name);
            Thread.Sleep(100);
            Console.WriteLine("You have been registered! Welcome to the game, please wait for your turn.");
            FileSystemWatcher listener = new FileSystemWatcher();
            Thread.Sleep(200);
            listener.Path = @"C:\Blackjack\" + name.ToUpper();
            listener.Filter = "*.txt";
            listener.EnableRaisingEvents = true;
            listener.Created += Recieved;
            listener.IncludeSubdirectories = true;
            String end = "";
            while(!end.Equals("end"))
            {
                end = Console.ReadLine();
                Communicate(end);
            }

        }

        private static void Recieved(object sender, FileSystemEventArgs e)
        {
            String line = "";
            Thread.Sleep(50);
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadToEnd();
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }

            String[] message = line.Split(' ');
            Console.WriteLine("\n_____________________________________________________________\n");
            switch(message[0]) //first word is the "command"
            {
                case "deal":
                    Console.WriteLine($"You were dealt a {message[1]} of {message[2]}");
                    break;
                case "dealerhas":
                    Console.WriteLine($"The dealer was dealt a {message[1]} of {message[2]}");
                    break;
                case "dealerfacedown":
                    Console.WriteLine("The dealer has recieved a face down card.");
                    break;
                case "itsyourturn":
                    Console.WriteLine($"Its your turn to play! Options are: (h)it, (s)tand");
                    Console.WriteLine($"Your hand value: {message[1]}\nDealer's Hand: {message[2]}");
                    break;
                case "bust":
                    Console.WriteLine($"You have BUSTED! (Hand value: {message[1]})");
                    break;
                case "pushblackjack":
                    Console.WriteLine($"The dealer and you have both been dealt blackjack. The hand has been pushed. ");
                    break;
                case "dealerhand":
                    Console.WriteLine("All players have finished their turns. It is now the dealers turn to play.");
                    Console.WriteLine($"The dealer reveals their flipped over card as a {message[1]} of {message[2]}");
                    Console.WriteLine($"The dealer has a hand value of {message[3]}");
                    break;
                case "dealerdraw":
                    Console.WriteLine($"The dealer has drawn a {message[1]} of {message[2]}. The dealers hand is now {message[3]}");
                    break;
                case "lose":
                    Console.WriteLine("The dealer has beaten you.");
                    break;
                case "push":
                    Console.WriteLine("You have tied the dealer. The hand is pushed.");
                    break;
                case "win":
                    Console.WriteLine("You have won the hand! Congratulations!");
                    break;
                case "dealerblackjack":
                    Console.WriteLine("The dealer has been dealt blackjack, ending the hand.");
                    break;
                case "stood":
                    Console.WriteLine("You have chosen to stand. Please wait.");
                    break;
                default:
                    Console.WriteLine("No valid message has been detected...");
                    break;
            }
        }

        private static void Communicate(String message)
        {
            //ALAN - Encrypt the message that is being sent to the controller
            String destination;
            if(count == 0) //The first message needs to be placed in the main directory of controller
                destination = @"C:\Blackjack\CONTROLLER" + "\\" + "registration" + name + ".txt";
            else
                destination = @"C:\Blackjack\CONTROLLER" + "\\" + name.ToUpper() + "\\"  + "communication" + count.ToString() + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            count++;
        }
    }

}
