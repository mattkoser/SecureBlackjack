using System;
using System.Collections.Generic;
using System.Text;

namespace SecureBlackjack
{

    class Card
    {
        public String Suit { get; }
        public String Name { get; }
        public int Val { get; }
        public string Image { get; }
        public Card(String s, String n)
        {
            Suit = s;
            Name = n;
            string cardTemplate = "";

            //Card images from : https://www.asciiart.eu/miscellaneous/playing-cards

            switch (Name)
            {
                case "Ace":
                    cardTemplate = " _____ \n|A    |\n|     |\n|  X  |\n|     |\n|____A|";
                    Val = 11; //The "or 1" is handled in the calculations later
                    break;
                case "Two":
                    cardTemplate = " _____ \n|2    |\n|  X  |\n|     |\n|  X  |\n|____2|";
                    Val = 2;
                    break;
                case "Three":
                    cardTemplate = " _____ \n|3    |\n| X X |\n|     |\n|  X  |\n|____3|";
                    Val = 3;
                    break;
                case "Four":
                    cardTemplate = " _____ \n|4    |\n| X X |\n|     |\n| X X |\n|____4|";
                    Val = 4;
                    break;
                case "Five":
                    cardTemplate = " _____ \n|5    |\n| X X |\n|  X  |\n| X X |\n|____5|";
                    Val = 5;
                    break;
                case "Six":
                    cardTemplate = " _____ \n|6    |\n| X X |\n| X X |\n| X X |\n|____6|";
                    Val = 6;
                    break;
                case "Seven":
                    cardTemplate = " _____ \n|7    |\n| X X |\n|X X X|\n| X X |\n|____7|";
                    Val = 7;
                    break;
                case "Eight":
                    cardTemplate = " _____ \n|8    |\n|X X X|\n| X X |\n|X X X|\n|____8|";
                    Val = 8;
                    break;
                case "Nine":
                    cardTemplate = " _____ \n|9    |\n|X X X|\n|X X X|\n|X X X|\n|____9|";
                    Val = 9;
                    break;
                case "Ten":
                    cardTemplate = " _____ \n|10 X |\n|X X X|\n|X X X|\n|X X X|\n|___10|";
                    Val = 10;
                    break;
                case "Jack":
                    Val = 10;
                    break;
                case "Queen":
                    Val = 10;
                    break;
                case "King":
                    Val = 10;
                    break;
                default:
                    cardTemplate = " _____ \n|?    |\n|     |\n|     |\n|     |\n|____?|";
                    Val = 0;
                    break;

            }
            switch(Suit)
            {
                case "Spades":
                    cardTemplate = cardTemplate.Replace("X", "♠");
                    break;
                case "Hearts":
                    cardTemplate = cardTemplate.Replace("X", "♥");
                    break;
                case "Diamonds":
                    cardTemplate = cardTemplate.Replace("X", "♦");
                    break;
                case "Clubs":
                    cardTemplate = cardTemplate.Replace("X", "♣");
                    break;
            }
            Image = cardTemplate;
            
        }

    }
}
