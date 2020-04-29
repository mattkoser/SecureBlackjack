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
        public int Count { get; set; }
        int Sent = 0;
        public Player(String name)
        {
            this.Name = name;
        }

        private void Wait()
        {

        }
        //Incoming messages to player P
        private void Recieved(object source, FileSystemEventArgs file)
        {

        }

        //Outgoing messages to Controller only
        private void Communicate(Player p, String message)
        {
            Encryption RSA = new Encryption();
            //ALAN - encrypt message for player p
            RSA.Encrypt(message, RSA.ExportParameters(false), false);
            string destination = p.Folder + "\\" + "message" + p.Count + ".txt";
            using (StreamWriter s = File.CreateText(destination))
            {
                s.WriteLine(message);
            }
            p.Count = p.Count++;
        }
    }
}
