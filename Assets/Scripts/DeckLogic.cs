// using UnityEngine;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// [System.Serializable]

// public class Deck : MonoBehaviour
// {
//     private static List<Card> cards = new List<Card>();
//     private int currCard;
//     public Deck()
//     {
//         Reset();
//         Shuffle();
//     }

    // public void Shuffle()
    // {
    //     System.Random random = new System.Random();
    //     int n = cards.Count;
    //     //Fisher- Yates
    //     while(n > 1)
    //     {
    //         int j = random.Next(0,n);
    //         n--;
    //         Card temp = cards[j];
    //         cards[j] = cards[n];
    //         cards[n] = temp;
    //     }
    // }

//     public Card DealCard()
//     {
//         if(currCard >= cards.Count)
//         {
//             throw new Exception("Deck is empty! This shouldn't happen in poker.");
//         }
//         return cards[currCard++];
//     }

//     public void Reset()
//     {
//         cards.Clear();
//         foreach(Card.Suit s in Enum.GetValues(typeof(Card.Suit)))
//         {
//             foreach (Card.Rank r in Enum.GetValues(typeof(Card.Rank)))
//             {
//                 cards.Add(new Card(s,r));
//             }
//         }
//         currCard = 0;
//     }

//     public int RemainingCards()
//     {
//         return cards.Count - currCard;
//     }

// }
