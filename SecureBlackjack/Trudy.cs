using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace SecureBlackjack
{
    class Trudy
    {
        private int count = 0;
        public Trudy()
        {
            Intrude();
        }

        void Intrude()
        {
            String msg = "";
            String player = "";
            while (!msg.Equals("end"))
            {
                Console.WriteLine("Please enter a player to send a spoofed message to.");
                player = Console.ReadLine();
                Console.WriteLine("Please enter the message you would like to send.");
                msg = Console.ReadLine();
                Communicate(player, msg);
            }
        }

        private void Communicate(string p, String message)
        {
            
            string signed = "";
            string destination;
            if (p.ToUpper().Equals("CONTROLLER"))
            {
                Console.WriteLine("Who should the message be spoofed as?");
                string opt = Console.ReadLine();
                opt = opt.ToUpper();
                destination = @"C:\Blackjack\CONTROLLER" + "\\" + opt + "\\" + "trudy" + count + ".txt";
            }
            else
                destination = @"C:\Blackjack\" + p + "\\" + "trudy" + count + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            count++;
        }
    }


}
