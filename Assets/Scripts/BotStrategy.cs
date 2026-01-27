using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BotStrategy : MonoBehaviour
{
    [Header("Bot Settings")]
    [Tooltip("Bot personality name")]
    public string botName = "Bot";
    
    [Tooltip("Delay before bot acts (seconds) - makes it feel more natural")]
    [Range(0.5f, 3f)]
    public float decisionDelay = 2f;
    
    protected PokerPlayer pokerPlayer;
    
    protected virtual void Awake()
    {
        pokerPlayer = GetComponent<PokerPlayer>();//O(1) as attached to pokerplayer object
    }
    
    private BotDecisionContext pendingContext;
    // WHEN: called by TurnManager when it's this bot's turn, time delay purpose
    public void MakeDecision(BotDecisionContext context)
    {

        Invoke("ExecuteDecision", decisionDelay);
        Debug.Log($"[BOT DECIDE] Player {context.playerIndex} Street={context.currentStreet} Bet={context.currentBet}");
        // Store context for delayed execution
        pendingContext = context;
    }

    // WHAT: Main logic decision method - override this in specific strategies
    public abstract BotAction DecideAction(BotDecisionContext context);
    
    
    // HOW: connect to the action
    private void ExecuteDecision()
    {
        if (pendingContext == null) return;
        
        // Get decision from strategy
        BotAction action = DecideAction(pendingContext);
        
        Debug.Log($"{botName} (Player {pendingContext.playerIndex}): {action}");
        
        // Execute the action
        ExecuteAction(action, pendingContext);
        
        pendingContext = null;
    }
    
    // Execute the bot's chosen action
    protected void ExecuteAction(BotAction action, BotDecisionContext context)
    {
        switch (action.actionType)
        {
            case BotActionType.Fold:
                context.turnManager.Fold();
                break;
                
            case BotActionType.Check:
                context.turnManager.Check();
                break;
                
            case BotActionType.Call:
                context.turnManager.Call();
                break;
                
            case BotActionType.Bet:
                context.turnManager.Bet(action.amount);
                break;
                
            case BotActionType.Raise:
                context.turnManager.Raise(action.amount);
                break;
        }
    }
    
    ///// Helper methods for strategies
    
    // Calculate pot odds, A call is profitable if chance of winning ≥ pot odds.
    protected float CalculatePotOdds(BotDecisionContext context)
    {
        if (context.callAmount == 0) return 0;
        return (float)context.callAmount / (context.pot + context.callAmount);
    }
    
    //Estimate rough hand strength (0.0 to 1.0)
    //placeholder - real poker AI would use proper hand evaluation
    protected float EstimateHandStrength(BotDecisionContext context)
    {
        
        if (context.holeCards == null || context.holeCards.Count < 2)
            return 0.5f;
        
        // Count active opponents
        int numOpponents = context.activePlayers - 1;
        if (numOpponents < 1) numOpponents = 1;
        
        // Use Monte Carlo simulation for better estimate
        Debug.Log($"[MC START] Player {context.playerIndex} Street={context.currentStreet}");
        float strength = HandStrengthCalculator.EstimateHandStrength(
            context.holeCards,
            context.communityCards ?? new List<Card>(),
            numOpponents,
            simulations: 500
        );
        
        return Mathf.Clamp01(strength);
    }
}
