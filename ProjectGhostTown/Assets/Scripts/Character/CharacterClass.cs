using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterClass
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public GameEnums.ClassType Type { get;  set; }
    
    public Dictionary<GameEnums.AttributeType, int> BaseAttributeModifiers { get; private set; } // flat character stats
    
    public Dictionary<GameEnums.AttributeType, int> FavoredAttributes { get; private set; } // additional bonuses
    
    private List<Subclass> _availableSubclasses;
    public GameEnums.ResourceType ResourceType { get; set; }
    
    public List<CardTemplate> StartingCards { get; private set; }
    public Dictionary<GameEnums.StatType, float> ClassBonuses { get; private set; }
    
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
        BaseAttributeModifiers = baseAttributeModifiers;
        FavoredAttributes = favoredAttributes;
        StartingCards = startingCards;
        ClassBonuses = classBonuses;
        _availableSubclasses = new List<Subclass>();
    }
    
    public void AddSubclass(Subclass subclass)
    {
        if (subclass.ParentClass.Type == Type)
        {
            _availableSubclasses.Add(subclass);
        }
    }
    
    public List<Subclass> GetAvailableSubclasses()
    {
        return new List<Subclass>(_availableSubclasses);
    }
    
    public static CharacterClass CreateClass(GameEnums.ClassType classType)
    {
        switch (classType)
        {
            case GameEnums.ClassType.Brawler:
                return new CharacterClass(
                    "Brawler",
                    "A scrappy brawler who fights dirty with his fists.",
                    GameEnums.ClassType.Brawler,
                    GameEnums.ResourceType.Rage,
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 3 },
                        { GameEnums.AttributeType.Resilience, 5 },
                        { GameEnums.AttributeType.Tenacity, 2 },
                        { GameEnums.AttributeType.Utility, 1 },
                        { GameEnums.AttributeType.Fortuity, 0 },
                        { GameEnums.AttributeType.Negotiation, 0 },
                        { GameEnums.AttributeType.Expertise, 1 }
                    },
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 5 },
                        { GameEnums.AttributeType.Resilience, 8 },
                        { GameEnums.AttributeType.Tenacity, 3 }
                    },
                    new List<CardTemplate>(), 
                    // Add starting cards as needed
                    new Dictionary<GameEnums.StatType, float> {
                        { GameEnums.StatType.PhysicalDamage, 10f },
                        { GameEnums.StatType.DamageReduction, 5f }
                    }
                );
                
            case GameEnums.ClassType.Radiomancer:
                return new CharacterClass(
                    "Radiomancer",
                    "Wields radioactive spells and magical attacks.",
                    GameEnums.ClassType.Radiomancer,
                    GameEnums.ResourceType.Radiopower,
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 5 },
                        { GameEnums.AttributeType.Resilience, 0 },
                        { GameEnums.AttributeType.Tenacity, 3 },
                        { GameEnums.AttributeType.Utility, 2 },
                        { GameEnums.AttributeType.Fortuity, 1 },
                        { GameEnums.AttributeType.Negotiation, 1 },
                        { GameEnums.AttributeType.Expertise, 0 }
                    },
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 8 },
                        { GameEnums.AttributeType.Tenacity, 5 },
                        { GameEnums.AttributeType.Expertise, 3 }
                    },
                    new List<CardTemplate>(),
                    new Dictionary<GameEnums.StatType, float> {
                        { GameEnums.StatType.MagicalDamage, 12f },
                        { GameEnums.StatType.CriticalChance, 3f }
                    }
                );
                
            case GameEnums.ClassType.Gunslinger:
                return new CharacterClass(
                    "Gunslinger",
                    "A skilled marksman who excels at ranged combat.",
                    GameEnums.ClassType.Gunslinger,
                    GameEnums.ResourceType.Energy,
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 4 },
                        { GameEnums.AttributeType.Resilience, 1 },
                        { GameEnums.AttributeType.Tenacity, 2 },
                        { GameEnums.AttributeType.Utility, 3 },
                        { GameEnums.AttributeType.Fortuity, 2 },
                        { GameEnums.AttributeType.Negotiation, 0 },
                        { GameEnums.AttributeType.Expertise, 4 }
                    },
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 6 },
                        { GameEnums.AttributeType.Expertise, 7 },
                        { GameEnums.AttributeType.Fortuity, 4 }
                    },
                    new List<CardTemplate>(),  
                    new Dictionary<GameEnums.StatType, float> {
                        { GameEnums.StatType.PhysicalDamage, 8f },
                        { GameEnums.StatType.CriticalDamage, 15f },
                        { GameEnums.StatType.AttackSpeed, 5f }
                    }
                );
                
            case GameEnums.ClassType.Gambler:
                return new CharacterClass(
                    "Gambler",
                    "Relies on luck and chance to turn the tide of battle.",
                    GameEnums.ClassType.Gambler,
                   GameEnums.ResourceType.Focus,
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Offense, 2 },
                        { GameEnums.AttributeType.Resilience, 2 },
                        { GameEnums.AttributeType.Tenacity, 2 },
                        { GameEnums.AttributeType.Utility, 2 },
                        { GameEnums.AttributeType.Fortuity, 5 },
                        { GameEnums.AttributeType.Negotiation, 3 },
                        { GameEnums.AttributeType.Expertise, 0 }
                    },
                    new Dictionary<GameEnums.AttributeType, int> {
                        { GameEnums.AttributeType.Fortuity, 8 },
                        { GameEnums.AttributeType.Negotiation, 5 },
                        { GameEnums.AttributeType.Offense, 3 }
                    },
                    new List<CardTemplate>(),  
                    new Dictionary<GameEnums.StatType, float> {
                        { GameEnums.StatType.CriticalChance, 8f },
                        { GameEnums.StatType.MagicFind, 20f },
                        { GameEnums.StatType.CriticalDamage, 10f }
                    }
                );
                
            default:
                Debug.LogWarning($"Unknown class type: {classType}. Creating Brawler as default.");
                return CreateClass(GameEnums.ClassType.Brawler);
        }
    }
}

    
    


