// BaseCharacter.cs - Updated version
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IDamageable, ISpellCaster, IStatusEffectTarget
{
    [Header("Identity")]
    [SerializeField] public string characterID;
    [SerializeField] public string characterName;
    
    [Header("Stats")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth;
    
    [Header("Attributes")]
    [SerializeField] protected Attributes attributes = new Attributes();
    
    // Events
    public event Action<int, int> OnHealthChanged;
    public event Action<DamageInfo> OnDamageTaken;
    public event Action<int> OnHealed;
    public event Action OnDeath;
    
    // Status tracking
    protected bool _isStunned;
    protected float _stunDuration;
    protected bool _isDead;
    
    // ISpellCaster implementation
    public string CasterID => characterID;
    public Attributes GetAttributes() => attributes;
    public new Transform transform => base.transform;
    
    // IDamageable implementation
    public bool IsDead() => _isDead;
    
    protected virtual void Awake()
    {
        // Generate ID if not set
        if (string.IsNullOrEmpty(characterID))
        {
            characterID = System.Guid.NewGuid().ToString();
        }
        
        // Set initial values
        if (currentHealth <= 0)
            currentHealth = maxHealth;
            
        // Initialize attributes if empty
        if (attributes == null)
        {
            attributes = new Attributes();
        }
    }
    
    protected virtual void Update()
    {
        // Update stun
        if (_isStunned && _stunDuration > 0)
        {
            _stunDuration -= Time.deltaTime;
            if (_stunDuration <= 0)
            {
                _isStunned = false;
            }
        }
    }
    
    public abstract void ConsumeResource(int amount);
    
    // IDamageable implementation
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        // Check if already dead
        if (_isDead)
            return;
            
        // Apply damage
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - damageInfo.Amount);
        
        // Debug output
        Debug.Log($"{characterName} took {damageInfo.Amount} damage. Health: {currentHealth}/{maxHealth}");
        
        // Trigger event
        OnHealthChanged?.Invoke(previousHealth, currentHealth);
        OnDamageTaken?.Invoke(damageInfo);
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        
        // Apply stun if applicable
        if (damageInfo.StunDuration > 0)
        {
            _isStunned = true;
            _stunDuration = damageInfo.StunDuration;
        }
    }
    
    public virtual void Heal(int amount)
    {
        // Check if already dead
        if (_isDead)
            return;
            
        // Apply healing
        int previousHealth = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
        // Debug output
        Debug.Log($"{characterName} healed for {amount}. Health: {currentHealth}/{maxHealth}");
        
        // Trigger event
        OnHealthChanged?.Invoke(previousHealth, currentHealth);
        OnHealed?.Invoke(amount);
    }
    
    protected virtual void Die()
    {
        if (_isDead)
            return;
            
        _isDead = true;
        
        // Debug output
        Debug.Log($"{characterName} has died!");
        
        // Trigger event
        OnDeath?.Invoke();
    }
    
    // IStatusEffectTarget implementation
    public void ApplyStatusEffect(StatusEffectInfo effectInfo)
    {
        StatusEffectManager.Instance.ApplyStatusEffect(gameObject, effectInfo);
    }
    
    public void RemoveStatusEffect(string effectID)
    {
        StatusEffectManager.Instance.RemoveStatusEffect(gameObject, effectID);
    }
    
    public bool HasStatusEffect(string effectName)
    {
        return StatusEffectManager.Instance.HasStatusEffect(gameObject, effectName);
    }
    
    public int GetAttributeValue(GameEnums.AttributeType attributeType)
    {
        return attributes != null ? attributes[attributeType] : 0;
    }

    public float GetStatValue(GameEnums.StatType statType)
    {
        return StatCalculator.CalculateStatValue(this, statType);
    }
}