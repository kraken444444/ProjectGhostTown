// CharacterClassData.cs
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[System.Serializable]
public class CharacterClassData
{
    public string Name;
    public string Description;
    public GameEnums.ClassType Type;
    public GameEnums.ResourceType ResourceType;
    
    public Dictionary<GameEnums.AttributeType, int> BaseAttributeModifiers = new Dictionary<GameEnums.AttributeType, int>();
    public Dictionary<GameEnums.AttributeType, int> FavoredAttributes = new Dictionary<GameEnums.AttributeType, int>();
    public List<CardTemplate> StartingCards = new List<CardTemplate>();
    public Dictionary<GameEnums.StatType, float> ClassBonuses = new Dictionary<GameEnums.StatType, float>();
    
    public List<CharacterClassData> AvailableSubclasses = new List<CharacterClassData>();
    
    public CharacterClassData()
    {
        BaseAttributeModifiers = new Dictionary<GameEnums.AttributeType, int>();
        FavoredAttributes = new Dictionary<GameEnums.AttributeType, int>();
        StartingCards = new List<CardTemplate>();
        ClassBonuses = new Dictionary<GameEnums.StatType, float>();
        AvailableSubclasses = new List<CharacterClassData>();
    }
    
    public static CharacterClassData CreateFromType(GameEnums.ClassType type)
    {
        CharacterClassData data = new CharacterClassData();
        
        data.Name = type.ToString();
        data.Description = "A " + type.ToString() + " character";
        data.Type = type;
        data.ResourceType = type.GetDefaultResourceType();
        
        // Set default attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            data.BaseAttributeModifiers[attrType] = 5;
        }
        
        // Set class-specific attributes
        switch (type)
        {
            case GameEnums.ClassType.Radiomancer:
                data.BaseAttributeModifiers[GameEnums.AttributeType.Offense] = 8;
                data.BaseAttributeModifiers[GameEnums.AttributeType.Tenacity] = 7;
                data.ClassBonuses[GameEnums.StatType.MagicalDamage] = 15f;
                data.StartingCards.Add(new CardTemplate("Radiation Bolt", "Fire a bolt of radiation", CardRarity.Common));
                data.StartingCards.Add(new CardTemplate("Atomic Shield", "Protect yourself with atomic energy", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Brawler:
                data.BaseAttributeModifiers[GameEnums.AttributeType.Resilience] = 8;
                data.BaseAttributeModifiers[GameEnums.AttributeType.Offense] = 7;
                data.ClassBonuses[GameEnums.StatType.PhysicalDamage] = 15f;
                data.ClassBonuses[GameEnums.StatType.DamageReduction] = 5f;
                data.StartingCards.Add(new CardTemplate("Heavy Punch", "A powerful melee attack", CardRarity.Common));
                data.StartingCards.Add(new CardTemplate("Block", "Guard against incoming attacks", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Gunslinger:
                data.BaseAttributeModifiers[GameEnums.AttributeType.Expertise] = 8;
                data.BaseAttributeModifiers[GameEnums.AttributeType.Fortuity] = 7;
                data.ClassBonuses[GameEnums.StatType.CriticalChance] = 5f;
                data.ClassBonuses[GameEnums.StatType.AttackSpeed] = 10f;
                data.StartingCards.Add(new CardTemplate("Quick Shot", "Fire a rapid bullet", CardRarity.Common));
                data.StartingCards.Add(new CardTemplate("Dodge Roll", "Quickly evade incoming attacks", CardRarity.Common));
                break;
                
            case GameEnums.ClassType.Gambler:
                data.BaseAttributeModifiers[GameEnums.AttributeType.Fortuity] = 9;
                data.BaseAttributeModifiers[GameEnums.AttributeType.Negotiation] = 8;
                data.ClassBonuses[GameEnums.StatType.MagicFind] = 20f;
                data.ClassBonuses[GameEnums.StatType.CriticalDamage] = 10f;
                data.StartingCards.Add(new CardTemplate("Lucky Strike", "Attack with increased critical chance", CardRarity.Common));
                data.StartingCards.Add(new CardTemplate("Fortune's Favor", "Increase luck temporarily", CardRarity.Common));
                break;
        }
        
        // Set favored attributes
        List<GameEnums.AttributeType> favored = type.GetFavoredAttributes();
        if (favored != null)
        {
            foreach (var attr in favored)
            {
                data.FavoredAttributes[attr] = 1;
            }
        }
        
        return data;
    }
}