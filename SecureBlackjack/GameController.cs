using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    class GameController
    {
        string Directory = @"C:\Blackjack";
        FileSystemWatcher Communicator = new FileSystemWatcher();
        List<Player> Players = new List<Player>();
        public GameController()
        {
            Console.WriteLine("Hello controller");
            Console.ReadKey();
            WaitForPlayers();
        }


        //Will call the game loop once all players are in.
        void WaitForPlayers()
        {

        }

        private void Communicate(Player p, String message)
        {

            //ALAN - encrypt message for player p

            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            using(StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            p.Count = p.Count++;
        }


    }


}
