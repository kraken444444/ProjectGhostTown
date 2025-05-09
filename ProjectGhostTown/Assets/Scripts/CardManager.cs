using UnityEngine;

// CardManager.cs - New class for managing card collections
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Singleton instance
    private static CardManager _instance;
    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CardManager");
                _instance = obj.AddComponent<CardManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    // Card template collections
    private Dictionary<string, CardTemplate> cardTemplates = new Dictionary<string, CardTemplate>();
    
    // Events
    public System.Action<Card> OnCardCreated;
    public System.Action<Card> OnCardDestroyed;
    
    private void Awake()
    {
        // Handle singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load card templates
        LoadCardTemplates();
    }
    
    private void LoadCardTemplates()
    {
        // Load from resources or create hardcoded templates
        CreateBasicCardTemplates();
    }
    
    private void CreateBasicCardTemplates()
    {
        // Create basic card templates for each class
        
        // Radiomancer cards
        RegisterCardTemplate(new CardTemplate("Radiation Bolt", "Fire a bolt of radiation that deals damage to enemies.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Atomic Shield", "Surround yourself with a protective atomic barrier.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Isotope Drain", "Drain energy from an enemy to restore your own power.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Radiation Field", "Create a hazardous field that damages enemies over time.", CardRarity.Uncommon));
        RegisterCardTemplate(new CardTemplate("Atomic Accelerator", "Increase your spell damage for a short time.", CardRarity.Uncommon));
        
        // Brawler cards
        RegisterCardTemplate(new CardTemplate("Heavy Punch", "A powerful melee attack that deals high damage.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Block", "Take a defensive stance to reduce incoming damage.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Adrenaline Rush", "Temporarily increase your attack speed and damage.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Whirlwind Strike", "Attack all enemies around you with a spinning attack.", CardRarity.Uncommon));
        RegisterCardTemplate(new CardTemplate("Iron Skin", "Harden your skin to substantially reduce incoming damage.", CardRarity.Uncommon));
        
        // Gunslinger cards
        RegisterCardTemplate(new CardTemplate("Quick Shot", "Fire a rapid bullet with high accuracy.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Dodge Roll", "Quickly evade incoming attacks and reposition.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Trick Shot", "Fire a bullet that can ricochet to hit multiple targets.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Headshot", "Aim for a critical spot to deal massive damage.", CardRarity.Uncommon));
        RegisterCardTemplate(new CardTemplate("Rapid Fire", "Fire multiple shots in quick succession.", CardRarity.Uncommon));
        
        // Gambler cards
        RegisterCardTemplate(new CardTemplate("Lucky Strike", "Attack with increased critical chance.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Fortune's Favor", "Temporarily increase your luck.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("Wild Card", "Play a random card with enhanced effects.", CardRarity.Common));
        RegisterCardTemplate(new CardTemplate("All In", "Stake your health to deal massive damage.", CardRarity.Uncommon));
        RegisterCardTemplate(new CardTemplate("Loaded Dice", "Guarantee a critical hit on your next attack.", CardRarity.Uncommon));
    }
    
    public void RegisterCardTemplate(CardTemplate template)
    {
        if (template == null)
        {
            Debug.LogError("Cannot register null card template");
            return;
        }
        
        if (string.IsNullOrEmpty(template.ID))
        {
            Debug.LogError("Cannot register card template with empty ID");
            return;
        }
        
        cardTemplates[template.ID] = template;
    }
    
    public CardTemplate GetCardTemplate(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Cannot get card template with empty ID");
            return null;
        }
        
        if (cardTemplates.TryGetValue(id, out CardTemplate template))
        {
            return template;
        }
        
        Debug.LogWarning($"Card template with ID {id} not found");
        return null;
    }
    
    public List<CardTemplate> GetCardTemplatesByRarity(CardRarity rarity)
    {
        return cardTemplates.Values.Where(t => t.Rarity == rarity).ToList();
    }
    
    public List<CardTemplate> GetAllCardTemplates()
    {
        return cardTemplates.Values.ToList();
    }
    
    public Card CreateCard(string templateId)
    {
        CardTemplate template = GetCardTemplate(templateId);
        if (template == null)
        {
            Debug.LogError($"Cannot create card with template ID {templateId} - template not found");
            return null;
        }
        
        Card card = new Card(template);
        OnCardCreated?.Invoke(card);
        return card;
    }
    
    public Card CreateRandomCard(CardRarity rarity)
    {
        List<CardTemplate> templates = GetCardTemplatesByRarity(rarity);
        if (templates.Count == 0)
        {
            Debug.LogWarning($"No card templates found for rarity {rarity}");
            return null;
        }
        
        int randomIndex = Random.Range(0, templates.Count);
        return new Card(templates[randomIndex]);
    }
    
    public void DestroyCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Cannot destroy null card");
            return;
        }
        
        OnCardDestroyed?.Invoke(card);
    }
}