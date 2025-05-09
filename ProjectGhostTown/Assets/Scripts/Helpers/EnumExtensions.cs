// EnumExtensions.cs - New helper class
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
    public static GameEnums.ResourceType GetDefaultResourceType(this GameEnums.ClassType classType)
    {
        switch (classType)
        {
            case GameEnums.ClassType.Radiomancer:
                return GameEnums.ResourceType.Radiopower;
            case GameEnums.ClassType.Brawler:
                return GameEnums.ResourceType.Rage;
            case GameEnums.ClassType.Gunslinger:
                return GameEnums.ResourceType.Energy;
            case GameEnums.ClassType.Gambler:
                return GameEnums.ResourceType.Focus;
            default:
                Debug.LogWarning($"Unknown class type: {classType}, defaulting to Mana");
                return GameEnums.ResourceType.Mana;
        }
    }
    
    public static List<GameEnums.AttributeType> GetFavoredAttributes(this GameEnums.ClassType classType)
    {
        List<GameEnums.AttributeType> favoredAttributes = new List<GameEnums.AttributeType>();
        
        switch (classType)
        {
            case GameEnums.ClassType.Radiomancer:
                favoredAttributes.Add(GameEnums.AttributeType.Offense);
                favoredAttributes.Add(GameEnums.AttributeType.Tenacity);
                favoredAttributes.Add(GameEnums.AttributeType.Expertise);
                break;
                
            case GameEnums.ClassType.Brawler:
                favoredAttributes.Add(GameEnums.AttributeType.Offense);
                favoredAttributes.Add(GameEnums.AttributeType.Resilience);
                favoredAttributes.Add(GameEnums.AttributeType.Tenacity);
                break;
                
            case GameEnums.ClassType.Gunslinger:
                favoredAttributes.Add(GameEnums.AttributeType.Offense);
                favoredAttributes.Add(GameEnums.AttributeType.Expertise);
                favoredAttributes.Add(GameEnums.AttributeType.Fortuity);
                break;
                
            case GameEnums.ClassType.Gambler:
                favoredAttributes.Add(GameEnums.AttributeType.Fortuity);
                favoredAttributes.Add(GameEnums.AttributeType.Negotiation);
                favoredAttributes.Add(GameEnums.AttributeType.Offense);
                break;
                
            default:
                Debug.LogWarning($"Unknown class type: {classType}, no favored attributes assigned");
                break;
        }
        
        return favoredAttributes;
    }
}