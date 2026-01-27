using UnityEngine;

public class TightBotStrategy : BotStrategy//only plays strong hand
{
    [Header("Tight Settings")]
    [Range(0f, 1f)]
    public float playThreshold = 0.6f;  // Minimum hand strength to play
    
    public override BotAction DecideAction(BotDecisionContext context)
    {
        float handStrength = EstimateHandStrength(context);
        
        // If can check for free, take it
        if (context.canCheck)
        {
            // Only bet with strong hands
            if (handStrength > 0.75f)
            {
                int betAmount = context.pot / 2;
                betAmount = Mathf.Min(betAmount, context.myStack);
                return new BotAction(BotActionType.Bet, betAmount);
            }
            return new BotAction(BotActionType.Check);
        }
        
        // Facing a bet
        if (context.callAmount > 0)
        {
            // Only continue with strong hands
            if (handStrength > playThreshold)
            {
                // Raise with very strong hands
                if (handStrength > 0.8f)
                {
                    int raiseAmount = context.currentBet + context.turnManager.minimumRaise;
                    raiseAmount = Mathf.Min(raiseAmount, context.myStack + context.myBet);
                    return new BotAction(BotActionType.Raise, raiseAmount);
                }
                
                // Call with good hands
                return new BotAction(BotActionType.Call);
            }
            
            // Fold everything else
            return new BotAction(BotActionType.Fold);
        }
        
        return new BotAction(BotActionType.Check);
    }
}
