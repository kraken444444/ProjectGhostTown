using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for working with game enums
/// </summary>
public static class EnumInfo
{
    #region AttributeType Extensions

    /// <summary>
    /// Gets the color associated with an attribute type for UI display
    /// </summary>
    public static Color GetColor(this GameEnums.AttributeType attributeType)
    {
        switch (attributeType)
        {
            case GameEnums.AttributeType.Offense:
                return new Color(0.8f, 0.2f, 0.2f); // Red
            case GameEnums.AttributeType.Resilience:
                return new Color(0.2f, 0.6f, 0.2f); // Green
            case GameEnums.AttributeType.Tenacity:
                return new Color(0.2f, 0.2f, 0.8f); // Blue
            case GameEnums.AttributeType.Utility:
                return new Color(0.8f, 0.8f, 0.2f); // Yellow
            case GameEnums.AttributeType.Fortuity:
                return new Color(0.8f, 0.4f, 0.8f); // Purple
            case GameEnums.AttributeType.Negotiation:
                return new Color(0.4f, 0.8f, 0.8f); // Cyan
            case GameEnums.AttributeType.Expertise:
                return new Color(0.8f, 0.5f, 0.2f); // Orange
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Gets the description of an attribute type
    /// </summary>
    public static string GetDescription(this GameEnums.AttributeType attributeType)
    {
        switch (attributeType)
        {
            case GameEnums.AttributeType.Offense:
                return "Increases damage output from all sources";
            case GameEnums.AttributeType.Resilience:
                return "Increases health and damage reduction";
            case GameEnums.AttributeType.Tenacity:
                return "Increases resource pools and resistance to control effects";
            case GameEnums.AttributeType.Utility:
                return "Improves tool efficiency and item durability";
            case GameEnums.AttributeType.Fortuity:
                return "Increases critical chance and item drop rates";
            case GameEnums.AttributeType.Negotiation:
                return "Improves vendor prices and unlocks dialogue options";
            case GameEnums.AttributeType.Expertise:
                return "Increases attack speed and critical damage";
            default:
                return "Unknown attribute";
        }
    }

    /// <summary>
    /// Gets the icon name for an attribute type
    /// </summary>
    public static string GetIconName(this GameEnums.AttributeType attributeType)
    {
        return $"Icon_Attribute_{attributeType}";
    }

    #endregion

    #region ClassType Extensions

    /// <summary>
    /// Gets the default resource type for a class
    /// </summary>
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
                return GameEnums.ResourceType.Mana;
        }
    }

    /// <summary>
    /// Gets the favored attributes for a class
    /// </summary>
    public static List<GameEnums.AttributeType> GetFavoredAttributes(this GameEnums.ClassType classType)
    {
        switch (classType)
        {
            case GameEnums.ClassType.Radiomancer:
                return new List<GameEnums.AttributeType>
                {
                    GameEnums.AttributeType.Offense,
                    GameEnums.AttributeType.Tenacity,
                    GameEnums.AttributeType.Expertise
                };
            case GameEnums.ClassType.Brawler:
                return new List<GameEnums.AttributeType>
                {
                    GameEnums.AttributeType.Offense,
                    GameEnums.AttributeType.Resilience,
                    GameEnums.AttributeType.Tenacity
                };
            case GameEnums.ClassType.Gunslinger:
                return new List<GameEnums.AttributeType>
                {
                    GameEnums.AttributeType.Offense,
                    GameEnums.AttributeType.Expertise,
                    GameEnums.AttributeType.Fortuity
                };
            case GameEnums.ClassType.Gambler:
                return new List<GameEnums.AttributeType>
                {
                    GameEnums.AttributeType.Fortuity,
                    GameEnums.AttributeType.Negotiation,
                    GameEnums.AttributeType.Offense
                };
            default:
                return new List<GameEnums.AttributeType>();
        }
    }

    #endregion

    #region StatType Extensions

    /// <summary>
    /// Gets the format string for displaying a stat value
    /// </summary>
    public static string GetFormatString(this GameEnums.StatType statType)
    {
        switch (statType)
        {
            case GameEnums.StatType.CriticalChance:
            case GameEnums.StatType.DamageReduction:
            case GameEnums.StatType.MagicFind:
                return "{0:F1}%"; // Percentage with 1 decimal
            case GameEnums.StatType.CriticalDamage:
                return "{0:F0}%"; // Percentage with no decimal
            case GameEnums.StatType.AttackSpeed:
                return "{0:F0}%"; // Percentage with no decimal
            default:
                return "{0:F1}"; // Simple number with 1 decimal
        }
    }

    /// <summary>
    /// Formats a stat value as a string according to its type
    /// </summary>
    public static string FormatValue(this GameEnums.StatType statType, float value)
    {
        return string.Format(statType.GetFormatString(), value);
    }

    #endregion
    



}