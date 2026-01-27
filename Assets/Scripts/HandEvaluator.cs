using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Determines poker hand rankings with proper tie-breaking
public class HandEvaluator
{
    public enum HandRank
    {
        HighCard = 0,
        Pair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourOfAKind = 7,
        StraightFlush = 8,
        RoyalFlush = 9
    }

    // Represents a complete hand evaluation with tie-breaking information
    public class HandValue : IComparable<HandValue>
    {
        public HandRank Rank { get; set; }
        public List<int> TieBreakers { get; set; } // Ordered from most to least important
        public List<Card> Cards { get; set; }

        public HandValue()
        {
            TieBreakers = new List<int>();
            Cards = new List<Card>();
        }

        // Compare two hands to determine winner
        // Returns: >0 if this hand wins, <0 if other wins, 0 if tie
        public int CompareTo(HandValue other)
        {
            if (other == null) return 1;

            // First compare hand ranks
            if (Rank != other.Rank)
                return Rank.CompareTo(other.Rank);

            // If same rank, compare tie-breakers in order
            for (int i = 0; i < Math.Min(TieBreakers.Count, other.TieBreakers.Count); i++)
            {
                if (TieBreakers[i] != other.TieBreakers[i])
                    return TieBreakers[i].CompareTo(other.TieBreakers[i]);
            }

            return 0; // Perfect tie
        }

        public override string ToString()
        {
            return $"{Rank} [{string.Join(", ", TieBreakers)}]";
        }
    }

    // Find the best 5-card poker hand from player's hole cards + community cards
    public static HandValue BestHandValue(List<Card> playerCards, List<Card> communityCards)
    {
        var allCards = playerCards.Concat(communityCards).ToList();
        HandValue best = null;
        
        foreach (var combo in Get5CardCombos(allCards))
        {
            var handValue = EvaluateHandValue(combo);
            if (best == null || handValue.CompareTo(best) > 0)
                best = handValue;
        }
        
        return best ?? new HandValue { Rank = HandRank.HighCard };
    }

    // Legacy method for backwards compatibility
    public static HandRank BestHand(List<Card> playerCards, List<Card> communityCards)
    {
        return BestHandValue(playerCards, communityCards).Rank;
    }

    // Generate all possible 5-card combinations from a list of cards
    public static IEnumerable<List<Card>> Get5CardCombos(List<Card> cards)
    {
        int n = cards.Count;
        
        if (n < 5) yield break;
        
        for (int i = 0; i < n - 4; i++)
            for (int j = i + 1; j < n - 3; j++)
                for (int k = j + 1; k < n - 2; k++)
                    for (int l = k + 1; l < n - 1; l++)
                        for (int m = l + 1; m < n; m++)
                            yield return new List<Card> { cards[i], cards[j], cards[k], cards[l], cards[m] };
    }

    // Evaluate a poker hand and return its complete value with tie-breakers
    public static HandValue EvaluateHandValue(List<Card> cards)
    {
        if (cards == null || cards.Count != 5)
            return new HandValue { Rank = HandRank.HighCard };

        HandValue result = new HandValue { Cards = new List<Card>(cards) };

        // Check hands from strongest to weakest
        if (IsRoyalFlush(cards))
        {
            result.Rank = HandRank.RoyalFlush;
            result.TieBreakers = new List<int> { 14 }; // Ace high
        }
        else if (IsStraightFlush(cards))
        {
            result.Rank = HandRank.StraightFlush;
            result.TieBreakers = GetStraightHighCard(cards);
        }
        else if (IsFourOfAKind(cards))
        {
            result.Rank = HandRank.FourOfAKind;
            result.TieBreakers = GetFourOfAKindValues(cards);
        }
        else if (IsFullHouse(cards))
        {
            result.Rank = HandRank.FullHouse;
            result.TieBreakers = GetFullHouseValues(cards);
        }
        else if (IsFlush(cards))
        {
            result.Rank = HandRank.Flush;
            result.TieBreakers = GetHighCards(cards, 5);
        }
        else if (IsStraight(cards))
        {
            result.Rank = HandRank.Straight;
            result.TieBreakers = GetStraightHighCard(cards);
        }
        else if (IsThreeOfAKind(cards))
        {
            result.Rank = HandRank.ThreeOfAKind;
            result.TieBreakers = GetThreeOfAKindValues(cards);
        }
        else if (IsTwoPair(cards))
        {
            result.Rank = HandRank.TwoPair;
            result.TieBreakers = GetTwoPairValues(cards);
        }
        else if (IsPair(cards))
        {
            result.Rank = HandRank.Pair;
            result.TieBreakers = GetPairValues(cards);
        }
        else
        {
            result.Rank = HandRank.HighCard;
            result.TieBreakers = GetHighCards(cards, 5);
        }

        return result;
    }

    // Legacy method for backwards compatibility
    public static HandRank EvaluateHand(List<Card> cards)
    {
        return EvaluateHandValue(cards).Rank;
    }

    // Helper methods to get tie-breaker values

    private static List<int> GetFourOfAKindValues(List<Card> cards)
    {
        var groups = cards.GroupBy(c => GetCardValue(c.CardRank))
                         .OrderByDescending(g => g.Count())
                         .ThenByDescending(g => g.Key)
                         .ToList();
        
        List<int> values = new List<int>();
        values.Add(groups[0].Key); // The quad
        values.Add(groups[1].Key); // The kicker
        return values;
    }

    private static List<int> GetFullHouseValues(List<Card> cards)
    {
        var groups = cards.GroupBy(c => GetCardValue(c.CardRank))
                         .OrderByDescending(g => g.Count())
                         .ThenByDescending(g => g.Key)
                         .ToList();
        
        List<int> values = new List<int>();
        values.Add(groups[0].Key); // The trips
        values.Add(groups[1].Key); // The pair
        return values;
    }

    private static List<int> GetThreeOfAKindValues(List<Card> cards)
    {
        var groups = cards.GroupBy(c => GetCardValue(c.CardRank))
                         .OrderByDescending(g => g.Count())
                         .ThenByDescending(g => g.Key)
                         .ToList();
        
        List<int> values = new List<int>();
        values.Add(groups[0].Key); // The trips
        // Add kickers in descending order
        foreach (var g in groups.Skip(1).OrderByDescending(g => g.Key))
            values.Add(g.Key);
        return values;
    }

    private static List<int> GetTwoPairValues(List<Card> cards)
    {
        var pairs = cards.GroupBy(c => GetCardValue(c.CardRank))
                        .Where(g => g.Count() == 2)
                        .OrderByDescending(g => g.Key)
                        .Select(g => g.Key)
                        .ToList();
        
        var kicker = cards.GroupBy(c => GetCardValue(c.CardRank))
                         .Where(g => g.Count() == 1)
                         .Select(g => g.Key)
                         .FirstOrDefault();
        
        List<int> values = new List<int>();
        values.AddRange(pairs); // High pair, then low pair
        values.Add(kicker); // Kicker
        return values;
    }

    private static List<int> GetPairValues(List<Card> cards)
    {
        var pairValue = cards.GroupBy(c => GetCardValue(c.CardRank))
                            .Where(g => g.Count() == 2)
                            .Select(g => g.Key)
                            .FirstOrDefault();
        
        var kickers = cards.GroupBy(c => GetCardValue(c.CardRank))
                          .Where(g => g.Count() == 1)
                          .OrderByDescending(g => g.Key)
                          .Select(g => g.Key)
                          .ToList();
        
        List<int> values = new List<int> { pairValue };
        values.AddRange(kickers);
        return values;
    }

    private static List<int> GetHighCards(List<Card> cards, int count)
    {
        return cards.Select(c => GetCardValue(c.CardRank))
                   .OrderByDescending(v => v)
                   .Take(count)
                   .ToList();
    }

    private static List<int> GetStraightHighCard(List<Card> cards)
    {
        var ranks = cards.Select(c => GetCardValue(c.CardRank)).OrderBy(x => x).ToList();
        
        // Check for Ace-low straight (A-2-3-4-5)
        if (ranks.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 }))
            return new List<int> { 5 }; // Ace-low straight is worth 5 high
        
        // Normal straight - return highest card
        return new List<int> { ranks.Max() };
    }

    private static int GetCardValue(Card.Rank rank)
    {
        return (int)rank;
    }

    // Hand checking methods (unchanged)
    
    private static bool IsFlush(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        return cards.GroupBy(c => c.CardSuit).Any(g => g.Count() >= 5);
    }

    private static bool IsStraight(List<Card> cards)
    {
        var ranks = cards.Select(c => (int)c.CardRank).OrderBy(x => x).ToList();
        
        // Check for Ace-low straight (A-2-3-4-5)
        if (ranks.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 }))
            return true;
        
        // Check for normal straight
        for (int i = 0; i < ranks.Count - 1; i++)
        {
            if (ranks[i + 1] != ranks[i] + 1)
                return false;
        }
        
        return true;
    }

    private static bool IsRoyalFlush(List<Card> cards)
    {
        return IsStraightFlush(cards) && cards.Any(c => c.CardRank == Card.Rank.Ace);
    }

    private static bool IsStraightFlush(List<Card> cards)
    {
        return IsFlush(cards) && IsStraight(cards);
    }

    private static bool IsFourOfAKind(List<Card> cards)
    {
        return cards.GroupBy(c => c.CardRank).Any(g => g.Count() == 4);
    }

    private static bool IsFullHouse(List<Card> cards)
    {
        var groups = cards.GroupBy(c => c.CardRank)
                         .Select(g => g.Count())
                         .OrderByDescending(x => x)
                         .ToList();
        return groups.SequenceEqual(new List<int> { 3, 2 });
    }

    private static bool IsThreeOfAKind(List<Card> cards)
    {
        return cards.GroupBy(c => c.CardRank).Any(g => g.Count() == 3);
    }

    private static bool IsTwoPair(List<Card> cards)
    {
        return cards.GroupBy(c => c.CardRank).Where(g => g.Count() == 2).Count() == 2;
    }

    private static bool IsPair(List<Card> cards)
    {
        return cards.GroupBy(c => c.CardRank).Any(g => g.Count() == 2);
    }
}