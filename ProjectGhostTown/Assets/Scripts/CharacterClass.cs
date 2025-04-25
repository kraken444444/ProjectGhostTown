using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterClass
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ClassType Type { get; private set; }
    
    public Dictionary<AttributeType, int> BaseAttributeModifiers { get; private set; }
    
    public Dictionary<AttributeType, int> FavoredAttributes { get; private set; }
    
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
    
    public static CharacterClass CreateWarrior()
    {
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
            new List<CardTemplate> {
                //add cards here
            },
            new Dictionary<StatType, float> {
                { StatType.PhysicalDamage, 10f },
                { StatType.DamageReduction, 5f }
            }
        );
    }
    
    public static CharacterClass CreateRadiomancer()
    {
        return new CharacterClass(
            "Magical Radioactive Wizard",
            "Wields radioactive spells and magical attacks.",
            ClassType.Radiomancer,
            ResourceType.Mana,
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
            new List<CardTemplate> {
                //cardtemplates go here
              
            },
            new Dictionary<StatType, float> {
              
            }
        );
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

    
    
   
  
  
    
    


