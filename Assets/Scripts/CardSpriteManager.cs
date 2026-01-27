using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;

// this is the visual layer of card sprites

public class CardSpriteManager : MonoBehaviour
{
    [Header("Card Sprites")]
    [Tooltip("Drag all card sprites here. Name them like: 2H(2_hearts) etc.")]
    public Sprite[] cardSprites;
    
    [Tooltip("Card back sprite (for face-down cards)")]
    public Sprite cardBackSprite;
    
    private Dictionary<string, Sprite> spriteDict;
    
    void Awake()
    {
        BuildSpriteDictionary();
    }
    
    void BuildSpriteDictionary()
    {
        spriteDict = new Dictionary<string, Sprite>();
        
        foreach (Sprite sprite in cardSprites)
        {
            spriteDict[sprite.name] = sprite;
            Debug.Log($"Loaded sprite: {sprite.name}");  // Debug each sprite
        }
        
        Debug.Log($"Loaded {spriteDict.Count} card sprites total");
        
        // Verify all expected cards are present
        VerifyAllCardsPresent();
    }
    
    void VerifyAllCardsPresent()
    {
        int missingCount = 0;
        
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                Card testCard = new Card(suit, rank);
                string expectedName = testCard.GetSpriteName();
                
                if (!spriteDict.ContainsKey(expectedName))
                {
                    Debug.LogError($"Missing sprite: {expectedName}");
                    missingCount++;
                }
            }
        }
        
        if (missingCount == 0)
        {
            Debug.Log("✓ All 52 card sprites found!");
        }
        else
        {
            Debug.LogError($"✗ Missing {missingCount} card sprites!");
        }
    }
    
    public Sprite GetCardSprite(Card card)
    {
        if (card == null) return cardBackSprite;
        
        string spriteName = card.GetSpriteName();
        
        if (spriteDict.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }
        
        Debug.LogWarning($"Sprite not found for: {spriteName}");
        return null;
    }
    
    public Sprite GetCardBack()
    {
        return cardBackSprite;
    }
}