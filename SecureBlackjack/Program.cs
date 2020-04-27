using System;
using System.IO;

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
            Console.WriteLine("Please enter your username. Controller to run controller client");


            String name = Console.ReadLine();
            if (name.ToUpper().Equals("CONTROLLER"))
            {
                GameController controller = new GameController();
            }
            else
            {
                Communicate(name);
            }
                

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
