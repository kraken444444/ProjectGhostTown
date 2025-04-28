using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : SerializedMonoBehaviour
{
    [BoxGroup("Stats"), PropertyRange(1, 1000)]
    [ProgressBar(0, "maxHealth", 1, 0, 0, Height = 20)]
    [OnValueChanged("ValidateCurrentHealth")]
    public int currentHealth;
    
    [BoxGroup("Stats"), PropertyRange(1, 1000)]
    public int maxHealth = 100;
    
    [BoxGroup("Attributes")]
    [ShowInInspector]
    public Attributes Attributes { get; private set; }
    
    [BoxGroup("Derived Stats")]
    [ShowInInspector, ReadOnly]
    private Dictionary<GameEnums.StatType, float> derivedStats = new Dictionary<GameEnums.StatType, float>();
    
    [BoxGroup("Drops")]
    [TableList(ShowIndexLabels = true)]
    // public List<EnemyDrop> potentialDrops = new List<EnemyDrop>();
    
    [BoxGroup("Level")]
    [ShowInInspector]
    public int Level { get; private set; } = 1;
    
    private void Awake()
    {
        Attributes = new Attributes();
        
        // Subscribe to attribute changes
        Attributes.OnAttributeChanged += OnAttributeChanged;
        
        InitializeAttributes();
        
        RecalculateStats();
    }
    
    private void InitializeAttributes()
    {
        Attributes[GameEnums.AttributeType.Offense] = Level * 2;
        Attributes[GameEnums.AttributeType.Resilience] = Level * 3;
        Attributes[GameEnums.AttributeType.Tenacity] = Level;
        Attributes[GameEnums.AttributeType.Expertise] = Level;
        Attributes[GameEnums.AttributeType.Fortuity] = Level;
    }
    
    private void OnAttributeChanged(GameEnums.AttributeType type, int oldValue, int newValue)
    {
        RecalculateStats();
    }
    
    private void RecalculateStats()
    {
        maxHealth = 50 + (Level * 10) + (Attributes[GameEnums.AttributeType.Resilience] * 5);
        
        derivedStats[GameEnums.StatType.PhysicalDamage] = 5f + (Attributes[GameEnums.AttributeType.Offense] * 1.2f);
        derivedStats[GameEnums.StatType.MagicalDamage] = 2f + (Attributes[GameEnums.AttributeType.Offense] * 0.8f);
        derivedStats[GameEnums.StatType.CriticalChance] = 3f + (Attributes[GameEnums.AttributeType.Expertise] * 0.3f);
        derivedStats[GameEnums.StatType.DamageReduction] = Mathf.Min(50f, Attributes[GameEnums.AttributeType.Resilience] * 0.3f);
        
        ValidateCurrentHealth();
    }
    
    private void ValidateCurrentHealth()
    {
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    
    [Button("Test Damage")]
    public void TestDamage(
        [Sirenix.OdinInspector.PropertyRange(1, 10000)] int damageAmount)
    {
        TakeDamage(damageAmount);
    }
    
    public void TakeDamage(int amount)
    {
        float damageReduction = derivedStats[GameEnums.StatType.DamageReduction] / 100f;
        int reducedDamage = Mathf.RoundToInt(amount * (1f - damageReduction));
        
        currentHealth -= reducedDamage;
        
        Debug.Log($"Enemy took {reducedDamage} damage (reduced from {amount} by {damageReduction*100}% DR)");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public float GetStat(GameEnums.StatType statType)
    {
        if (derivedStats.ContainsKey(statType))
            return derivedStats[statType];
            
        return 0f;
    }
    
    private void Die()
    {
        Debug.Log("Enemy died!");
        
        // TODO: Generate drops based on player's Fortuity attribute
        // GenerateDrops();
        
        Destroy(gameObject);
    }
    
    /*
    private void GenerateDrops()
    {
    }
    */
    
    public void SetLevel(int level)
    {
        Level = Mathf.Max(1, level);
        InitializeAttributes();
        RecalculateStats();
        currentHealth = maxHealth; 
    }
}