using System;
using System.IO;
using System.Threading;

namespace SecureBlackjack
{
    class Client
    {
        static int count = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine("<Title>");
            Console.WriteLine("Welcome to Secure Blackjack!");
            Console.WriteLine("Enter Username or CONTROLLER: ");


            String name = Console.ReadLine();
            if (name.ToUpper().Equals("CONTROLLER"))
            {
                GameController controller = new GameController();
                Environment.Exit(0);
            }
            Communicate(name);
            FileSystemWatcher listener = new FileSystemWatcher();
            Thread.Sleep(50);
            listener.Path = @"C:\Blackjack\" + name.ToUpper();
            listener.Filter = "*.txt";
            listener.EnableRaisingEvents = true;
            listener.Created += Recieved;
            Console.ReadKey();


        }

        private static void Recieved(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(50);
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    Console.WriteLine("Message recieved: " + line);
                    Console.WriteLine(line.Length);
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
            Console.ReadKey();
        }

        private static void Communicate(String message)
        {
            //ALAN - Encrypt the message that is being sent to the controller

            string destination = @"C:\Blackjack\CONTROLLER" + "\\" + message.ToUpper()+ count.ToString() + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
        }
    }

}
