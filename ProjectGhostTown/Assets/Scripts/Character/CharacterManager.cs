using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterManager : SerializedMonoBehaviour
{
    #region Singleton

    public static CharacterManager Instance { get; private set; }

    [BoxGroup("References")] [SerializeField, Required]
    private Character playerCharacter;

    private GameObject playerCharacterGameObject;

    [ShowInInspector, ReadOnly] public Character PlayerCharacter => playerCharacter;

    #endregion

    #region Character Creation & Management

    [BoxGroup("Character Creation")] [EnumToggleButtons] [OnValueChanged("OnClassTypeChanged")] [SerializeField]
    private GameEnums.ClassType selectedClassType = GameEnums.ClassType.Radiomancer;

    [BoxGroup("Character Creation")] [SerializeField]
    private int startingAttributePoints = 8;

    [BoxGroup("Experience")] [ProgressBar(0, "experienceForNextLevel")] [ShowInInspector]
    private int currentExperience = 0;

    [BoxGroup("Experience")] [ShowInInspector]
    private int experienceForNextLevel = 100;

    [BoxGroup("Stats")] [ShowInInspector]
    private Dictionary<GameEnums.StatType, float> derivedStats = new Dictionary<GameEnums.StatType, float>();

    [BoxGroup("Attributes")] [ShowInInspector]
    private Attributes attributes;

    [BoxGroup("Resources")] [ProgressBar(0, "maxHealth", r: 1, g: 0, b: 0)] [ShowInInspector]
    private int currentHealth;

    [BoxGroup("Resources")] [ShowInInspector]
    private int maxHealth;

    [BoxGroup("Resources")] [ProgressBar(0, "maxResource", r: 0, g: 0.5f, b: 1)] [ShowInInspector]
    private int currentResource;

    [BoxGroup("Resources")] [ShowInInspector]
    private int maxResource;

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

            attributes = new Attributes(playerCharacter.Attributes);

            attributes.OnAttributeChanged += OnAttributeChanged;

            derivedStats = new Dictionary<GameEnums.StatType, float>();
            foreach (GameEnums.StatType statType in System.Enum.GetValues(typeof(GameEnums.StatType)))
            {
                derivedStats[statType] = playerCharacter.GetStat(statType);
            }
        }
    }

    private void OnAttributeChanged(GameEnums.AttributeType type, int oldValue, int newValue)
    {
        if (playerCharacter != null)
        {
            playerCharacter.Attributes[type] = newValue;
            RecalculateStats();
        }

        Debug.Log($"Attribute {type} changed from {oldValue} to {newValue}");
    }

    #region Character Creation Methods

    [Button("Create New Character", ButtonSizes.Large), GUIColor(0, 0.8f, 0)]
    public CharacterClass CreateNewCharacter(GameEnums.ClassType classType)
    {
        CharacterClass characterClass = CharacterClass.CreateClass(classType);
        playerCharacter = new Character(characterClass);
        playerCharacter.ID = System.Guid.NewGuid().ToString();

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
        if (playerCharacter != null && attributes != null)
        {
            Dictionary<GameEnums.AttributeType, int> startingValues = new Dictionary<GameEnums.AttributeType, int>();

            foreach (GameEnums.AttributeType attributeType in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
            {
                startingValues[attributeType] = startingAttributePoints;
            }

            if (playerCharacter.Class != null)
            {
                foreach (var modifier in playerCharacter.Class.BaseAttributeModifiers)
                {
                    startingValues[modifier.Key] += modifier.Value;
                }
            }

            attributes = new Attributes(startingValues);

            foreach (var attr in startingValues)
            {
                playerCharacter.Attributes[attr.Key] = attr.Value;
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

        foreach (GameEnums.AttributeType type in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            if (attributes != null)
            {
                playerCharacter.Attributes[type] = attributes[type];
            }
        }

        playerCharacter.RecalculateStats();

        maxHealth = playerCharacter.MaxHealth;
        maxResource = playerCharacter.MaxResource;

        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentResource = Mathf.Min(currentResource, maxResource);

        playerCharacter.CurrentHealth = currentHealth;
        playerCharacter.CurrentResource = currentResource;

        foreach (GameEnums.StatType statType in System.Enum.GetValues(typeof(GameEnums.StatType)))
        {
            derivedStats[statType] = playerCharacter.GetStat(statType);
        }
    }

    public float GetStat(GameEnums.StatType statType)
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
            playerCharacter.SelectSubclass(subclass);
            // Update our local attributes copy with the subclass modifiers
            if (attributes != null && subclass.AttributeModifiers != null)
            {
                foreach (var modifier in subclass.AttributeModifiers)
                {
                    attributes[modifier.Key] = playerCharacter.Attributes[modifier.Key];
                }
            }

            Debug.Log($"Selected subclass: {subclass.Name}");
        }
    }

    #endregion

    #region Attribute Management

    [Button("Modify Attribute")]
    public void ModifyAttribute(GameEnums.AttributeType type, int amount)
    {
        if (playerCharacter == null || attributes == null)
            return;

        attributes.ModifyValue(type, amount);
        playerCharacter.Attributes[type] = attributes[type];

        RecalculateStats();

        Debug.Log($"Modified {type} by {amount}. New value: {attributes[type]}");
    }

    [Button("Get Attribute Value")]
    public int GetAttributeValue(GameEnums.AttributeType type)
    {
        if (attributes != null)
        {
            return attributes[type];
        }

        if (playerCharacter != null && playerCharacter.Attributes != null)
        {
            return playerCharacter.Attributes[type];
        }

        return 0;
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

        Debug.Log($"=== Character Stats ===");
        Debug.Log($"Class: {playerCharacter.Class.Name}");
        Debug.Log($"Level: {playerCharacter.Level}");
        Debug.Log($"Health: {currentHealth}/{maxHealth}");
        Debug.Log($"Resource: {currentResource}/{maxResource}");
        Debug.Log($"Experience: {currentExperience}/{experienceForNextLevel}");

        Debug.Log("Attributes:");
        foreach (GameEnums.AttributeType type in System.Enum.GetValues(typeof(GameEnums.AttributeType)))
        {
            Debug.Log($"  {type}: {playerCharacter.Attributes[type]}");
        }

        Debug.Log("Derived Stats:");
        foreach (var stat in derivedStats)
        {
            Debug.Log($"  {stat.Key}: {stat.Value}");
        }
    }
}

#endregion