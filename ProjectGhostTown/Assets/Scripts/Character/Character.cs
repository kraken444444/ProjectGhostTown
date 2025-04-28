using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    // Base Stats
    public int Level { get;  set; } = 1;
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxResource { get; set; } 
    public int CurrentResource { get; set; }
    public int Experience { get; set; } = 0;
    public int ExperienceToNextLevel { get; set; }
    public string ID { get; set; }

    public Transform transform;

    public GameObject characterMain;
    public CharacterClass Class { get; set; }
    public Subclass Subclass { get; set; }

    // Replace direct Dictionary with Attributes class
    public Attributes Attributes { get; private set; }

    public SkillTree SkillTree { get; set; }

    private Dictionary<GameEnums.StatType, float> derivedStats;

    // Level cap
    private const int MAX_LEVEL = 99;
    
    private void Start()
    { 
        transform = GameObject.FindGameObjectWithTag("Player").transform;
        characterMain = GameObject.FindGameObjectWithTag("Player");
    }
    
    public Character(CharacterClass characterClass)
    {
        GameEnums.ClassType type = characterClass.Type;
        Class = CharacterClass.CreateClass(type);
        
        // Initialize attributes with class base modifiers
        Attributes = new Attributes(Class.BaseAttributeModifiers);
        
        // Initialize derived stats
        RecalculateStats();
        
        // Initialize skill tree based on class
        // TODO: SkillTree = new SkillTree(Class);
        
        // Set experience for next level
        ExperienceToNextLevel = CalculateExperienceRequired(Level + 1);
        
        // Health and resource to max
        CurrentHealth = MaxHealth;
        CurrentResource = MaxResource;
    }

    public void SelectSubclass(Subclass subclass)
    {
        Subclass = subclass;
        
        // Apply subclass attribute modifiers if they exist
        if (subclass.AttributeModifiers != null)
        {
            Attributes.ApplyModifiers(subclass.AttributeModifiers);
        }
        
        // Recalculate stats with new modifiers
        RecalculateStats();
    }

    public void GainExperience(int amount)
    {
        if (Level >= MAX_LEVEL)
            return;
            
        Experience += amount;
        
        while (Experience >= ExperienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        
        int previousRequirement = ExperienceToNextLevel;
        ExperienceToNextLevel = CalculateExperienceRequired(Level + 1);
        
        // TODO: Apply level-up benefits
        // SkillTree.AddSkillPoints(2);
        
        RecalculateStats();
        
        CurrentHealth = MaxHealth;
        CurrentResource = MaxResource;
    }

    private int CalculateExperienceRequired(int targetLevel)
    {
        return 100 * targetLevel * targetLevel;
    }

    public void RecalculateStats()
    {
        derivedStats = new Dictionary<GameEnums.StatType, float>();
        
        MaxHealth = 100 + (Level * 10) + (Attributes[GameEnums.AttributeType.Resilience] * 5);
        MaxResource = 50 + (Level * 5) + (Attributes[GameEnums.AttributeType.Tenacity] * 3);
        
        derivedStats[GameEnums.StatType.PhysicalDamage] = CalculatePhysicalDamage();
        derivedStats[GameEnums.StatType.MagicalDamage] = CalculateMagicalDamage();
        derivedStats[GameEnums.StatType.CriticalChance] = CalculateCriticalChance();
        derivedStats[GameEnums.StatType.CriticalDamage] = CalculateCriticalDamage();
        derivedStats[GameEnums.StatType.AttackSpeed] = CalculateAttackSpeed();
        derivedStats[GameEnums.StatType.DamageReduction] = CalculateDamageReduction();
        derivedStats[GameEnums.StatType.MagicFind] = CalculateMagicFind();
        
        // Apply class bonuses
        if (Class != null && Class.ClassBonuses != null)
        {
            foreach (var bonus in Class.ClassBonuses)
            {
                if (derivedStats.ContainsKey(bonus.Key))
                {
                    derivedStats[bonus.Key] += bonus.Value;
                }
            }
        }
        
        // TODO: Apply skill tree effects
        /*
        foreach (var activeSkill in SkillTree.GetActiveSkills())
        {
            foreach (var statModifier in activeSkill.StatModifiers)
            {
                derivedStats[statModifier.Key] += statModifier.Value;
            } 
        }
        */
    }

    private float CalculatePhysicalDamage()
    {
        return 10f + (Attributes[GameEnums.AttributeType.Offense] * 1.5f);
    }

    private float CalculateMagicalDamage()
    {
        return 5f + (Attributes[GameEnums.AttributeType.Offense] * 1.2f);
    }

    private float CalculateCriticalChance()
    {
        return 5f + (Attributes[GameEnums.AttributeType.Expertise] * 0.5f);
    }

    private float CalculateCriticalDamage()
    {
        return 150f + (Attributes[GameEnums.AttributeType.Expertise] * 2f);
    }

    private float CalculateAttackSpeed()
    {
        return 100f + (Attributes[GameEnums.AttributeType.Expertise] * 0.5f);
    }

    private float CalculateDamageReduction()
    {
        return Mathf.Min(75f, Attributes[GameEnums.AttributeType.Resilience] * 0.5f);
    }

    private float CalculateMagicFind()
    {
        return Attributes[GameEnums.AttributeType.Fortuity] * 2f;
    }

    // Get a derived stat
    public float GetStat(GameEnums.StatType statType)
    {
        if (derivedStats.ContainsKey(statType))
            return derivedStats[statType];
            
        return 0f;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void UseResource(int amount)
    {
        CurrentResource = Mathf.Max(0, CurrentResource - amount);
    }

    public void RestoreResource(int amount)
    {
        CurrentResource = Mathf.Min(MaxResource, CurrentResource + amount);
    }

    private void Die()
    {
        Debug.Log("Character died!");
        
        if (Level > 1)
        {
            int penalty = (int)(Experience * 0.1f); // 10% XP penalty on death
            Experience = Mathf.Max(0, Experience - penalty);
            Debug.Log($"Lost {penalty} experience!");
        }
    }
}