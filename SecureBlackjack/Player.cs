using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SecureBlackjack
{
    class Player
    {
        FileSystemWatcher Listener = new FileSystemWatcher();
        public int Chips { get; set; }
        public string Folder { get; }
        public string Name;
        public List<Card> hand = new List<Card>();
        public int Count { get; set; }
        public Player(String name)
        {
            Name = name;
            Folder = @"C:\Blackjack\" + name.ToUpper();
            Chips = 500; //Starting amount
            Count = 0;
        }

        private void Wait()
        {

        }

        public void DealCard(Card c)
        {
            hand.Add(c);
        }
        public List<Card> GetHand()
        {
            return hand;
        }



        private void Recieved(object source, FileSystemEventArgs file)
        {

        }


    }
}
