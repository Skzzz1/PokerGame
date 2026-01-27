using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// Estimates how likely your hand is to win
public class HandStrengthCalculator
{
    // Estimate hand strength with Monte Carlo simulation for bots
    public static float EstimateHandStrength(List<Card> myHand, List<Card> board, int numOpponents, int simulations = 500)
    {
        int wins = 0;
        int ties = 0;
        System.Random rng = new System.Random();

        for (int s = 0; s < simulations; s++)
        {
            // 1. Build deck excluding known cards
            List<Card> deck = BuildFullDeck().Except(myHand).Except(board).ToList();

            // 2. Deal random hands to opponents
            List<List<Card>> opponentHands = new List<List<Card>>();
            for (int p = 0; p < numOpponents; p++)
            {
                opponentHands.Add(DealRandomHand(deck, rng, 2));
            }

            // 3. Complete community cards
            List<Card> fullBoard = new List<Card>(board);
            while (fullBoard.Count < 5)
            {
                int idx = rng.Next(deck.Count);
                fullBoard.Add(deck[idx]);
                deck.RemoveAt(idx);
            }

            // 4. Evaluate hands using new HandValue system
            var myHandValue = HandEvaluator.BestHandValue(myHand, fullBoard);
            
            bool win = true;
            bool tie = false;
            
            foreach (var opp in opponentHands)
            {
                var oppHandValue = HandEvaluator.BestHandValue(opp, fullBoard);
                int comparison = myHandValue.CompareTo(oppHandValue);
                
                if (comparison < 0) // Opponent wins
                { 
                    win = false; 
                    tie = false;
                    break; 
                }
                else if (comparison == 0) // Tie
                {
                    tie = true;
                }
            }

            if (win && !tie) 
                wins++;
            else if (win && tie)
                ties++;
        }

        // Count ties as half wins
        return ((float)wins + (float)ties * 0.5f) / simulations;
    }

    private static List<Card> BuildFullDeck()
    {
        var deck = new List<Card>();
        foreach (Card.Suit s in Enum.GetValues(typeof(Card.Suit)))
            foreach (Card.Rank r in Enum.GetValues(typeof(Card.Rank)))
                deck.Add(new Card(s, r));
        return deck;
    }

    private static List<Card> DealRandomHand(List<Card> deck, System.Random rng, int count)
    {
        var hand = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            int idx = rng.Next(deck.Count);
            hand.Add(deck[idx]);
            deck.RemoveAt(idx);
        }
        return hand;
    }
}