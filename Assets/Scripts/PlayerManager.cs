using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PokerPlayer[] players = new PokerPlayer[4];

    // Called from Awake in TurnManager
    public void InitializePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                bool isHuman = (i == 0);
                string name = "";
                if(i == 0)
                {
                    name = "Player";
                } else if(i == 1)
                {
                    name = "BOBO";
                } else if(i == 2)
                {
                    name = "MIMI";
                }else if(i == 3)
                {
                    name = "DADA";
                }
                players[i].Initialize(i, name, isHuman);
            }
        }
    }

    public void UpdatePlayerState(int index, int stack, int bet, bool folded, bool isActive, bool isDealer)
    {
        if (index >= 0 && index < players.Length && players[index] != null)
        {
            players[index].SetState(stack, bet, folded, isActive, isDealer);
        }
    }

    public void ResetAllPlayers()
    {
        foreach (var player in players)
            player?.ResetForNewHand();
    }

    public PokerPlayer GetPlayer(int index)
    {
        return (index >= 0 && index < players.Length) ? players[index] : null;
    }
}
