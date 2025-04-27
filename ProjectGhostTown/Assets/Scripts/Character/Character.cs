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

    public Dictionary<AttributeType, int> Attributes { get; set; }

    public SkillTree SkillTree { get; set; }

    private Dictionary<StatType, float> derivedStats;

    // Level cap
    private const int MAX_LEVEL = 99;
    
    private void Start()
    { 
        transform = GameObject.FindGameObjectWithTag("Player").transform;
        characterMain = GameObject.FindGameObjectWithTag("Player");
        
    }
    public Character(CharacterClass characterClass)
    {
       // Class.Type = characterClass.Type;
       //TODO 
        Class = CharacterClass.CreateClass(ClassType.Brawler);
        
        Attributes = Class.BaseAttributeModifiers;
    

       

        
        
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
        derivedStats = new Dictionary<StatType, float>();
        
        MaxHealth = 100 + (Level * 10) + (Attributes[AttributeType.Resilience] * 5);
        MaxResource = 50 + (Level * 5) + (Attributes[AttributeType.Tenacity] * 3);
        
        derivedStats[StatType.PhysicalDamage] = CalculatePhysicalDamage();
        derivedStats[StatType.MagicalDamage] = CalculateMagicalDamage();
        derivedStats[StatType.CriticalChance] = CalculateCriticalChance();
        derivedStats[StatType.CriticalDamage] = CalculateCriticalDamage();
        derivedStats[StatType.AttackSpeed] = CalculateAttackSpeed();
        derivedStats[StatType.DamageReduction] = CalculateDamageReduction();
        derivedStats[StatType.MagicFind] = CalculateMagicFind();
        
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
