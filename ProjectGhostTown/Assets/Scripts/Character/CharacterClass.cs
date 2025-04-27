using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterClass
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ClassType Type { get;  set; }
    
    public Dictionary<AttributeType, int> BaseAttributeModifiers { get; private set; } // flat character stats
    
    public Dictionary<AttributeType, int> FavoredAttributes { get; private set; } // additional bonuses
    
    private List<Subclass> _availableSubclasses;
    public ResourceType ResourceType { get; private set; }
    
    public List<CardTemplate> StartingCards { get; private set; }
    public Dictionary<StatType, float> ClassBonuses { get; private set; }
    
    public CharacterClass(
        string name, 
        string description, 
        ClassType type, 
        ResourceType resourceType,
        Dictionary<AttributeType, int> baseAttributeModifiers,
        Dictionary<AttributeType, int> favoredAttributes,
        List<CardTemplate> startingCards,
        Dictionary<StatType, float> classBonuses)
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
    
    public static CharacterClass CreateClass(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Brawler:
                return new CharacterClass(
                    "Brawler",
                    "A scrappy brawler who fights dirty with his fists.",
                    ClassType.Brawler,
                    ResourceType.Rage,
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 3 },
                        { AttributeType.Resilience, 5 },
                        { AttributeType.Tenacity, 2 },
                        { AttributeType.Utility, 1 },
                        { AttributeType.Fortuity, 0 },
                        { AttributeType.Negotiation, 0 },
                        { AttributeType.Expertise, 1 }
                    },
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 5 },
                        { AttributeType.Resilience, 8 },
                        { AttributeType.Tenacity, 3 }
                    },
                    new List<CardTemplate>(), 
                    // Add starting cards as needed
                    new Dictionary<StatType, float> {
                        { StatType.PhysicalDamage, 10f },
                        { StatType.DamageReduction, 5f }
                    }
                );
                
            case ClassType.Radiomancer:
                return new CharacterClass(
                    "Radiomancer",
                    "Wields radioactive spells and magical attacks.",
                    ClassType.Radiomancer,
                    ResourceType.Radiopower,
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 5 },
                        { AttributeType.Resilience, 0 },
                        { AttributeType.Tenacity, 3 },
                        { AttributeType.Utility, 2 },
                        { AttributeType.Fortuity, 1 },
                        { AttributeType.Negotiation, 1 },
                        { AttributeType.Expertise, 0 }
                    },
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 8 },
                        { AttributeType.Tenacity, 5 },
                        { AttributeType.Expertise, 3 }
                    },
                    new List<CardTemplate>(),
                    new Dictionary<StatType, float> {
                        { StatType.MagicalDamage, 12f },
                        { StatType.CriticalChance, 3f }
                    }
                );
                
            case ClassType.Gunslinger:
                return new CharacterClass(
                    "Gunslinger",
                    "A skilled marksman who excels at ranged combat.",
                    ClassType.Gunslinger,
                    ResourceType.Energy,
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 4 },
                        { AttributeType.Resilience, 1 },
                        { AttributeType.Tenacity, 2 },
                        { AttributeType.Utility, 3 },
                        { AttributeType.Fortuity, 2 },
                        { AttributeType.Negotiation, 0 },
                        { AttributeType.Expertise, 4 }
                    },
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 6 },
                        { AttributeType.Expertise, 7 },
                        { AttributeType.Fortuity, 4 }
                    },
                    new List<CardTemplate>(),  
                    new Dictionary<StatType, float> {
                        { StatType.PhysicalDamage, 8f },
                        { StatType.CriticalDamage, 15f },
                        { StatType.AttackSpeed, 5f }
                    }
                );
                
            case ClassType.Gambler:
                return new CharacterClass(
                    "Gambler",
                    "Relies on luck and chance to turn the tide of battle.",
                    ClassType.Gambler,
                    ResourceType.Focus,
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Offense, 2 },
                        { AttributeType.Resilience, 2 },
                        { AttributeType.Tenacity, 2 },
                        { AttributeType.Utility, 2 },
                        { AttributeType.Fortuity, 5 },
                        { AttributeType.Negotiation, 3 },
                        { AttributeType.Expertise, 0 }
                    },
                    new Dictionary<AttributeType, int> {
                        { AttributeType.Fortuity, 8 },
                        { AttributeType.Negotiation, 5 },
                        { AttributeType.Offense, 3 }
                    },
                    new List<CardTemplate>(),  
                    new Dictionary<StatType, float> {
                        { StatType.CriticalChance, 8f },
                        { StatType.MagicFind, 20f },
                        { StatType.CriticalDamage, 10f }
                    }
                );
                
            default:
                Debug.LogWarning($"Unknown class type: {classType}. Creating Brawler as default.");
                return CreateClass(ClassType.Brawler);
        }
    }
}

public enum ClassType
{
    Radiomancer,
    Brawler,
    Gunslinger,
    Gambler
}

public enum ResourceType
{
    Health,  
    Mana,    
    Rage,    
    Energy,  
    Focus,
    Radiopower
}

    
    
   
  
  
    
    


