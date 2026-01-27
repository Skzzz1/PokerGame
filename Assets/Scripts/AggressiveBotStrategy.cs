using UnityEngine;

public class AggressiveBotStrategy : BotStrategy
{
    [Header("Aggressive Settings")]
    [Range(0f, 1f)]
    public float aggressionLevel = 0.7f;  // How often to bet/raise
    
    public override BotAction DecideAction(BotDecisionContext context)
    {
        float handStrength = EstimateHandStrength(context);
        float potOdds = CalculatePotOdds(context);
        
        // Aggressive: lower threshold for betting
        bool shouldBeAggressive = handStrength > 0.3f || Random.value < aggressionLevel;
        
        // If can check for free, sometimes bet anyway
        if (context.canCheck)
        {
            if (shouldBeAggressive && Random.value < 0.6f)
            {
                // Bet to apply pressure
                int betAmount = Mathf.Max(context.pot / 2, context.turnManager.bigBlind);
                betAmount = Mathf.Min(betAmount, context.myStack);
                return new BotAction(BotActionType.Bet, betAmount);
            }
            return new BotAction(BotActionType.Check);
        }
        
        // Facing a bet
        if (context.callAmount > 0)
        {
            // Strong hand or aggressive mood: raise
            if (handStrength > 0.6f || (shouldBeAggressive && handStrength > 0.4f))
            {
                int raiseAmount = context.currentBet + context.turnManager.minimumRaise;
                raiseAmount = Mathf.Min(raiseAmount, context.myStack + context.myBet);
                return new BotAction(BotActionType.Raise, raiseAmount);
            }
            
            // Medium hand: call
            if (handStrength > 0.3f)
            {
                return new BotAction(BotActionType.Call);
            }
            
            // Weak hand: fold
            return new BotAction(BotActionType.Fold);
        }
        
        // Default: check
        return new BotAction(BotActionType.Check);
    }
}
