using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace SecureBlackjack
{
    class Client
    {
        /*static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        static RSAParameters rsaPrivKey = RSA.ExportParameters(true);
        static RSAParameters rsaPubKey = RSA.ExportParameters(false);
        static Encryption Encryptor = new Encryption(rsaPrivKey, rsaPubKey);*/
        
        static int count = 0;
        static string name;
        static string RegStatus = "notreg";
        static bool Started = false;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                                                           \r\n                                                           \r\n  /$$$$$$$  /$$$$$$   /$$$$$$$ /$$   /$$  /$$$$$$  /$$$$$$ \r\n /$$_____/ /$$__  $$ /$$_____/| $$  | $$ /$$__  $$/$$__  $$\r\n|  $$$$$$ | $$$$$$$$| $$      | $$  | $$| $$  \\__/ $$$$$$$$\r\n \\____  $$| $$_____/| $$      | $$  | $$| $$     | $$_____/\r\n /$$$$$$$/|  $$$$$$$|  $$$$$$$|  $$$$$$/| $$     |  $$$$$$$\r\n|_______/  \\_______/ \\_______/ \\______/ |__/      \\_______/\r\n                                                           \r\n                                                          ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("/$$       /$$                     /$$                               /$$      \r\n| $$      | $$                    | $$                              | $$      \r\n| $$$$$$$ | $$  /$$$$$$   /$$$$$$$| $$   /$$ /$$  /$$$$$$   /$$$$$$$| $$   /$$\r\n| $$__  $$| $$ |____  $$ /$$_____/| $$  /$$/|__/ |____  $$ /$$_____/| $$  /$$/\r\n| $$  \\ $$| $$  /$$$$$$$| $$      | $$$$$$/  /$$  /$$$$$$$| $$      | $$$$$$/ \r\n| $$  | $$| $$ /$$__  $$| $$      | $$_  $$ | $$ /$$__  $$| $$      | $$_  $$ \r\n| $$$$$$$/| $$|  $$$$$$$|  $$$$$$$| $$ \\  $$| $$|  $$$$$$$|  $$$$$$$| $$ \\  $$\r\n|_______/ |__/ \\_______/ \\_______/|__/  \\__/| $$ \\_______/ \\_______/|__/  \\__/\r\n                                       /$$  | $$                              \r\n                                      |  $$$$$$/                              \r\n                                       \\______/                               ");
            Console.WriteLine("\nWelcome to Secure Blackjack, a CS 492 - Computer Security final project");
            Console.WriteLine("This project was created by : Matt Koser & Alan Stano");
            Console.WriteLine("Please enter a username. Enter \"CONTROLLER\" to run the controller client. This is required to play.");
            Register();
            Thread.Sleep(400);
            Console.WriteLine("You have been registered! Welcome to the game, please wait for your turn.");
            FileSystemWatcher listener = new FileSystemWatcher();
            Thread.Sleep(200);
            listener.Path = @"C:\Blackjack\" + name.ToUpper();
            listener.Filter = "*.txt";
            listener.EnableRaisingEvents = true;
            listener.Created += Recieved;
            listener.IncludeSubdirectories = true;

            while(!Started) //will not start accepting input until the game starts
            {
                continue;
            }
            String end = "";
            while(!end.Equals("end")) //currently no way to "unregister" a player. will implement soon
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
            { 
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    line = sr.ReadToEnd();
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            //String decryptedData = Encryptor.Decrypt(line, rsaPrivKey, false);
            String[] message = line.Split(' ');
            Console.WriteLine("\n_____________________________________________________________\n");
            switch(message[0]) //first word is the "command"
            {
                case "start":
                    Started = true;
                    break;
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
                    Console.WriteLine($"You have BUSTED with hand value {message[1]}");
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
                    Console.WriteLine("The dealer has beaten you. You recieve no chips.");
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
                case "newround":
                    Console.WriteLine("A new round is starting!");
                    break;
                case "stood":
                    Console.WriteLine("You have chosen to stand. Please wait.");
                    break;
                case "deposit":
                    Console.WriteLine($"{message[1]} chips have been depositied into your balance. Your new balance is {message[2]}");
                    break;
                case "withdraw":
                    Console.WriteLine($"{message[1]} chips have been withdrawn into your balance. Your new balance is {message[2]}");
                    break;
                case "badbet":
                    Console.WriteLine($"The bet you have entered is invalid. The bet limits are: {message[1]} to {message[2]}");
                    break;
                case "inputbet":
                    Console.WriteLine($"Its your turn to bet. Your balance is currently {message[1]}. Please enter an integer between {message[2]} and {message[3]}");
                    break;
                case "outofchips":
                    Console.WriteLine("Oh no, you have ran out of chips! You have been given more.");
                    break;
                default:
                    Console.WriteLine("No valid message has been detected...");
                    break;
            }
        }

        private static void Register()
        {
            name = Console.ReadLine();
            if (name.ToUpper().Equals("CONTROLLER"))
            {
                GameController controller = new GameController(); //This redirects all of the logic to the gamecontroller object. When it finishes in the controller, the program exits
                Environment.Exit(0);
            }
            String folder = @"C:\Blackjack";
            DirectoryInfo folderMaker = new DirectoryInfo(folder);
            FileSystemWatcher registerListen = new FileSystemWatcher();
            try
            {
                folderMaker.CreateSubdirectory(name.ToUpper()); //Creates C:\Blackjack\NAME, folder where outgoing comms are placed
            }
            catch (Exception f)
            {
                Console.WriteLine($"Error: {f.ToString()}.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Thread.Sleep(350);
            Communicate(name);
            string path = @"C:\Blackjack\" + name.ToUpper();
            registerListen.Path = path;
            registerListen.Filter = "*.txt";
            registerListen.EnableRaisingEvents = true;
            registerListen.Created += ConfirmRegister;
            registerListen.IncludeSubdirectories = true;

            while (RegStatus.Equals("notreg"))
                continue;
            if (RegStatus.Equals("bad"))
            {
                Console.WriteLine("Your name has been rejected by the server. Please try again.");
                Thread.Sleep(1000);
                registerListen.Dispose();
                Environment.Exit(0);
            }
            registerListen.Dispose();
        }

        private static void ConfirmRegister(object sender, FileSystemEventArgs e)
        {
            //String data = "";
            String line = "";
            Thread.Sleep(50);
            try
            {
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    line = sr.ReadToEnd();
                    line = line.Remove(line.Length - 2); // get rid of new line escape char 
                    //data = Encryptor.Decrypt(line, rsaPrivKey, false);
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            RegStatus = line;
        }

        private static void Communicate(String message)
        {
            //String data = Encryptor.Encrypt(message, rsaPubKey, false);
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
