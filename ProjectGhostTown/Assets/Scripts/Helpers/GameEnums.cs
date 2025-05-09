using System;
using UnityEngine.UIElements;

/// <summary>
/// A centralized container for all game-related enums
/// </summary>
public static class GameEnums
{
    #region Character Attributes

    /// <summary>
    /// Defines the core attributes that characters can have
    /// </summary>
    public enum AttributeType
    {
        Fortuity,    // affects luck, drops, critical chance
        Offense,     // affects damage output
        Resilience,  // affects damage reduction, health
        Tenacity,    // affects crowd control resistance, resource
        Utility,     // affects durability, tool efficiency
        Negotiation, // affects vendor prices, dialogue options
        Expertise    // affects attack speed, critical damage
    }

    /// <summary>
    /// Defines the derived stats calculated from base attributes
    /// </summary>
    public enum StatType
    {
        PhysicalDamage,
        MagicalDamage,
        CriticalChance,
        CriticalDamage,
        AttackSpeed,
        DamageReduction,
        MagicFind
        // Add more as needed
    }

    #endregion

    #region Spells

    public enum SpellType
    {
        Attack,
        Healing,
        Buff,
        Debuff,
        Utility,
        Summoning
    }
    
    public enum DamageType
    {
        Magic,
        Fire,
        Ice,
        Lightning,
        Poison,
        True, // Ignores resistance
        Physical,
        Magical,
        Radiation
    }
    

    #endregion
    
  
    #region Character Classes

    /// <summary>
    /// Defines the available character classes
    /// </summary>
    public enum ClassType
    {
        Radiomancer,
        Brawler,
        Gunslinger,
        Gambler
    }

    /// <summary>
    /// Defines the types of resources that classes can use
    /// </summary>
    public enum ResourceType
    {
        Health,
        Mana,
        Rage,
        Energy,
        Focus,
        Radiopower
    }

    #endregion

    #region Items and Cards

    /// <summary>
    /// Defines the rarity levels for cards and items
    /// </summary>
    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
        Unique
    }

    /// <summary>
    /// Defines the types of items
    /// </summary>
    public enum ItemType
    {
        Card,
        Consumable,
        Crafting,
        Quest
    }

    #endregion

    #region Combat

    /// <summary>
    /// Defines the types of targeting for spells and abilities
    /// </summary>
    public enum TargetType
    {
        Self,
        Position,
        Direction
    }
    

    /// <summary>
    /// Defines status effect types
    /// </summary>
    public enum StatusEffectType
    {
        Buff,
        Debuff,
        OverTime, // Damage or healing over time
        Control,
        DoT_Damage,
        HoT_Healing,
        Stun,
        Slow,
        Buff_Speed,
        Debuff_Defense,
        
        
    }

    #endregion

    #region World and Environment

    /// <summary>
    /// Defines types of environments
    /// </summary>
    public enum EnvironmentType
    {
        Forest,
        Desert,
        Mountain,
        Cave,
        City,
        Ruins,
        Wasteland
    }
    
    #endregion

    #region Quests and Dialogue

    /// <summary>
    /// Defines quest states
    /// </summary>
    public enum QuestState
    {
        NotStarted,
        Active,
        Completed,
        Failed
    }

    /// <summary>
    /// Defines dialogue option types
    /// </summary>
    public enum DialogueOptionType
    {
        Friendly,
        Hostile,
        Neutral,
        Persuasion,
        Intimidation,
        Question,
        Farewell
    }

    #endregion



    
}