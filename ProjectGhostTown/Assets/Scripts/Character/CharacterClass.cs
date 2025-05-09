// CharacterClass.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterClass
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public GameEnums.ClassType Type { get; private set; }
    public GameEnums.ResourceType ResourceType { get; private set; }
    
    public Dictionary<GameEnums.AttributeType, int> BaseAttributeModifiers { get; private set; }
    public Dictionary<GameEnums.AttributeType, int> FavoredAttributes { get; private set; }
    public List<CardTemplate> StartingCards { get; private set; }
    public Dictionary<GameEnums.StatType, float> ClassBonuses { get; private set; }
    public List<CharacterClass> AvailableSubclasses { get; private set; }
    
    public CharacterClass(
        string name, 
        string description, 
        GameEnums.ClassType type,
        GameEnums.ResourceType resourceType,
        Dictionary<GameEnums.AttributeType, int> baseAttributeModifiers,
        Dictionary<GameEnums.AttributeType, int> favoredAttributes,
        List<CardTemplate> startingCards,
        Dictionary<GameEnums.StatType, float> classBonuses)
    {
        Name = name;
        Description = description;
        Type = type;
        ResourceType = resourceType;
        BaseAttributeModifiers = baseAttributeModifiers ?? new Dictionary<GameEnums.AttributeType, int>();
        FavoredAttributes = favoredAttributes ?? new Dictionary<GameEnums.AttributeType, int>();
        StartingCards = startingCards ?? new List<CardTemplate>();
        ClassBonuses = classBonuses ?? new Dictionary<GameEnums.StatType, float>();
        AvailableSubclasses = new List<CharacterClass>();
    }
    
    public static CharacterClass FromClassData(CharacterClassData data)
    {
        if (data == null)
        {
            Debug.LogError("Cannot create CharacterClass: CharacterClassData is null");
            return null;
        }
        
        Dictionary<GameEnums.AttributeType, int> baseAttrs = new Dictionary<GameEnums.AttributeType, int>();
        Dictionary<GameEnums.AttributeType, int> favoredAttrs = new Dictionary<GameEnums.AttributeType, int>();
        List<CardTemplate> startingCards = new List<CardTemplate>();
        Dictionary<GameEnums.StatType, float> bonuses = new Dictionary<GameEnums.StatType, float>();
        
        if (data.BaseAttributeModifiers != null)
        {
            foreach (var pair in data.BaseAttributeModifiers)
            {
                baseAttrs[pair.Key] = pair.Value;
            }
        }
        
        if (data.FavoredAttributes != null)
        {
            foreach (var pair in data.FavoredAttributes)
            {
                favoredAttrs[pair.Key] = pair.Value;
            }
        }
        
        if (data.StartingCards != null)
        {
            startingCards.AddRange(data.StartingCards);
        }
        
        if (data.ClassBonuses != null)
        {
            foreach (var pair in data.ClassBonuses)
            {
                bonuses[pair.Key] = pair.Value;
            }
        }
        
        CharacterClass newClass = new CharacterClass(
            string.IsNullOrEmpty(data.Name) ? "Unknown" : data.Name,
            string.IsNullOrEmpty(data.Description) ? "No description" : data.Description,
            data.Type,
            data.ResourceType,
            baseAttrs,
            favoredAttrs,
            startingCards,
            bonuses
        );
        
        if (data.AvailableSubclasses != null)
        {
            foreach (var subclassData in data.AvailableSubclasses)
            {
                if (subclassData != null)
                {
                    CharacterClass subclass = FromClassData(subclassData);
                    if (subclass != null)
                    {
                        newClass.AvailableSubclasses.Add(subclass);
                    }
                }
            }
        }
        
        return newClass;
    }
    
    public static CharacterClass CreateDefaultClass(GameEnums.ClassType type)
    {
        string name = type.ToString();
        string description = "A " + type.ToString() + " character";
        GameEnums.ResourceType resourceType = type.GetDefaultResourceType();
        Dictionary<GameEnums.AttributeType, int> baseAttrs = new Dictionary<GameEnums.AttributeType, int>();
        Dictionary<GameEnums.AttributeType, int> favoredAttrs = new Dictionary<GameEnums.AttributeType, int>();
        List<CardTemplate> startingCards = new List<CardTemplate>();
        Dictionary<GameEnums.StatType, float> bonuses = new Dictionary<GameEnums.StatType, float>();
        
        // Set default attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            baseAttrs[attrType] = 5;
        }
        
        // Set class-specific attributes
        switch (type)
        {
            case GameEnums.ClassType.Radiomancer:
                baseAttrs[GameEnums.AttributeType.Offense] = 8;
                baseAttrs[GameEnums.AttributeType.Tenacity] = 7;
                bonuses[GameEnums.StatType.MagicalDamage] = 15f;
                startingCards.Add(new CardTemplate("Radiation Bolt", "Fire a bolt of radiation", CardRarity.Common));
                startingCards.Add(new CardTemplate("Atomic Shield", "Protect yourself with atomic energy", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Brawler:
                baseAttrs[GameEnums.AttributeType.Resilience] = 8;
                baseAttrs[GameEnums.AttributeType.Offense] = 7;
                bonuses[GameEnums.StatType.PhysicalDamage] = 15f;
                bonuses[GameEnums.StatType.DamageReduction] = 5f;
                startingCards.Add(new CardTemplate("Heavy Punch", "A powerful melee attack", CardRarity.Common));
                startingCards.Add(new CardTemplate("Block", "Guard against incoming attacks", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Gunslinger:
                baseAttrs[GameEnums.AttributeType.Expertise] = 8;
                baseAttrs[GameEnums.AttributeType.Fortuity] = 7;
                bonuses[GameEnums.StatType.CriticalChance] = 5f;
                bonuses[GameEnums.StatType.AttackSpeed] = 10f;
                startingCards.Add(new CardTemplate("Quick Shot", "Fire a rapid bullet", CardRarity.Common));
                startingCards.Add(new CardTemplate("Dodge Roll", "Quickly evade incoming attacks", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Gambler:
                baseAttrs[GameEnums.AttributeType.Fortuity] = 9;
                baseAttrs[GameEnums.AttributeType.Negotiation] = 8;
                bonuses[GameEnums.StatType.MagicFind] = 20f;
                bonuses[GameEnums.StatType.CriticalDamage] = 10f;
                startingCards.Add(new CardTemplate("Lucky Strike", "Attack with increased critical chance", CardRarity.Common));
                startingCards.Add(new CardTemplate("Fortune's Favor", "Increase luck temporarily", CardRarity.Common));
                break;
        }
        
        // Set favored attributes
        List<GameEnums.AttributeType> favored = type.GetFavoredAttributes();
        if (favored != null)
        {
            foreach (var attr in favored)
            {
                favoredAttrs[attr] = 1;
            }
        }
        
        return new CharacterClass(name, description, type, resourceType, baseAttrs, favoredAttrs, startingCards, bonuses);
    }
    
    public void AddSubclass(CharacterClass subclass)
    {
        AvailableSubclasses.Add(subclass);
    }
}