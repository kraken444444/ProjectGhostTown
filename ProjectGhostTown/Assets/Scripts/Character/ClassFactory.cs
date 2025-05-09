// ClassFactory.cs - New helper class
using System.Collections.Generic;
using UnityEngine;

public static class ClassFactory
{
    public static CharacterClass CreateClass(GameEnums.ClassType classType)
    {
        CharacterClassData data = CharacterClassData.CreateFromType(classType);
        return CharacterClass.FromClassData(data);
    }
    
    public static CharacterClassData CreateRadiomancerData()
    {
        CharacterClassData data = new CharacterClassData();
        
        data.Name = "Radiomancer";
        data.Description = "A master of dangerous atomic energies, able to harness radiation for destructive or healing purposes.";
        data.Type = GameEnums.ClassType.Radiomancer;
        data.ResourceType = GameEnums.ResourceType.Radiopower;
        
        // Set base attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            data.BaseAttributeModifiers[attrType] = 5;
        }
        
        data.BaseAttributeModifiers[GameEnums.AttributeType.Offense] = 8;
        data.BaseAttributeModifiers[GameEnums.AttributeType.Tenacity] = 7;
        
        // Set favored attributes
        data.FavoredAttributes[GameEnums.AttributeType.Offense] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Tenacity] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Expertise] = 1;
        
        // Set class bonuses
        data.ClassBonuses[GameEnums.StatType.MagicalDamage] = 15f;
        
        // Add starting cards
        data.StartingCards.Add(new CardTemplate("Radiation Bolt", "Fire a bolt of radiation that deals damage to enemies.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Atomic Shield", "Surround yourself with a protective atomic barrier.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Isotope Drain", "Drain energy from an enemy to restore your own power.", CardRarity.Common));
        
        return data;
    }
    
    public static CharacterClassData CreateBrawlerData()
    {
        CharacterClassData data = new CharacterClassData();
        
        data.Name = "Brawler";
        data.Description = "A skilled close-combat fighter who uses fists and physical prowess to overcome challenges.";
        data.Type = GameEnums.ClassType.Brawler;
        data.ResourceType = GameEnums.ResourceType.Rage;
        
        // Set base attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            data.BaseAttributeModifiers[attrType] = 5;
        }
        
        data.BaseAttributeModifiers[GameEnums.AttributeType.Resilience] = 8;
        data.BaseAttributeModifiers[GameEnums.AttributeType.Offense] = 7;
        
        // Set favored attributes
        data.FavoredAttributes[GameEnums.AttributeType.Offense] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Resilience] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Tenacity] = 1;
        
        // Set class bonuses
        data.ClassBonuses[GameEnums.StatType.PhysicalDamage] = 15f;
        data.ClassBonuses[GameEnums.StatType.DamageReduction] = 5f;
        
        // Add starting cards
        data.StartingCards.Add(new CardTemplate("Heavy Punch", "A powerful melee attack that deals high damage.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Block", "Take a defensive stance to reduce incoming damage.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Adrenaline Rush", "Temporarily increase your attack speed and damage.", CardRarity.Common));
        
        return data;
    }
    
    public static CharacterClassData CreateGunslingerData()
    {
        CharacterClassData data = new CharacterClassData();
        
        data.Name = "Gunslinger";
        data.Description = "A marksman with lightning reflexes who excels with ranged weapons and precision strikes.";
        data.Type = GameEnums.ClassType.Gunslinger;
        data.ResourceType = GameEnums.ResourceType.Energy;
        
        // Set base attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            data.BaseAttributeModifiers[attrType] = 5;
        }
        
        data.BaseAttributeModifiers[GameEnums.AttributeType.Expertise] = 8;
        data.BaseAttributeModifiers[GameEnums.AttributeType.Fortuity] = 7;
        
        // Set favored attributes
        data.FavoredAttributes[GameEnums.AttributeType.Offense] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Expertise] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Fortuity] = 1;
        
        // Set class bonuses
        data.ClassBonuses[GameEnums.StatType.CriticalChance] = 5f;
        data.ClassBonuses[GameEnums.StatType.AttackSpeed] = 10f;
        
        // Add starting cards
        data.StartingCards.Add(new CardTemplate("Quick Shot", "Fire a rapid bullet with high accuracy.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Dodge Roll", "Quickly evade incoming attacks and reposition.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Trick Shot", "Fire a bullet that can ricochet to hit multiple targets.", CardRarity.Common));
        
        return data;
    }
    
    public static CharacterClassData CreateGamblerData()
    {
        CharacterClassData data = new CharacterClassData();
        
        data.Name = "Gambler";
        data.Description = "A risk-taker who harnesses luck and probability to turn the tide of battle in their favor.";
        data.Type = GameEnums.ClassType.Gambler;
        data.ResourceType = GameEnums.ResourceType.Focus;
        
        // Set base attributes
        foreach (GameEnums.AttributeType attrType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            data.BaseAttributeModifiers[attrType] = 5;
        }
        
        data.BaseAttributeModifiers[GameEnums.AttributeType.Fortuity] = 9;
        data.BaseAttributeModifiers[GameEnums.AttributeType.Negotiation] = 8;
        
        // Set favored attributes
        data.FavoredAttributes[GameEnums.AttributeType.Fortuity] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Negotiation] = 1;
        data.FavoredAttributes[GameEnums.AttributeType.Offense] = 1;
        
        // Set class bonuses
        data.ClassBonuses[GameEnums.StatType.MagicFind] = 20f;
        data.ClassBonuses[GameEnums.StatType.CriticalDamage] = 10f;
        
        // Add starting cards
        data.StartingCards.Add(new CardTemplate("Lucky Strike", "Attack with increased critical chance.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Fortune's Favor", "Temporarily increase your luck.", CardRarity.Common));
        data.StartingCards.Add(new CardTemplate("Wild Card", "Play a random card with enhanced effects.", CardRarity.Common));
        
        return data;
    }
}