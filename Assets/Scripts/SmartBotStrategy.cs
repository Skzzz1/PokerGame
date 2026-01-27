using UnityEngine;

public class SmartBalancedBotStrategy : BotStrategy
{
    [Header("Smart Balanced Settings")]
    [Range(0f, 1f)] public float bluffFrequency = 0.12f;
    [Range(0f, 1f)] public float valueRaiseEquity = 0.72f;
    [Range(0f, 1f)] public float callBuffer = 0.05f; // looseness vs pure pot odds

    public override BotAction DecideAction(BotDecisionContext context)
    {
        float equity = EstimateHandStrength(context);
        float potOdds = CalculatePotOdds(context);
        bool bluff = Random.value < bluffFrequency;

        // --- FREE ACTION (CHECK OPTION) ---
        if (context.canCheck)
        {
            if (equity > 0.6f || bluff)
            {
                int bet = Mathf.Clamp(
                    Mathf.RoundToInt(context.pot * 0.66f),
                    context.turnManager.bigBlind,
                    context.myStack
                );
                return new BotAction(BotActionType.Bet, bet);
            }

            return new BotAction(BotActionType.Check);
        }

        // --- FACING A BET ---

        // VALUE RAISE (clearly +EV even if called)
        if (equity >= valueRaiseEquity)
        {
            int raise = context.currentBet + context.turnManager.minimumRaise;
            raise = Mathf.Min(raise, context.myStack + context.myBet);
            return new BotAction(BotActionType.Raise, raise);
        }

        // positive EV, call
        if (equity >= potOdds + callBuffer)
        {
            return new BotAction(BotActionType.Call);
        }

        // SEMI-BLUFF (heads-up or late street)
        if (bluff && context.activePlayers <= 2 && equity > 0.25f)
        {
            int raise = context.currentBet + context.turnManager.minimumRaise;
            raise = Mathf.Min(raise, context.myStack + context.myBet);
            return new BotAction(BotActionType.Raise, raise);
        }

        return new BotAction(BotActionType.Fold);
    }
}

