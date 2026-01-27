using System.Collections.Generic;
using UnityEngine;

public class BotDecisionContext
{
    // Player info
    public int playerIndex;
    public int myStack;
    public int myBet;
    
    // Game state
    public int[] allStacks;
    public int[] allBets;
    public bool[] folded;
    public int pot;
    public int currentBet;
    public int activePlayers;
    
    // Betting info
    public int callAmount;// How much to call
    public int minRaise;// Minimum raise amount
    public bool canCheck;// Can check for free
    
    // Cards (bot's hand)
    public List<Card> holeCards;
    public List<Card> communityCards;
    
    // Street info
    public string currentStreet;  // "Preflop", "Flop", "Turn", "River"
    
    // Reference to execute action
    public TurnManager turnManager;
}
