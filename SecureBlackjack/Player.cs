﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
        public bool Done { get; set; }
        public bool Bust { get; set; }
        public int Count { get; set; }
        public bool Won { get; set; }
        public int Bet { get; set; }
        public RSAParameters pubKey;
        public Player(String name, RSAParameters key)
        {
            Name = name;
            Folder = @"C:\Blackjack\" + name.ToUpper();
            Chips = 500; //Starting amount
            Done = false;
            Count = 0;
            Won = false;
            pubKey = key;
        }

       public void Reset()
       {
            hand.Clear();
            Bust = false;
            Done = false;
            Won = false;
            Bet = 0;
       }

        public void DealCard(Card c)
        {
            hand.Add(c);
        }
        public List<Card> GetHand()
        {
            return hand;
        }
    }
}
