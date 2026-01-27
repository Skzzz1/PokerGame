using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TurnManager : MonoBehaviour
{
    //4 players for now
    [Header("Players")]
    public int playerCount = 4;
    public int currentPlayer;
    public int dealerButton;
    public int[] stacks;
    public int[] bets;
    public bool[] folded;

    [Header("Betting")]
    public int pot;
    public int currentBet;
    public int smallBlind = 10;
    public int bigBlind = 20;
    public int minimumRaise = 20;

    [Header("Round Management")]
    public int lastRaiserIndex = -1;// track last better or raiser
    public int activePlayers; // Players who haven't folded
    // private bool anyBetThisRound;// if everyone chooses to check
    private bool[] hasActedThisRound;


    
    [Header("Hand Stage")]
    public Street currentStreet;
    public enum Street { Preflop, Flop, Turn, River, Showdown, HandComplete }
    
    // public List<string> communityCards; // Placeholder for actual card objects

    [Header("Card Management")]
    public PokerHandManager handManager;  // Reference to card system
    public PlayerManager playerManager; //reference to all players
    
    
    TurnUI turnUI;

    //Initialization, everyone gets 1000
    void Awake()
    {
        stacks = new int[playerCount];
        bets = new int[playerCount];
        folded = new bool[playerCount];
        dealerButton = 0;
        activePlayers = playerCount;

        // Initialize PlayerManager and players
        playerManager?.InitializePlayers();

        // Set starting stacks
        for (int i = 0; i < playerCount; i++)
            stacks[i] = 1000;
        
        //if no ui find ui first
        if (turnUI == null)
        {
            turnUI = FindFirstObjectByType<TurnUI>();

            if (turnUI == null)  //still null after searching
            {
                Debug.LogError("TurnUI not found in scene! Add a TurnUI component.");
                return;  //end game
            }
        }

        if (handManager == null)
        {
            handManager = FindFirstObjectByType<PokerHandManager>();
            if (handManager == null)  //still null after searching
            {
                Debug.LogError("Poker Hand Manager not found in scene! Add a hand manager component.");
                return;  //end game
            }
        }
        if (playerManager == null)
        {
            playerManager = FindFirstObjectByType<PlayerManager>();
            if (playerManager == null)  //still null after searching
            {
                Debug.LogError("Poker player Manager not found in scene! Add a player manager component.");
                return;  //end game
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // called after awake
    void Start()
    {
        StartNewHand();
    }

    public void StartNewHand()
    {
        handManager.FlipBoardCards();
        // anyBetThisRound = false;
        if (activePlayers <= 0)
        {
            Debug.LogError("Cannot start hand: no active players");
            return;
        }

        //reset for new hand
        pot = 0;
        turnUI.SetPot(pot);
        currentBet = 0;
        lastRaiserIndex = -1;
        activePlayers = 0;
        currentStreet = Street.Preflop;
        currentPlayer = (dealerButton+3)%playerCount;//skip sb and bb

        hasActedThisRound = new bool[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            //initialize all bets
            bets[i] = 0;
            folded[i] = stacks[i] == 0; //auto-fold if no chips
            if (!folded[i]) activePlayers++;
            hasActedThisRound[i] = false;
        }
        if (handManager != null)
        {
            handManager.DealHoleCards(playerCount, folded);
        }
        // Update player UIs (turn indicators, dealer button, stacks, bets)
        if (playerManager != null)
        {
            playerManager.ResetAllPlayers();
            UpdateAllPlayerUI();
        }

        PostBlinds();
        // Update UI after blinds
        if (playerManager != null)
        {
            UpdateAllPlayerUI();
        }
        SkipFoldedPlayers();
        StartCoroutine(StartTurnWithDelay(currentPlayer, 1f));
    }
    IEnumerator StartTurnWithDelay(int player, float delay)
    {
        yield return new WaitForSeconds(delay);  // wait 1 second
        StartTurn(player);                        // call your function after delay
    }

    void RotateDealerButton()
    {
        int startPos = dealerButton;

        do
        {
            dealerButton = (dealerButton+1) % playerCount;
            if (dealerButton == startPos && stacks[dealerButton] == 0)
            {
                Debug.LogError("All players are broke! Game cannot continue.");
                break;
            }
        } while(stacks[dealerButton]==0);
    }

    //deal with blinds first
    void PostBlinds()
    {
        // anyBetThisRound = true;
        int sbPos = (dealerButton + 1) % playerCount;
        int bbPos = (dealerButton + 2) % playerCount;
        lastRaiserIndex = bbPos;

        int sbAmount = Mathf.Min(stacks[sbPos],smallBlind);
        CommitChips(sbPos,sbAmount);
        int bbAmount = Mathf.Min(stacks[bbPos],bigBlind);
        CommitChips(bbPos,bbAmount);

        currentBet = bbAmount;
    }
    //new turn for player
    public void StartTurn(int playerIndex)
    {
        // anyBetThisRound = false;
        currentPlayer = playerIndex;

        if(folded[currentPlayer] || stacks[currentPlayer]==0)//fold or All-in ed
        {
            NextPlayer();
            return;
        }
        UpdateAllPlayerUI();

        int maxAmount = stacks[currentPlayer];
        bool betExists = currentBet > bets[playerIndex];
        int callAmount = currentBet - bets[currentPlayer];

        bool isHuman = playerManager != null && playerManager.players[currentPlayer].isHuman;

        if(isHuman)
        {
            turnUI.ShowActions(betExists, callAmount,minimumRaise, maxAmount);
        }
        else
        {
            //bots
            BotStrategy bot = GetBotStrategy(currentPlayer);
            if (bot != null)
            {
                BotDecisionContext context = CreateBotContext(currentPlayer);
                bot.MakeDecision(context);
            }
            else
            {
                Debug.LogError($"Player {currentPlayer} is a bot but has no BotStrategy component!");
                // Default to fold to prevent game freeze
                Fold();
            }
        }
        
    }

    //retrieve bots strategy
    private BotStrategy GetBotStrategy(int playerIndex)
    {
        if (playerManager == null) return null;
        
        PokerPlayer player = playerManager.GetPlayer(playerIndex);
        if (player == null) return null;
        
        return player.GetComponent<BotStrategy>();
    }//maybe a loop here player get bot, bot get player?

    //pack info for bot
    private BotDecisionContext CreateBotContext(int playerIndex)
    {
        BotDecisionContext context = new BotDecisionContext();
        
        // Player info
        context.playerIndex = playerIndex;
        context.myStack = stacks[playerIndex];
        context.myBet = bets[playerIndex];
        
        // Game state
        context.allStacks = (int[])stacks.Clone();
        context.allBets = (int[])bets.Clone();
        context.folded = (bool[])folded.Clone();
        context.pot = pot;
        context.currentBet = currentBet;
        context.activePlayers = activePlayers;
        
        // Betting info
        context.callAmount = currentBet - bets[playerIndex];
        context.minRaise = currentBet + minimumRaise;
        context.canCheck = (currentBet == bets[playerIndex]);
        
        // Cards
        if (handManager != null)
        {
            context.holeCards = handManager.GetPlayerHand(playerIndex);
            context.communityCards = handManager.GetCommunityCards();
        }
        // else
        // {
        //     context.holeCards = new List<Card>();
        //     context.communityCards = new List<Card>();
        // }
        
        // Street info
        context.currentStreet = currentStreet.ToString();
        
        // Reference to execute actions
        context.turnManager = this;
        
        return context;
    }
    

    public void Bet(int amount)
    {
        // anyBetThisRound = true;
        //betting when there's no current bet
        amount = Mathf.Max(amount, bigBlind);
        ApplyBet(amount);
        lastRaiserIndex = currentPlayer;
        hasActedThisRound[currentPlayer] = true;
        UpdateAllPlayerUI();
        Invoke(nameof(NextPlayer), 0.5f);
    }

    public void Raise(int amount)
    {
        // anyBetThisRound = true;  
        amount = Mathf.Max(amount, currentBet + minimumRaise);
        int amountToAdd = amount - bets[currentPlayer];
        ApplyBet(amountToAdd);
        lastRaiserIndex = currentPlayer;
        hasActedThisRound[currentPlayer] = true;
        UpdateAllPlayerUI();
        Invoke(nameof(NextPlayer), 0.5f);
    }

    public void Call()
    {
        int callAmount = currentBet - bets[currentPlayer];
        ApplyBet(callAmount);
        hasActedThisRound[currentPlayer] = true;
        UpdateAllPlayerUI();
        Invoke(nameof(NextPlayer), 0.5f);
    }

    public void Check()
    {
        hasActedThisRound[currentPlayer] = true;
        // Can only check if no bet to call
        if (bets[currentPlayer] == currentBet)
        {
            Invoke(nameof(NextPlayer), 0.5f);
        }
    }

    public void Fold()
    {
        folded[currentPlayer] = true;
        activePlayers--;
        UpdateAllPlayerUI();
        hasActedThisRound[currentPlayer] = true;
        
        if (activePlayers == 1)
        {
            EndBettingRound();
            return;
        }
        
        Invoke(nameof(NextPlayer), 0.5f);
    }

    void NextPlayer()
    {
        // First check if betting round is already complete
        if (IsBettingRoundComplete())
        {
            EndBettingRound();
            return;
        }
        
        int safetyCounter = 0;
        
        do
        {
            currentPlayer = (currentPlayer + 1) % playerCount;
            safetyCounter++;
            
            // Prevent infinite loop
            if (safetyCounter > playerCount)
            {
                Debug.LogError("NextPlayer: Infinite loop detected! Forcing end of betting round.");
                EndBettingRound();
                return;
            }
            
            // Check again after moving to next player
            if (IsBettingRoundComplete())
            {
                EndBettingRound();
                return;
            }
            
        } while (folded[currentPlayer] || stacks[currentPlayer] == 0);
        
        StartTurn(currentPlayer);
    }

    void SkipFoldedPlayers()
    {
        int safetyCounter = 0;
        
        while (folded[currentPlayer] || stacks[currentPlayer] == 0)
        {
            currentPlayer = (currentPlayer + 1) % playerCount;
            safetyCounter++;
            
            if (safetyCounter >= playerCount)
            {
                Debug.LogError("SkipFoldedPlayers: No players can act! This shouldn't happen.");
                // Force end the betting round
                EndBettingRound();
                return;
            }
        }
    }

    void ApplyBet(int amount)
    {
        CommitChips(currentPlayer,amount);
        currentBet = bets[currentPlayer];
    }

    void CommitChips(int pos, int amount)
    {
        //making sure pos is within range
        if(pos < 0 || pos >= stacks.Length)
        {
            Debug.LogError($"CommitChips: invalid pos {pos}, stacks.Length={stacks.Length}");
            return;
        }
        //making sure player can afford raise
        amount = Mathf.Min(amount, stacks[pos]);

        stacks[pos] -= amount;
        bets[pos] += amount;
        pot += amount;
        turnUI.SetPot(pot);
    }

    // handles all-in players
    bool IsBettingRoundComplete()
    {
        // Count how many players still need to act
        int playersWhoCanAct = 0;
        int playersWhoHaveActed = 0;
        
        for (int i = 0; i < playerCount; i++)
        {
            // Skip folded players
            if (folded[i]) continue;
            
            // All-in players don't need to act,but bet should match currentBet or be their full stack
            if (stacks[i] == 0)
            {
                continue;
            }
            
            playersWhoCanAct++;
            
            // Check if they've acted and matched the current bet
            if (hasActedThisRound[i] && bets[i] == currentBet)
            {
                playersWhoHaveActed++;
            }
        }
        
        // If no one can act (everyone folded or all-in), round is complete
        if (playersWhoCanAct == 0)
            return true;
        
        // If everyone who can act has acted and matched the bet, round is complete
        if (playersWhoHaveActed == playersWhoCanAct)
            return true;
        
        return false;
    }

    void EndBettingRound()
    {
        Debug.Log($"Betting round complete. Pot: {pot}");
        
        // Count active players (not folded)
        int activeCount = 0;
        for (int i = 0; i < playerCount; i++)
        {
            if (!folded[i])
                activeCount++;
        }
        
        if (activeCount == 1)
        {
            // Only one player left, they win
            int winner = GetLastActivePlayer();
            Debug.Log($"Player {winner} wins {pot} chips (everyone else folded)");
            stacks[winner] += pot;
            playerManager.players[winner].WinPot(pot);
            turnUI.SetPot(0);
            pot = 0;

            Invoke(nameof(StartNewHand), 4f);
        }
        else
        {
            // Continue to next street or showdown
            Debug.Log($"Ready for next street. {activeCount} players remain.");
            StartNextStreet();
        }
    }

    int GetLastActivePlayer()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (!folded[i])
                return i;
        }
        return -1;
    }

    public void StartNextStreet()
    {
        switch(currentStreet)
        {
            case Street.Preflop:
                currentStreet = Street.Flop;
                handManager.DealFlop();
                break;
            case Street.Flop:
                currentStreet = Street.Turn;
                handManager.DealTurn();
                break;
            case Street.Turn:
                currentStreet = Street.River;
                handManager.DealRiver();
                break;
            case Street.River:
                currentStreet = Street.Showdown;
                Showdown();
                return;
        }

        // Reset bets for the new street
        for(int i = 0; i < playerCount; i++)
            bets[i] = 0;

        currentBet = 0;
        lastRaiserIndex = -1;
        hasActedThisRound = new bool[playerCount];
        
        // NEW: Check if anyone can actually bet
        bool anyoneCanBet = false;
        for (int i = 0; i < playerCount; i++)
        {
            if (!folded[i] && stacks[i] > 0)
            {
                anyoneCanBet = true;
                break;
            }
        }
        
        // If everyone is all-in or folded, skip straight to next street/showdown
        if (!anyoneCanBet)
        {
            Debug.Log("All players all-in. Running out the board...");
            
            // Short delay then continue
            if (currentStreet == Street.Showdown)
            {
                Showdown();
            }
            else
            {
                Invoke(nameof(StartNextStreet), 1f);
            }
            return;
        }
        
        // Find first player who can act (not folded, has chips)
        currentPlayer = (dealerButton + 1) % playerCount;
        SkipFoldedPlayers();
        
        // Start their turn
        StartTurn(currentPlayer);
    }


    void Showdown()
    {
        Debug.Log("Showdown!");

        // Reveal all player cards
        for(int i = 0; i < playerCount; i++)
        {
            if(!folded[i])
                handManager.RevealPlayerCards(i);
        }

        List<int> winners = new List<int>();
        HandEvaluator.HandValue bestHandValue = null;

        // Evaluate each player's best hand
        for (int i = 0; i < playerCount; i++)
        {
            if (folded[i]) continue;

            // Get player's best 5-card hand using the new method
            HandEvaluator.HandValue playerHandValue = HandEvaluator.BestHandValue(
                handManager.GetPlayerHand(i), 
                handManager.GetCommunityCards()
            );

            Debug.Log($"Player {i}: {playerHandValue}");

            // Compare with current best hand
            if (bestHandValue == null || playerHandValue.CompareTo(bestHandValue) > 0)
            {
                // New best hand found
                bestHandValue = playerHandValue;
                winners.Clear();
                winners.Add(i);
            }
            else if (playerHandValue.CompareTo(bestHandValue) == 0)
            {
                // Tie with current best hand
                winners.Add(i);
            }
        }

        // Split pot among winners
        int share = pot / winners.Count;
        int remainder = pot % winners.Count; // Handle odd chips
        
        for (int idx = 0; idx < winners.Count; idx++)
        {
            int w = winners[idx];
            int winAmount = share + (idx == 0 ? remainder : 0); // First winner gets remainder
            stacks[w] += winAmount;
            playerManager.players[w].WinPot(winAmount);
        }
        
        pot = 0;
        turnUI.SetPot(0);

        // Log the results
        string winnerText = string.Join(", ", winners.Select(w => $"Player {w}"));
        if (winners.Count > 1)
            Debug.Log($"{winnerText} tie with {bestHandValue.Rank}. Each wins {share} chips.");
        else
            Debug.Log($"{winnerText} wins with {bestHandValue.Rank}!");

        currentStreet = Street.HandComplete;

        // Rotate dealer button to next player
        RotateDealerButton();
        
        // Start next hand
        Invoke(nameof(StartNewHand), 10f);
    }

    void UpdateAllPlayerUI()
    {
        if (playerManager == null) return;

        for (int i = 0; i < playerCount; i++)
        {
            bool isActive = (i == currentPlayer);
            bool isDealer = (i == dealerButton);
            playerManager.UpdatePlayerState(i, stacks[i], bets[i], folded[i], isActive, isDealer);
        }
    }


    

    // Update is called once per frame
    void Update()
    {
        
    }
}
