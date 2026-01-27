using UnityEngine;

public enum BotActionType
{
    Fold,
    Check,
    Call,
    Bet,
    Raise
}

public class BotAction
{
    public BotActionType actionType;
    public int amount;  // For Bet/Raise actions
    
    public BotAction(BotActionType type, int amt = 0)
    {
        actionType = type;
        amount = amt;
    }
    
    public override string ToString()
    {
        if (actionType == BotActionType.Bet || actionType == BotActionType.Raise)
        {
            return $"{actionType} ${amount}";
        }
        return actionType.ToString();
    }
}
