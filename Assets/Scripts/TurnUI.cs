using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUI : MonoBehaviour
{
    //Game Buttons
    public Button CallBtn;
    public Button BetBtn;
    public Button FoldBtn;
    public Button CheckBtn;
    public Button RaiseBtn;

    public Slider BetSlider;

    public TextMeshProUGUI AmountTxt;
    public TextMeshProUGUI CallAmountTxt;
    public TextMeshProUGUI PotTxt;

    TurnManager turnManager;

    void Awake()
    {
        turnManager = FindFirstObjectByType<TurnManager>();

        FoldBtn.onClick.AddListener(OnFoldClicked);
        CheckBtn.onClick.AddListener(OnCheckClicked);
        CallBtn.onClick.AddListener(OnCallClicked);
        BetBtn.onClick.AddListener(OnBetClicked);
        RaiseBtn.onClick.AddListener(OnRaiseClicked);
        
        BetSlider.onValueChanged.AddListener(UpdateAmountTxt);
        
        HideAll();
    }
    // hide all components unless needed in a turn
    public void HideAll()
    {
        FoldBtn.gameObject.SetActive(false);
        CallBtn.gameObject.SetActive(false);
        CheckBtn.gameObject.SetActive(false);
        BetBtn.gameObject.SetActive(false);
        RaiseBtn.gameObject.SetActive(false);
        BetSlider.gameObject.SetActive(false);
        AmountTxt.gameObject.SetActive(false);
        
        if (CallAmountTxt != null)
            CallAmountTxt.gameObject.SetActive(false);
        
        // if(PotTxt!=null)
        // {
        //     PotTxt.gameObject.SetActive(true);
        //     SetPot(0);//initally 0 in the pot
        // }
        
    }
    public void SetPot(int pot)
    {
        if (PotTxt == null)
        {
            Debug.LogError("PotTxt is NULL on TurnUI");
            return;
        }

        if (PotTxt != null)
            PotTxt.text = $"Pot: ${pot}";
    }

    //Updating text when sliding
    void UpdateAmountTxt(float value)
    {
        AmountTxt.text = "$" +((int)value).ToString();
        Debug.Log($"Slider dragged to: {value}");  // if slider doesnt work this should be in console
    }

    void OnFoldClicked()
    {
        HideAll();
        turnManager.Fold();
    }

    void OnCheckClicked()
    {
        HideAll();
        turnManager.Check();
    }

    void OnCallClicked()
    {
        HideAll();
        turnManager.Call();
    }
    //Updating  bet amount when click bet after slider
    void OnBetClicked()
    {
        HideAll();
        turnManager.Bet((int)BetSlider.value);
    }

    //Updating  raise amount when click bet after slider
    void OnRaiseClicked()
    {
        HideAll();
        turnManager.Raise((int)BetSlider.value);
    }

    // UI changes when action buttons are required for player
    //Call amount is the difference between current bet and player's bet
    //Max amount is what player has left in their stack
    public void ShowActions(bool betExists, int callAmount,int minRaise, int maxAmount)
    {
        HideAll();

        // Always show fold
        FoldBtn.gameObject.SetActive(true);

        if (betExists)
        {
            // There's a bet to face
            if (callAmount > 0 && callAmount <= maxAmount)
            {
                // Show call button
                CallBtn.gameObject.SetActive(true);
                if (CallAmountTxt != null)// safe
                {
                    CallAmountTxt.gameObject.SetActive(true);
                    CallAmountTxt.text = "Call $" + callAmount;
                }
            }
            else if (callAmount == 0)
            {
                // Already matched the bet, can check
                CheckBtn.gameObject.SetActive(true);
            }

            // Show raise option if player has enough chips
            if (maxAmount >= minRaise)
            {
                RaiseBtn.gameObject.SetActive(true);
                BetSlider.gameObject.SetActive(true);
                AmountTxt.gameObject.SetActive(true);
                
                BetSlider.minValue = minRaise;
                BetSlider.maxValue = maxAmount;
                BetSlider.value = minRaise;
                
                UpdateAmountTxt(minRaise);
            }
        }
        else
        {
            // No bet exists
            CheckBtn.gameObject.SetActive(true);
            
            // Show bet option if player can bet at least big blind
            if (maxAmount >= turnManager.bigBlind)
            {
                BetBtn.gameObject.SetActive(true);
                BetSlider.gameObject.SetActive(true);
                AmountTxt.gameObject.SetActive(true);
                
                BetSlider.minValue = turnManager.bigBlind;
                BetSlider.maxValue = maxAmount;
                BetSlider.value = turnManager.bigBlind;
                
                UpdateAmountTxt(turnManager.bigBlind);//initial display
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
