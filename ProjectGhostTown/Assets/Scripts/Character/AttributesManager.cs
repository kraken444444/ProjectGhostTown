using UnityEngine;

// AttributesManager.cs - New utility class for managing attributes
using System.Collections.Generic;
using UnityEngine;

public class AttributesManager
{
    // Attribute descriptions
    private static Dictionary<GameEnums.AttributeType, string> attributeDescriptions = new Dictionary<GameEnums.AttributeType, string>()
    {
        { GameEnums.AttributeType.Offense, "Increases damage output from all sources" },
        { GameEnums.AttributeType.Resilience, "Increases health and damage reduction" },
        { GameEnums.AttributeType.Tenacity, "Increases resource pools and resistance to control effects" },
        { GameEnums.AttributeType.Utility, "Improves tool efficiency and item durability" },
        { GameEnums.AttributeType.Fortuity, "Increases critical chance and item drop rates" },
        { GameEnums.AttributeType.Negotiation, "Improves vendor prices and unlocks dialogue options" },
        { GameEnums.AttributeType.Expertise, "Increases attack speed and critical damage" }
    };
    
    // Attribute colors
    private static Dictionary<GameEnums.AttributeType, Color> attributeColors = new Dictionary<GameEnums.AttributeType, Color>()
    {
        { GameEnums.AttributeType.Offense, new Color(0.8f, 0.2f, 0.2f) }, // Red
        { GameEnums.AttributeType.Resilience, new Color(0.2f, 0.6f, 0.2f) }, // Green
        { GameEnums.AttributeType.Tenacity, new Color(0.2f, 0.2f, 0.8f) }, // Blue
        { GameEnums.AttributeType.Utility, new Color(0.8f, 0.8f, 0.2f) }, // Yellow
        { GameEnums.AttributeType.Fortuity, new Color(0.8f, 0.4f, 0.8f) }, // Purple
        { GameEnums.AttributeType.Negotiation, new Color(0.4f, 0.8f, 0.8f) }, // Cyan
        { GameEnums.AttributeType.Expertise, new Color(0.8f, 0.5f, 0.2f) }  // Orange
    };
    
    // Get attribute description
    public static string GetAttributeDescription(GameEnums.AttributeType attributeType)
    {
        if (attributeDescriptions.TryGetValue(attributeType, out string description))
        {
            return description;
        }
        
        return "Unknown attribute";
    }
    
    // Get attribute color
    public static Color GetAttributeColor(GameEnums.AttributeType attributeType)
    {
        if (attributeColors.TryGetValue(attributeType, out Color color))
        {
            return color;
        }
        
        return Color.white;
    }
    
    // Get primary attributes for a class
    public static List<GameEnums.AttributeType> GetPrimaryAttributes(GameEnums.ClassType classType)
    {
        List<GameEnums.AttributeType> primary = new List<GameEnums.AttributeType>();
        
        switch (classType)
        {
            case GameEnums.ClassType.Radiomancer:
                primary.Add(GameEnums.AttributeType.Offense);
                primary.Add(GameEnums.AttributeType.Tenacity);
                break;
                
            case GameEnums.ClassType.Brawler:
                primary.Add(GameEnums.AttributeType.Offense);
                primary.Add(GameEnums.AttributeType.Resilience);
                break;
                
            case GameEnums.ClassType.Gunslinger:
                primary.Add(GameEnums.AttributeType.Offense);
                primary.Add(GameEnums.AttributeType.Expertise);
                break;
                
            case GameEnums.ClassType.Gambler:
                primary.Add(GameEnums.AttributeType.Fortuity);
                primary.Add(GameEnums.AttributeType.Negotiation);
                break;
        }
        
        return primary;
    }
    
    // Get secondary attributes for a class
    public static List<GameEnums.AttributeType> GetSecondaryAttributes(GameEnums.ClassType classType)
    {
        List<GameEnums.AttributeType> secondary = new List<GameEnums.AttributeType>();
        
        switch (classType)
        {
            case GameEnums.ClassType.Radiomancer:
                secondary.Add(GameEnums.AttributeType.Expertise);
                break;
                
            case GameEnums.ClassType.Brawler:
                secondary.Add(GameEnums.AttributeType.Tenacity);
                break;
                
            case GameEnums.ClassType.Gunslinger:
                secondary.Add(GameEnums.AttributeType.Fortuity);
                break;
                
            case GameEnums.ClassType.Gambler:
                secondary.Add(GameEnums.AttributeType.Offense);
                break;
        }
        
        return secondary;
    }
    
    // Calculate attribute point cost based on current value
    public static int GetAttributeUpgradeCost(int currentValue)
    {
        return currentValue / 5 + 1; // 1 point for values 0-4, 2 points for 5-9, etc.
    }
}