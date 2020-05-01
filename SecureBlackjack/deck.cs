using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace SecureBlackjack
{
    enum Names
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
    enum Suits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    class Deck
    {
        public int Size { get; }
        Stack<Card> shuffled = new Stack<Card>();

        public Deck() // Deck should only be created once. When the Size is seen as 0 when drawing a card it will re-shuffle. The first creation includes a shuffle
        {
            Shuffle();
        }

        public void Shuffle()
        {
            List<Card> raw = new List<Card>(); //Generate a list of every card in order, 8 total decks.
            int count = 0;
            for (int i = 0; i < 8; i++) //Casinos use 8 total decks up before re-shuffling to hinder card counting, we will do the same
            {
                for (int j = 0; j < 4; j++) //For each suit
                {
                    for (int k = 0; k < 13; k++) //For every value
                    {
                        Card next = new Card((Enum.GetName(typeof(Suits), j)), (Enum.GetName(typeof(Names), k)));
                        raw.Add(next);
                    }
                }
            }
            //Securely choose a number between 0 to n-1, n-2, n-3...
            //Push the card that exists at that position into the "stack" of cards and remove. Rinse and repeat until no cards are left in the raw deck.
            int cardChoice;
            int count2 = 0;
            while(raw.Count > 0)
            {   
                cardChoice = RandomNumberGenerator.GetInt32(0, raw.Count); //Choose a random card and put it in our final stack. RandomNumberGenerator is derived from System.Security.Cryptography
                shuffled.Push(raw[cardChoice]);
                raw.RemoveAt(cardChoice);

                count2++;
            }
        }

        public Card DrawCard()
        {
            if(shuffled.Count == 0)
            {
                Console.WriteLine("The deck is being re-shuffled!");
                Shuffle(); //We should use up the entire deck before creating a new set of 8 decks.
            }
            return shuffled.Pop();
        }
    }
}
