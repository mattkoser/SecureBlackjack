using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SecureBlackjack
{
    class Player
    {
        FileSystemWatcher Listener = new FileSystemWatcher();
        string Directory = @"C:\Blackjack";
        public string Folder { get; }
        public string Name;

        public int MCount { get; set; } //Message Number
        int Sent = 0;

        public Player(String name)
        {
            this.Name = name;
            string destination = @"C:\Blackjack" + "\\" + "message" + p.MCount + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(name);
            }
        }

        private void Wait()
        {

        }

        //Incoming messages to player P
        //Needes to be decrypted
        private void Recieve(object source, FileSystemEventArgs file)
        {
            String message = "";

            StreamReader read = new StreamReader(@"C:\Blackjack\\" + Name);
            try
            {
                message = read.ReadToEnd();
            }
            catch(IOException e)
            {
                Console.WriteLine("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }

        //Outgoing messages to Controller only
        //Needs to be encrypted
        private void Communicate(Player p, String message)
        {
            Encryption RSA = new Encryption();
            //ALAN - encrypt message for player p
            string destination = @"C:\Blackjack" + "\\" + "message" + p.MCount + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            p.MCount++;
        }
    }
}
