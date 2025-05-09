using UnityEngine;
// DamageCalculator.cs - New utility class for damage calculations
using UnityEngine;

public static class DamageCalculator
{
    // Calculate damage for an attack
    public static int CalculateDamage(
        BaseCharacter attacker, 
        BaseCharacter target, 
        int baseDamage, 
        GameEnums.DamageType damageType,
        float critChanceModifier = 0f,
        float critDamageModifier = 0f)
    {
        if (attacker == null || target == null)
        {
            return baseDamage;
        }
        
        // Get relevant stats
        float attackerOffense = attacker.GetAttributeValue(GameEnums.AttributeType.Offense);
        float targetResilience = target.GetAttributeValue(GameEnums.AttributeType.Resilience);
        
        // Apply offensive multiplier based on damage type
        float damageMultiplier = 1.0f;
        switch (damageType)
        {
            case GameEnums.DamageType.Physical:
                damageMultiplier = attacker.GetStatValue(GameEnums.StatType.PhysicalDamage) / 100f;
                break;
                
            case GameEnums.DamageType.Magical:
            case GameEnums.DamageType.Fire:
            case GameEnums.DamageType.Ice:
            case GameEnums.DamageType.Lightning:
            case GameEnums.DamageType.Poison:
            case GameEnums.DamageType.Radiation:
                damageMultiplier = attacker.GetStatValue(GameEnums.StatType.MagicalDamage) / 100f;
                break;
                
            case GameEnums.DamageType.True:
                damageMultiplier = 1.0f; // True damage ignores resistances
                break;
        }
        
        // Apply damage reduction (except for True damage)
        float damageReduction = 0f;
        if (damageType != GameEnums.DamageType.True)
        {
            damageReduction = target.GetStatValue(GameEnums.StatType.DamageReduction) / 100f;
        }
        
        // Calculate base damage after multipliers
        float damage = baseDamage * damageMultiplier * (1f - damageReduction);
        
        // Check for critical hit
        bool isCritical = false;
        float critChance = attacker.GetStatValue(GameEnums.StatType.CriticalChance) + critChanceModifier;
        if (Random.Range(0f, 100f) <= critChance)
        {
            isCritical = true;
            float critDamage = attacker.GetStatValue(GameEnums.StatType.CriticalDamage) + critDamageModifier;
            damage *= critDamage / 100f;
        }
        
        // Round to int
        int finalDamage = Mathf.RoundToInt(damage);
        
        // Ensure minimum damage
        finalDamage = Mathf.Max(1, finalDamage);
        
        // Create and return damage info
        return finalDamage;
    }
    
    // Create a DamageInfo struct with calculated damage
    public static DamageInfo CreateDamageInfo(
        ISpellCaster source,
        BaseCharacter target,
        int baseDamage,
        GameEnums.DamageType damageType,
        float critChanceModifier = 0f,
        float critDamageModifier = 0f,
        float knockbackForce = 0f,
        float stunDuration = 0f)
    {
        // Validate parameters
        if (source == null || target == null)
        {
            return new DamageInfo(source, baseDamage, damageType);
        }
        
        BaseCharacter attacker = source as BaseCharacter;
        if (attacker == null)
        {
            return new DamageInfo(source, baseDamage, damageType);
        }
        
        // Calculate damage
        int damage = CalculateDamage(attacker, target, baseDamage, damageType, critChanceModifier, critDamageModifier);
        
        // Check for critical hit
        bool isCritical = false;
        float critChance = attacker.GetStatValue(GameEnums.StatType.CriticalChance) + critChanceModifier;
        if (Random.Range(0f, 100f) <= critChance)
        {
            isCritical = true;
        }
        
        // Create and return damage info
        return new DamageInfo(source, damage, damageType, isCritical, knockbackForce, stunDuration);
    }
}