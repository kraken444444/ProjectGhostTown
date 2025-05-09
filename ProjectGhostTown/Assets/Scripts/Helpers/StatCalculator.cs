// StatCalculator.cs - Updated version
using System.Collections.Generic;
using UnityEngine;

public static class StatCalculator
{
    public static int CalculateMaxHealth(int level, int resilience)
    {
        return 100 + (level * 10) + (resilience * 5);
    }
    
    public static int CalculateMaxResource(int level, int tenacity, GameEnums.ResourceType resourceType)
    {
        int baseAmount = 50 + (level * 5) + (tenacity * 3);
        
        // Adjust based on resource type
        switch (resourceType)
        {
            case GameEnums.ResourceType.Rage:
                return Mathf.RoundToInt(baseAmount * 0.8f); // Less resource but more powerful
            case GameEnums.ResourceType.Energy:
                return Mathf.RoundToInt(baseAmount * 1.2f); // More resource but less powerful per point
            case GameEnums.ResourceType.Radiopower:
                return Mathf.RoundToInt(baseAmount * 1.1f); // Slightly more resource
            default:
                return baseAmount;
        }
    }
    
    public static Dictionary<GameEnums.StatType, float> CalculateDerivedStats(
        BaseCharacter character,
        CharacterClass characterClass = null)
    {
        if (character == null)
        {
            Debug.LogError("Cannot calculate stats for null character");
            return new Dictionary<GameEnums.StatType, float>();
        }
        
        Attributes attributes = character.GetAttributes();
        if (attributes == null)
        {
            Debug.LogError("Character has null attributes");
            return new Dictionary<GameEnums.StatType, float>();
        }
        
        Dictionary<GameEnums.StatType, float> stats = new Dictionary<GameEnums.StatType, float>();
        
        // Get level if PlayerCharacter
        int level = 1;
        if (character is PlayerCharacter playerCharacter)
        {
            // TODO: Get level from player character
        }
        
        // Base stats from attributes
        stats[GameEnums.StatType.PhysicalDamage] = 10f + (attributes[GameEnums.AttributeType.Offense] * 1.5f) + (level * 0.5f);
        stats[GameEnums.StatType.MagicalDamage] = 5f + (attributes[GameEnums.AttributeType.Offense] * 1.2f) + (level * 0.5f);
        stats[GameEnums.StatType.CriticalChance] = 5f + (attributes[GameEnums.AttributeType.Expertise] * 0.5f);
        stats[GameEnums.StatType.CriticalDamage] = 150f + (attributes[GameEnums.AttributeType.Expertise] * 2f);
        stats[GameEnums.StatType.AttackSpeed] = 100f + (attributes[GameEnums.AttributeType.Expertise] * 0.5f);
        stats[GameEnums.StatType.DamageReduction] = Mathf.Min(75f, attributes[GameEnums.AttributeType.Resilience] * 0.5f);
        stats[GameEnums.StatType.MagicFind] = attributes[GameEnums.AttributeType.Fortuity] * 2f;
        
        // Apply class bonuses if available
        if (characterClass != null && characterClass.ClassBonuses != null)
        {
            foreach (var bonus in characterClass.ClassBonuses)
            {
                if (stats.ContainsKey(bonus.Key))
                {
                    stats[bonus.Key] += bonus.Value;
                }
                else
                {
                    stats[bonus.Key] = bonus.Value;
                }
            }
        }
        
        return stats;
    }
    
    public static float CalculateStatValue(BaseCharacter character, GameEnums.StatType statType)
    {
        if (character == null)
        {
            Debug.LogError("Cannot calculate stat for null character");
            return 0f;
        }
        
        Attributes attributes = character.GetAttributes();
        if (attributes == null)
        {
            Debug.LogError("Character has null attributes");
            return 0f;
        }
        
        // Calculate derived stats based on attributes
        switch (statType)
        {
            case GameEnums.StatType.CriticalChance:
                // Crit chance based on Fortuity
                return attributes[GameEnums.AttributeType.Fortuity] * 0.5f;
            
            case GameEnums.StatType.CriticalDamage:
                // Crit damage based on Expertise (100% base + bonus)
                return 1.0f + (attributes[GameEnums.AttributeType.Expertise] * 0.1f);
            
            case GameEnums.StatType.PhysicalDamage:
                return attributes[GameEnums.AttributeType.Offense] * 1.5f;
            
            case GameEnums.StatType.MagicalDamage:
                return attributes[GameEnums.AttributeType.Offense] * 1.2f;
            
            case GameEnums.StatType.DamageReduction:
                return attributes[GameEnums.AttributeType.Resilience] * 0.5f;
            
            case GameEnums.StatType.AttackSpeed:
                return 100f + (attributes[GameEnums.AttributeType.Expertise] * 2f);
            
            case GameEnums.StatType.MagicFind:
                return attributes[GameEnums.AttributeType.Fortuity] * 2f;
            
            default:
                return 0f;
        }
    }
    
    public static int CalculateExperienceToNextLevel(int currentLevel)
    {
        return 100 * currentLevel * currentLevel;
    }
    
    public static int CalculateSpellDamage(Spell spell, BaseCharacter caster)
    {
        if (spell == null || caster == null)
        {
            return 0;
        }
        
        Attributes attributes = caster.GetAttributes();
        if (attributes == null)
        {
            return spell.baseDamage;
        }
        
        float primaryStatValue = attributes[spell.primaryScalingStat];
        float secondaryStatValue = attributes[spell.secondaryScalingStat];
        
        return Mathf.RoundToInt(spell.baseDamage + 
                               (primaryStatValue * spell.primaryScalingFactor) + 
                               (secondaryStatValue * spell.secondaryScalingFactor));
    }
    
    public static int CalculateHealing(Spell spell, BaseCharacter caster)
    {
        if (spell == null || caster == null)
        {
            return 0;
        }
        
        Attributes attributes = caster.GetAttributes();
        if (attributes == null)
        {
            return spell.baseHealing;
        }
        
        float primaryStatValue = attributes[spell.primaryScalingStat];
        float secondaryStatValue = attributes[spell.secondaryScalingStat];
        
        return Mathf.RoundToInt(spell.baseHealing + 
                               (primaryStatValue * spell.primaryScalingFactor) + 
                               (secondaryStatValue * spell.secondaryScalingFactor));
    }
}