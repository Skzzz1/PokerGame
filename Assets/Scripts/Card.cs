using UnityEngine;
// using System;




// [CreateAssetMenu(fileName ="New Card",menuName = "Card")]
public class Card
{
    // public string cardName;
    public Suit CardSuit;
    public Rank CardRank;

        public enum Suit
    {
        H, D, C, S
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 1
    }
    

    public Card (Suit s, Rank r)
    {
        CardSuit = s;
        CardRank = r;
    }

    public override string ToString()
    {
        return $"{CardRank} Of {CardSuit} ";
    }

    // Helper to get sprite name (e.g., "2H", "AS")
    public string GetSpriteName()
    {
        string rankStr = CardRank == Rank.Ace ? "A" :
                        CardRank == Rank.King ? "K" :
                        CardRank == Rank.Queen ? "Q" :
                        CardRank == Rank.Jack ? "J" :
                        ((int)CardRank).ToString();
        
        string suitStr = CardSuit.ToString();
        
        return $"{rankStr}{suitStr}";
    }

}

