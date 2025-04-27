using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Central manager for all character-related functionality
public class CharacterManager : SerializedMonoBehaviour
{
    #region Singleton
    public static CharacterManager Instance { get; private set; }
    
    [BoxGroup("References")]
    [SerializeField, Required]
    private Character playerCharacter;
    private GameObject playerCharacterGameObject;
    
    [ShowInInspector, ReadOnly]
    public Character PlayerCharacter => playerCharacter;
    #endregion
    
    #region Character Creation & Management
    [BoxGroup("Character Creation")]
    [EnumToggleButtons]
    [OnValueChanged("OnClassTypeChanged")]
    [SerializeField] private ClassType selectedClassType = ClassType.Radiomancer;
    
    [BoxGroup("Character Creation")]
    [SerializeField] private int startingAttributePoints = 8;
    
    [BoxGroup("Experience")]
    [ProgressBar(0, "experienceForNextLevel")]
    [ShowInInspector] 
    private int currentExperience = 0;
    
    [BoxGroup("Experience")]
    [ShowInInspector] 
    private int experienceForNextLevel = 100;
    
    [BoxGroup("Stats")]
    [ShowInInspector]
    private Dictionary<StatType, float> derivedStats = new Dictionary<StatType, float>();
    
    [BoxGroup("Attributes")]
    [ShowInInspector]
    private Dictionary<AttributeType, int> attributes = new Dictionary<AttributeType, int>();
    
    [BoxGroup("Resources")]
    [ProgressBar(0, "maxHealth", r: 1, g: 0, b: 0)]
    [ShowInInspector] 
    private int currentHealth;
    
    [BoxGroup("Resources")]
    [ShowInInspector] 
    private int maxHealth;
    
    [BoxGroup("Resources")]
    [ProgressBar(0, "maxResource", r: 0, g: 0.5f, b: 1)]
    [ShowInInspector] 
    private int currentResource;
    
    [BoxGroup("Resources")]
    [ShowInInspector] 
    private int maxResource;
    
    // Level  cap
    private const int MAX_LEVEL = 99;
    #endregion
    
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (playerCharacter == null)
        {
            CreateNewCharacter(selectedClassType);
        }
        else
        {
            SyncFromCharacter();
        }
    }
    
    private void SyncFromCharacter()
    {
        if (playerCharacter != null)
        {
            currentHealth = playerCharacter.CurrentHealth;
            maxHealth = playerCharacter.MaxHealth;
            currentResource = playerCharacter.CurrentResource;
            maxResource = playerCharacter.MaxResource;
            currentExperience = playerCharacter.Experience;
            experienceForNextLevel = playerCharacter.ExperienceToNextLevel;
            
            //clone attributes and stats to avoid reference issues
            attributes = new Dictionary<AttributeType, int>(playerCharacter.Attributes);
            
            // populate derived info
            derivedStats = new Dictionary<StatType, float>();
            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
            {
                derivedStats[statType] = playerCharacter.GetStat(statType);
            }
        }
    }
    
    #region Character Creation Methods
    [Button("Create New Character", ButtonSizes.Large), GUIColor(0, 0.8f, 0)]
    public CharacterClass CreateNewCharacter(ClassType classType)
    {
        CharacterClass characterClass = CharacterClass.CreateClass(classType);
        playerCharacter = new Character(characterClass);
        playerCharacter.ID = System.Guid.NewGuid().ToString();
        
        // init manager data
        SyncFromCharacter();
        
        Debug.Log($"Created new {classType} character!");
        return characterClass;
    }
    
    private void OnClassTypeChanged()
    {
        if (Application.isPlaying)
        {
            CreateNewCharacter(selectedClassType);
        }
    }
    
    [Button("Reset Attributes")]
    public void ResetAttributes()
    {
        if (playerCharacter != null)
        {
            foreach (AttributeType attributeType in System.Enum.GetValues(typeof(AttributeType)))
            {
                attributes[attributeType] = startingAttributePoints;
            }
            
            // Apply class modifiers
            if (playerCharacter.Class != null)
            {
                foreach (var modifier in playerCharacter.Class.BaseAttributeModifiers)
                {
                    attributes[modifier.Key] += modifier.Value;
                }
            }
            
            RecalculateStats();
        }
    }
    #endregion
    
    #region Experience and Leveling
    [Button("Add Experience")]
    public void GainExperience(int amount)
    {
        if (playerCharacter == null || playerCharacter.Level >= MAX_LEVEL)
            return;
            
        currentExperience += amount;
        playerCharacter.Experience = currentExperience;
        
        while (currentExperience >= experienceForNextLevel)
        {
            LevelUp();
        }
        
        Debug.Log($"Gained {amount} experience. Total: {currentExperience}/{experienceForNextLevel}");
    }
    
    private void LevelUp()
    {
        if (playerCharacter.Level >= MAX_LEVEL)
            return;
            
        playerCharacter.Level++;
        
        experienceForNextLevel = CalculateExperienceRequired(playerCharacter.Level + 1);
        playerCharacter.ExperienceToNextLevel = experienceForNextLevel;
        
        // AddSkillPoints(2);
        
        RecalculateStats();
        
        currentHealth = maxHealth;
        currentResource = maxResource;
        playerCharacter.CurrentHealth = currentHealth;
        playerCharacter.CurrentResource = currentResource;
        
        Debug.Log($"Level up! Now level {playerCharacter.Level}");
    }
    
    private int CalculateExperienceRequired(int targetLevel)
    {
        return 100 * targetLevel * targetLevel;
    }
    #endregion
    
    #region Stat Calculation
    [Button("Recalculate Stats")]
    public void RecalculateStats()
    {
        if (playerCharacter == null)
            return;
            
        foreach (var attr in attributes)
        {
            playerCharacter.Attributes[attr.Key] = attr.Value;
        }
        
        maxHealth = 100 + (playerCharacter.Level * 10) + (attributes[AttributeType.Resilience] * 5);
        maxResource = 50 + (playerCharacter.Level * 5) + (attributes[AttributeType.Tenacity] * 3);
        
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentResource = Mathf.Min(currentResource, maxResource);
        
        playerCharacter.MaxHealth = maxHealth;
        playerCharacter.CurrentHealth = currentHealth;
        playerCharacter.MaxResource = maxResource;
        playerCharacter.CurrentResource = currentResource;
        
        derivedStats[StatType.PhysicalDamage] = CalculatePhysicalDamage();
        derivedStats[StatType.MagicalDamage] = CalculateMagicalDamage();
        derivedStats[StatType.CriticalChance] = CalculateCriticalChance();
        derivedStats[StatType.CriticalDamage] = CalculateCriticalDamage();
        derivedStats[StatType.AttackSpeed] = CalculateAttackSpeed();
        derivedStats[StatType.DamageReduction] = CalculateDamageReduction();
        derivedStats[StatType.MagicFind] = CalculateMagicFind();
        
        if (playerCharacter.Class != null && playerCharacter.Class.ClassBonuses != null)
        {
            foreach (var bonus in playerCharacter.Class.ClassBonuses)
            {
                if (derivedStats.ContainsKey(bonus.Key))
                {
                    derivedStats[bonus.Key] += bonus.Value;
                }
            }
        }
        
        // Apply skill effects (when we have a proper SkillTree)
        /* 
        foreach (var activeSkill in SkillTree.GetActiveSkills())
        {
            foreach (var statModifier in activeSkill.StatModifiers)
            {
                _derivedStats[statModifier.Key] += statModifier.Value;
            } 
        }
        */
        
        playerCharacter.RecalculateStats();
    }
    
    private float CalculatePhysicalDamage()
    {
        return 10f + (attributes[AttributeType.Offense] * 1.5f);
    }
    
    private float CalculateMagicalDamage()
    {
        return 5f + (attributes[AttributeType.Offense] * 1.2f);
    }
    
    private float CalculateCriticalChance()
    {
        return 5f + (attributes[AttributeType.Expertise] * 0.5f);
    }
    
    private float CalculateCriticalDamage()
    {
        return 150f + (attributes[AttributeType.Expertise] * 2f);
    }
    
    private float CalculateAttackSpeed()
    {
        return 100f + (attributes[AttributeType.Expertise] * 0.5f);
    }
    
    private float CalculateDamageReduction()
    {
        return Mathf.Min(75f, attributes[AttributeType.Resilience] * 0.5f);
    }
    
    private float CalculateMagicFind()
    {
        return attributes[AttributeType.Fortuity] * 2f;
    }
    
    // Get a derived stat - public accessor for other systems
    public float GetStat(StatType statType)
    {
        if (derivedStats.ContainsKey(statType))
            return derivedStats[statType];
            
        return 0f;
    }
    #endregion
    
    #region Health and Resource Management
    [Button("Take Damage")]
    public void TakeDamage(int amount)
    {
        if (playerCharacter == null)
            return;
            
        currentHealth = Mathf.Max(0, currentHealth - amount);
        playerCharacter.CurrentHealth = currentHealth;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"Character took {amount} damage. Health: {currentHealth}/{maxHealth}");
    }
    
    [Button("Heal")]
    public void Heal(int amount)
    {
        if (playerCharacter == null)
            return;
            
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        playerCharacter.CurrentHealth = currentHealth;
        
        Debug.Log($"Character healed {amount} health. Health: {currentHealth}/{maxHealth}");
    }
    
    [Button("Use Resource")]
    public void UseResource(int amount)
    {
        if (playerCharacter == null)
            return;
            
        currentResource = Mathf.Max(0, currentResource - amount);
        playerCharacter.CurrentResource = currentResource;
        
        Debug.Log($"Used {amount} resource. Remaining: {currentResource}/{maxResource}");
    }
    
    [Button("Restore Resource")]
    public void RestoreResource(int amount)
    {
        if (playerCharacter == null)
            return;
            
        currentResource = Mathf.Min(maxResource, currentResource + amount);
        playerCharacter.CurrentResource = currentResource;
        
        Debug.Log($"Restored {amount} resource. Resource: {currentResource}/{maxResource}");
    }
    
    private void Die()
    {
        Debug.Log("Character died!");
        
        if (playerCharacter.Level > 1)
        {
            int penalty = (int)(currentExperience * 0.1f); // 10% XP penalty
            currentExperience = Mathf.Max(0, currentExperience - penalty);
            playerCharacter.Experience = currentExperience;
            Debug.Log($"Lost {penalty} experience!");
        }
        
    }
    #endregion
    
    #region Subclass Management
    [Button("Select Subclass")]
    public void SelectSubclass(Subclass subclass)
    {
        if (playerCharacter == null || subclass == null)
            return;
            
        if (subclass.ParentClass.Type == playerCharacter.Class.Type)
        {
            playerCharacter.Subclass = subclass;
            RecalculateStats();
            Debug.Log($"Selected subclass: {subclass.Name}");
        }
    }
    #endregion
    
    #region Save/Load System Integration
    public void SaveCharacter()
    {
        SyncFromCharacter();
        
        // TODO implement save system
        Debug.Log("Character data saved");
    }
    
    public void LoadCharacter(Character character)
    {
        //TODO implement load system
        playerCharacter = character;
        SyncFromCharacter();
        Debug.Log("Character data loaded");
    }
    #endregion
    
    #region Test and Debug Methods
    [BoxGroup("Debug"), Button("Print Character Stats")]
    private void DebugPrintCharacterStats()
    {
        if (playerCharacter == null)
        {
            Debug.LogWarning("No character exists!");
            return;
        }
        
        Debug.Log($"=== Characters Big Ass Dick Stats ===");
        Debug.Log($"Class {playerCharacter.Class.Name}");
        Debug.Log($"Level {playerCharacter.Level}");
        Debug.Log($"Health {currentHealth}/{maxHealth}");
        Debug.Log($"Resource {currentResource}/{maxResource}");
        Debug.Log($"Experience {currentExperience}/{experienceForNextLevel}");
        
        Debug.Log("Attributes:");
        foreach (var attr in attributes)
        {
            Debug.Log($"  {attr.Key}: {attr.Value}");
        }
        
        Debug.Log("Derived Stats ");
        foreach (var stat in derivedStats)
        {
            Debug.Log($"  {stat.Key} | {stat.Value}");
        }
    }
    #endregion
}