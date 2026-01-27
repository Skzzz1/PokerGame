using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public UnityEngine.UI.Image cardImage;
    public bool isFaceUp = false;
    
    private Card card;
    private CardSpriteManager spriteManager;

    
    void Start()
    {
        spriteManager = FindFirstObjectByType<CardSpriteManager>();
    }
    
    public void SetCard(Card c, bool faceUp = true)
    {
        card = c;
        isFaceUp = faceUp;
        UpdateVisual();
    }
    
    public void Flip()
    {
        isFaceUp = !isFaceUp;
        UpdateVisual();
    }
    
    void UpdateVisual()
    {
        if (spriteManager == null)
        {
            spriteManager = FindFirstObjectByType<CardSpriteManager>();
        }
        
        if (isFaceUp && card != null)
        {
            cardImage.sprite = spriteManager.GetCardSprite(card);
        }
        else
        {
            cardImage.sprite = spriteManager.GetCardBack();
        }
    }
    
    public Card GetCard()
    {
        return card;
    }
}
