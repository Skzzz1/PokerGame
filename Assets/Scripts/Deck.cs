using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards;
    private int currIndex;
    
    public Deck()
    {
        cards = new List<Card>();
        Reset();
    }
    
    public void Reset()
    {
        cards.Clear();
        currIndex = 0;
        
        // Create all 52 cards
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                cards.Add(new Card(suit, rank));
            }
        }
        
        Shuffle();
    }
    
    public void Shuffle()
    {
        System.Random random = new System.Random();
        int n = cards.Count;
        //Fisher- Yates
        while(n > 1)
        {
            int j = random.Next(0,n);
            n--;
            Card temp = cards[j];
            cards[j] = cards[n];
            cards[n] = temp;
        }
    }
    
    public Card DrawCard()
    {
        if (currIndex >= cards.Count)
        {
            Debug.LogError("Deck is empty!");
            return null;
        }
        
        Card drawn = cards[currIndex++];
        cards.RemoveAt(0);
        return drawn;
    }
    
    public int CardsRemaining()
    {
        return cards.Count - currIndex;
    }
}
