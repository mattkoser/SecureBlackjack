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
            
        }

        private void Wait()
        {

        }



        private void Recieved(object source, FileSystemEventArgs file)
        {

        }


    }
}
