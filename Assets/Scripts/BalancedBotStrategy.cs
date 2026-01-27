using UnityEngine;

public class BalancedBotStrategy : BotStrategy
{
    [Header("Balanced Settings")]
    [Range(0f, 1f)]
    public float bluffFrequency = 0.15f;  // How often to bluff
    
    public override BotAction DecideAction(BotDecisionContext context)
    {
        float handStrength = EstimateHandStrength(context);
        float potOdds = CalculatePotOdds(context);
        bool shouldBluff = Random.value < bluffFrequency;
        
        // If can check for free
        if (context.canCheck)
        {
            // Bet with strong hands or as bluff
            if (handStrength > 0.6f || shouldBluff)
            {
                int betAmount = Mathf.RoundToInt(context.pot * 0.66f);
                betAmount = Mathf.Max(betAmount, context.turnManager.bigBlind);
                betAmount = Mathf.Min(betAmount, context.myStack);
                return new BotAction(BotActionType.Bet, betAmount);
            }
            return new BotAction(BotActionType.Check);
        }
        
        // Facing a bet
        if (context.callAmount > 0)
        {
            // Strong hand: usually raise
            if (handStrength > 0.75f)
            {
                if (Random.value < 0.7f)  // 70% raise, 30% slow-play (call)
                {
                    int raiseAmount = context.currentBet + context.turnManager.minimumRaise;
                    raiseAmount = Mathf.Min(raiseAmount, context.myStack + context.myBet);
                    return new BotAction(BotActionType.Raise, raiseAmount);
                }
                return new BotAction(BotActionType.Call);
            }
            
            // Medium hand: consider pot odds
            if (handStrength > 0.45f)
            {
                // Good pot odds or decent hand: call
                if (potOdds < 0.33f || handStrength > 0.55f)
                {
                    return new BotAction(BotActionType.Call);
                }
            }
            
            // Bluff raise occasionally
            if (shouldBluff && context.activePlayers <= 2)
            {
                int raiseAmount = context.currentBet + context.turnManager.minimumRaise;
                raiseAmount = Mathf.Min(raiseAmount, context.myStack + context.myBet);
                return new BotAction(BotActionType.Raise, raiseAmount);
            }
            
            // Weak hand: fold
            return new BotAction(BotActionType.Fold);
        }
        
        return new BotAction(BotActionType.Check);
    }
}
