using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PokerPlayer : MonoBehaviour
{
    public int playerIndex;
    public string playerName = "Player";
    public bool isHuman = false;

    public GameObject dealerButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stackText;
    public TextMeshProUGUI betText;
    public Color32 activeTurnColor = new Color32(0x36, 0xE0, 0xF1, 0xFF);
    public Color inactiveTurnColor = new Color32(77, 77, 77, 255);
    public Color foldedColor = new Color32(128, 25, 25, 255);

    private int currentStack;
    private int currentBet;
    private bool hasFolded;

    // Called from PlayerManager during Awake
    public void Initialize(int index, string name, bool human)
    {
        playerIndex = index;
        playerName = name;
        isHuman = human;

        if (nameText != null)
            nameText.text = playerName;

        if (dealerButton != null)
            dealerButton.SetActive(false);

        ResetForNewHand();
    }

    public void SetState(int stack, int bet, bool folded, bool isActive, bool isDealer)
    {
        currentStack = stack;
        currentBet = bet;
        hasFolded = folded;

        // Update UI
        UpdateStackDisplay();
        UpdateBetDisplay();
        UpdateFoldedState();

        SetActive(isActive);
        SetDealer(isDealer);
    }

    void UpdateStackDisplay()
    {
        if (stackText != null)
        {
            stackText.text = $"${currentStack}";
            stackText.color = currentStack == 0 ? Color.red :
                              currentStack < 100 ? Color.yellow :
                              Color.white;
        }
    }

    void UpdateBetDisplay()
    {
        if (betText != null)
        {
            if (currentBet > 0)
            {
                betText.text = $"Bet: ${currentBet}";
                betText.gameObject.SetActive(true);
            }
            else
            {
                betText.gameObject.SetActive(false);
            }
        }
    }

    void UpdateFoldedState()
    {
        if (hasFolded)
        {
            if (nameText != null) nameText.color = foldedColor;
            if (stackText != null) stackText.color = foldedColor;
            if (betText != null) betText.gameObject.SetActive(false);
        }
    }

    public void SetActive(bool isActive)
    {
        if (!hasFolded && nameText != null)
            nameText.color = isActive ? activeTurnColor : inactiveTurnColor;
    }

    public void SetDealer(bool isDealer)
    {
        if (dealerButton != null)
            dealerButton.SetActive(isDealer);
    }

    public void ResetForNewHand()
    {
        currentBet = 0;
        hasFolded = false;
        SetActive(false);
        UpdateStackDisplay();
        UpdateBetDisplay();
        UpdateFoldedState();
    }

    public void WinPot(int amount)
    {
        currentStack += amount;
        UpdateStackDisplay();
    }
}
