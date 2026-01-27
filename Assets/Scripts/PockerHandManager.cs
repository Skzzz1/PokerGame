using UnityEngine;
using System.Collections.Generic;

public class PokerHandManager : MonoBehaviour
{
    [Header("References")]
    public CardSpriteManager spriteManager;
    
    [Header("Community Card Displays")]
    public CardDisplay flopCard1;
    public CardDisplay flopCard2;
    public CardDisplay flopCard3;
    public CardDisplay turnCard;
    public CardDisplay riverCard;
    
    [Header("Player Hand Displays")]
    public CardDisplay[] PlayerCards;  // 2 cards
    public CardDisplay[] Bot1Cards;  
    public CardDisplay[] Bot2Cards;  
    public CardDisplay[] Bot3Cards;
    
    private Deck deck;
    private List<Card> communityCards;
    private List<Card>[] playerHands;
    
    void Start()
    {
        deck = new Deck();
        communityCards = new List<Card>();
        playerHands = new List<Card>[4];
        
        for (int i = 0; i < 4; i++)
        {
            playerHands[i] = new List<Card>();
        }
    }
    
    // Called by TurnManager.StartNewHand()
    public void DealHoleCards(int playerCount, bool[] folded)
    {
        deck.Reset();  // New shuffled deck
        
        // Deal 2 cards to each active player
        for (int i = 0; i < playerCount; i++)
        {
            CardDisplay[] displays = GetPlayerCardDisplays(i);
            if (folded[i]) 
            {
                bool isFaceUp = false;
                displays[0].SetCard(playerHands[i][0], isFaceUp);
                displays[1].SetCard(playerHands[i][1], isFaceUp);
                continue;
            }
            
            playerHands[i].Clear();
            playerHands[i].Add(deck.DrawCard());
            playerHands[i].Add(deck.DrawCard());
            
            // Show cards face-up for player 0 (human), face-down for others
            bool showCards = (i == 0);
            
            if (displays == null || displays.Length < 2)
            {
                Debug.LogError($"Player {i} CardDisplay array not assigned or too small!");
                continue; // skip dealing to this player
            }

            displays[0].SetCard(playerHands[i][0], showCards);
            displays[1].SetCard(playerHands[i][1], showCards);
            
            Debug.Log($"Player {i}: {playerHands[i][0]}, {playerHands[i][1]}");
        }
    }
    
    public List<Card> DealFlop()
    {
        communityCards.Clear();
        
        // // Burn 1 card
        // deck.DrawCard();
        
        // Deal 3 cards
        for (int i = 0; i < 3; i++)
        {
            Card card = deck.DrawCard();
            communityCards.Add(card);
        }
        
        // Display them
        flopCard1.SetCard(communityCards[0], true);
        flopCard2.SetCard(communityCards[1], true);
        flopCard3.SetCard(communityCards[2], true);
        
        Debug.Log($"FLOP: {communityCards[0]}, {communityCards[1]}, {communityCards[2]}");
        
        return new List<Card>(communityCards);
    }
    
    public Card DealTurn()
    {
        
        // Deal turn card
        Card card = deck.DrawCard();
        communityCards.Add(card);
        
        turnCard.SetCard(card, true);
        
        Debug.Log($"TURN: {card}");
        
        return card;
    }
    
    public Card DealRiver()
    {
        // // Burn 1 card
        // deck.DrawCard();
        
        // Deal river card
        Card card = deck.DrawCard();
        communityCards.Add(card);
        
        riverCard.SetCard(card, true);
        
        Debug.Log($"RIVER: {card}");
        
        return card;
    }
    
    public void RevealPlayerCards(int playerIndex)
    {
        if(playerIndex == 0) return;
        CardDisplay[] displays = GetPlayerCardDisplays(playerIndex);
        displays[0].Flip();
        displays[1].Flip();
    }

    public void FlipBoardCards ()
    {
        flopCard1.Flip();
        flopCard2.Flip();
        flopCard3.Flip();
        turnCard.Flip();
        riverCard.Flip();
    
    }
    
    public List<Card> GetPlayerHand(int playerIndex)
    {
        return playerHands[playerIndex];
    }
    
    public List<Card> GetCommunityCards()
    {
        return communityCards;
    }
    
    private CardDisplay[] GetPlayerCardDisplays(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return PlayerCards;
            case 1: return Bot1Cards;
            case 2: return Bot2Cards;
            case 3: return Bot3Cards;
            default: return null;
        }
    }
}
