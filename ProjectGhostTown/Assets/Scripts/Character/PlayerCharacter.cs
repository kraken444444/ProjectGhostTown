// PlayerCharacter.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    [Header("Player Specifics")]
    [SerializeField] public GameEnums.ClassType classType;
    [SerializeField] public int level = 1;
    [SerializeField] public int maxResource = 100;
    [SerializeField] public int currentResource;
    [SerializeField] public int experience;
    [SerializeField] public int experienceToNextLevel;
    
    // Character class related
    private CharacterClass characterClass;
    private CharacterClassData characterClassData;
    
    // Derived stats
    private Dictionary<GameEnums.StatType, float> derivedStats = new Dictionary<GameEnums.StatType, float>();
    
    // Equipped spells
    [SerializeField] private List<Spell> equippedSpells = new List<Spell>();
    
    // Events specific to player character
    public event Action<int, int> OnResourceChanged;
    public event Action<int, int> OnLevelChanged;
    public event Action<Spell> OnSpellCast;
    
    protected override void Awake()
    {
        base.Awake();

        try
        {
            // Initialize character class from class type
            characterClassData = CharacterClassData.CreateFromType(classType);
            characterClass = CharacterClass.FromClassData(characterClassData);
            
            if (characterClass == null)
            {
                Debug.LogError("Failed to create character class, using default");
                characterClass = CharacterClass.CreateDefaultClass(classType);
            }
            
            // Initialize class-specific attributes
            InitializeClass();
            
            // Initialize derived stats
            RecalculateStats();
            
            // Set initial values
            if (currentResource <= 0)
                currentResource = maxResource;
            experienceToNextLevel = CalculateExpForNextLevel();
        }
        catch (Exception e)
        {
            Debug.LogError("Error in PlayerCharacter.Awake: " + e.Message + "\n" + e.StackTrace);
        }
    }
    
    private void InitializeClass()
    {
        if (characterClass == null || characterClass.BaseAttributeModifiers == null)
        {
            Debug.LogError("Character class or attributes are null in InitializeClass");
            return;
        }
        
        foreach (var modifier in characterClass.BaseAttributeModifiers)
        {
            attributes[modifier.Key] = modifier.Value;
        }
    }
    
    private void RecalculateStats()
    {
        // Calculate max health and resource
        maxHealth = 100 + (level * 10) + (attributes[GameEnums.AttributeType.Resilience] * 5);
        maxResource = 50 + (level * 5) + (attributes[GameEnums.AttributeType.Tenacity] * 3);
        
        // Calculate derived stats
        derivedStats.Clear();
        
        derivedStats[GameEnums.StatType.PhysicalDamage] = 10f + (attributes[GameEnums.AttributeType.Offense] * 1.5f);
        derivedStats[GameEnums.StatType.MagicalDamage] = 5f + (attributes[GameEnums.AttributeType.Offense] * 1.2f);
        derivedStats[GameEnums.StatType.CriticalChance] = 5f + (attributes[GameEnums.AttributeType.Expertise] * 0.5f);
        derivedStats[GameEnums.StatType.CriticalDamage] = 150f + (attributes[GameEnums.AttributeType.Expertise] * 2f);
        derivedStats[GameEnums.StatType.AttackSpeed] = 100f + (attributes[GameEnums.AttributeType.Expertise] * 0.5f);
        derivedStats[GameEnums.StatType.DamageReduction] = Mathf.Min(75f, attributes[GameEnums.AttributeType.Resilience] * 0.5f);
        derivedStats[GameEnums.StatType.MagicFind] = attributes[GameEnums.AttributeType.Fortuity] * 2f;
        
        // Apply class bonuses
        if (characterClass != null && characterClass.ClassBonuses != null)
        {
            foreach (var bonus in characterClass.ClassBonuses)
            {
                if (derivedStats.ContainsKey(bonus.Key))
                {
                    derivedStats[bonus.Key] += bonus.Value;
                }
                else
                {
                    derivedStats[bonus.Key] = bonus.Value;
                }
            }
        }
    }
    
    public override void TakeDamage(DamageInfo damageInfo)
    {
        float damageReduction = derivedStats.ContainsKey(GameEnums.StatType.DamageReduction) 
            ? derivedStats[GameEnums.StatType.DamageReduction] / 100f 
            : 0f;
        
        base.TakeDamage(damageInfo);
    }
    
    public void ConsumeResource(int amount)
    {
        // Check if enough resource
        if (currentResource < amount)
            return;
            
        // Apply resource cost
        int previousResource = currentResource;
        currentResource = Mathf.Max(0, currentResource - amount);
        
        // Debug output
        Debug.Log($"{characterName} used {amount} resource. Remaining: {currentResource}/{maxResource}");
        
        // Trigger event
        OnResourceChanged?.Invoke(previousResource, currentResource);
    }
    
    public void RestoreResource(int amount)
    {
        // Apply resource restoration
        int previousResource = currentResource;
        currentResource = Mathf.Min(maxResource, currentResource + amount);
        
        // Debug output
        Debug.Log($"{characterName} restored {amount} resource. Current: {currentResource}/{maxResource}");
        
        // Trigger event
        OnResourceChanged?.Invoke(previousResource, currentResource);
    }
    
    public void CastSpell(int spellIndex, Vector3 targetPosition)
    {
        // Check if stunned
        if (_isStunned)
        {
            Debug.Log($"{characterName} is stunned and cannot cast!");
            return;
        }
            
        // Check if dead
        if (_isDead)
            return;
            
        // Check spell index
        if (spellIndex < 0 || spellIndex >= equippedSpells.Count)
            return;
            
        Spell spell = equippedSpells[spellIndex];
        
        // Check resource cost
        if (currentResource < spell.resourceCost)
        {
            Debug.Log($"Not enough resource to cast {spell.spellName}. Required: {spell.resourceCost}, Current: {currentResource}");
            return;
        }
        
        // Check cooldown
        float remainingCooldown = SpellManager.Instance.GetRemainingCooldown(this, spell);
        if (remainingCooldown > 0)
        {
            Debug.Log($"{spell.spellName} on cooldown: {remainingCooldown:F1}s remaining");
            return;
        }
        
        // Check range
        if (spell.targetType != GameEnums.TargetType.Self)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > spell.range)
            {
                Debug.Log($"Target out of range! Max range: {spell.range}, Current: {distance:F1}");
                return;
            }
        }
        
        // Cast the spell
        bool success = SpellManager.Instance.CastSpell(this, targetPosition, spell);
        
        // Handle success
        if (success)
        {
            Debug.Log($"{characterName} cast {spell.spellName}");
            
            // Consume resource
            ConsumeResource(spell.resourceCost);
            
            // Trigger event
            OnSpellCast?.Invoke(spell);
        }
    }
    
    // Experience and leveling methods
    public void GainExperience(int amount)
    {
        // Check if at max level (assumed level cap of 100)
        if (level >= 100)
            return;
            
        // Apply experience gain
        int previousExperience = experience;
        experience += amount;
        
        Debug.Log($"{characterName} gained {amount} XP. Total: {experience}/{experienceToNextLevel}");
        
        // Check for level up
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        // Calculate experience overflow
        int experienceOverflow = experience - experienceToNextLevel;
        
        // Increase level
        int previousLevel = level;
        level++;
        
        // Update experience values
        experience = experienceOverflow;
        experienceToNextLevel = CalculateExpForNextLevel();
        
        // Recalculate stats
        RecalculateStats();
        
        // Restore health and resource
        currentHealth = maxHealth;
        currentResource = maxResource;
        
        Debug.Log($"{characterName} leveled up to level {level}!");
        
        // Trigger event
        OnLevelChanged?.Invoke(previousLevel, level);
    }
    
    private int CalculateExpForNextLevel()
    {
        return 100 * level * level;
    }
    
    // Utility methods
    public float GetStat(GameEnums.StatType statType)
    {
        if (derivedStats.ContainsKey(statType))
            return derivedStats[statType];
            
        return 0f;
    }
    
    public bool EquipSpell(Spell spell)
    {
        if (spell == null)
            return false;
            
        // Check if already at max spells (arbitrary limit of 6)
        if (equippedSpells.Count >= 6)
            return false;
            
        // Check requirements
        if (level < spell.levelRequirement)
            return false;
            
        if (spell.classRequirement != classType)
            return false;
            
        // Add spell
        equippedSpells.Add(spell);
        Debug.Log($"{characterName} equipped {spell.spellName}");
        return true;
    }
    
    public bool UnequipSpell(int index)
    {
        if (index < 0 || index >= equippedSpells.Count)
            return false;
            
        string spellName = equippedSpells[index].spellName;
        equippedSpells.RemoveAt(index);
        Debug.Log($"{characterName} unequipped {spellName}");
        return true;
    }
    
    public int GetAttributeValue(GameEnums.AttributeType attributeType)
    {
        return attributes[attributeType];
    }
    
    public void ModifyAttribute(GameEnums.AttributeType attributeType, int amount)
    {
        int oldValue = attributes[attributeType];
        attributes[attributeType] += amount;
        Debug.Log($"{characterName}'s {attributeType} changed from {oldValue} to {attributes[attributeType]}");
        
        // Recalculate stats since attributes changed
        RecalculateStats();
    }
    
    public CharacterClass GetCharacterClass()
    {
        return characterClass;
    }
    
    public void ResetCooldowns()
    {
        foreach (var spell in equippedSpells)
        {
            SpellManager.Instance.ResetCooldown(this, spell);
        }
        Debug.Log($"Reset all cooldowns for {characterName}");
    }
}