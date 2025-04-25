using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    [SerializeField] private int StartingAttributeNum = 4;
    // Base Stats
    public int Level { get; private set; } = 1;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxResource { get; private set; } 
    public int CurrentResource { get; private set; }
    public int Experience { get; private set; } = 0;
    public int ExperienceToNextLevel { get; private set; }

    public CharacterClass Class { get; private set; }
    public Subclass Subclass { get; private set; }

    public Dictionary<AttributeType, int> Attributes { get; private set; }

    public SkillTree SkillTree { get; private set; }

    private Dictionary<StatType, float> _derivedStats;

    // Level cap
    private const int MAX_LEVEL = 99;

    public Character(CharacterClass characterClass)
    {
        Class = characterClass;
        
        Attributes = new Dictionary<AttributeType, int>
        {
            { AttributeType.Fortuity,    StartingAttributeNum },
            { AttributeType.Offense,     StartingAttributeNum },
            { AttributeType.Resilience,  StartingAttributeNum },
            { AttributeType.Tenacity,    StartingAttributeNum },
            { AttributeType.Utility,     StartingAttributeNum },
            { AttributeType.Negotiation, StartingAttributeNum },
            { AttributeType.Expertise,   StartingAttributeNum }
        };
        
        //TODO:apply class base attribute modifiers
        
        //initialize derived stats
        RecalculateStats();
        
        //initialize skill tree based on class
     // TODO:  SkillTree = new SkillTree(Class);
        
        //TODO: set experience for next level
        ExperienceToNextLevel = CalculateExperienceRequired(Level + 1);
        
        // health and resource to max
        CurrentHealth = MaxHealth;
        CurrentResource = MaxResource;
    }

    public void SelectSubclass(Subclass subclass)
    {
        // cock n ball torture
        //: add subclass shit
        Subclass = subclass;
        
        //subclass modifiers
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
        
        // TOD: Apply level-up benefit
    //    SkillTree.AddSkillPoints(2); ?? probably
        
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
        _derivedStats = new Dictionary<StatType, float>();
        
        MaxHealth = 100 + (Level * 10) + (Attributes[AttributeType.Resilience] * 5);
        MaxResource = 50 + (Level * 5) + (Attributes[AttributeType.Tenacity] * 3);
        
        _derivedStats[StatType.PhysicalDamage] = CalculatePhysicalDamage();
        _derivedStats[StatType.MagicalDamage] = CalculateMagicalDamage();
        _derivedStats[StatType.CriticalChance] = CalculateCriticalChance();
        _derivedStats[StatType.CriticalDamage] = CalculateCriticalDamage();
        _derivedStats[StatType.AttackSpeed] = CalculateAttackSpeed();
        _derivedStats[StatType.DamageReduction] = CalculateDamageReduction();
        _derivedStats[StatType.MagicFind] = CalculateMagicFind();
        
      //  foreach (var activeSkill in SkillTree.GetActiveSkills())
      //  {
        //    foreach (var statModifier in activeSkill.StatModifiers)
       //     {
         //       _derivedStats[statModifier.Key] += statModifier.Value;
       //     } 
      //  } recalculate skills based on new stats
    }

    private float CalculatePhysicalDamage()
    {
        return 10f + (Attributes[AttributeType.Offense] * 1.5f);
    }

    private float CalculateMagicalDamage()
    {
        return 5f + (Attributes[AttributeType.Offense] * 1.2f);
    }

    private float CalculateCriticalChance()
    {
        return 5f + (Attributes[AttributeType.Expertise] * 0.5f);
    }

    private float CalculateCriticalDamage()
    {
        return 150f + (Attributes[AttributeType.Expertise] * 2f);
    }

    private float CalculateAttackSpeed()
    {
        return 100f + (Attributes[AttributeType.Expertise] * 0.5f);
    }

    private float CalculateDamageReduction()
    {
        return Mathf.Min(75f, Attributes[AttributeType.Resilience] * 0.5f);
    }

    private float CalculateMagicFind()
    {
        return Attributes[AttributeType.Fortuity] * 2f;
    }

   // Get a derived stat
    public float GetStat(StatType statType)
    {
        if (_derivedStats.ContainsKey(statType))
            return _derivedStats[statType];
            
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

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
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
            int penalty = (int)(Experience * 0.1f); //10% ecks dee penalty on death
            Experience = Mathf.Max(0, Experience - penalty);
            Debug.Log($"Lost {penalty} experience!");
        }
    }
    
}

public enum AttributeType
{
    Fortuity,      //affects luck, drops, critical chance
    Offense,       //affects damage output
    Resilience,    //affects damage reduction, health
    Tenacity,      //affects crowd control resistance, resource
    Utility,       //affects durability, tool efficiency
    Negotiation,   //affects vendor prices, dialogue options
    Expertise      //affects attack speed, critical damage
}

public enum StatType
{
    PhysicalDamage,
    MagicalDamage,
    CriticalChance,
    CriticalDamage,
    AttackSpeed,
    DamageReduction,
    MagicFind
    //add more as needed
}
