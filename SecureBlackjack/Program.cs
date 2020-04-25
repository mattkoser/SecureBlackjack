using System;

namespace SecureBlackjack
{
    class Client
    {
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
                Player player = new Player(name);
            }
                

        }

        static void InitialSetup(String drive)
        {

        }
    }

}
