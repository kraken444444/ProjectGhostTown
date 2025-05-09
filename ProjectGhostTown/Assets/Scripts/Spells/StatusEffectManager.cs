// Manages status effects application and tracking

using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    // Dictionary to track active status effects
    private Dictionary<GameObject, List<ActiveStatusEffect>> activeEffects = new Dictionary<GameObject, List<ActiveStatusEffect>>();
    
    // Singleton
    private static StatusEffectManager _instance;
    public static StatusEffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StatusEffectManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("StatusEffectManager");
                    _instance = obj.AddComponent<StatusEffectManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        // Update all active effects
        foreach (var entry in activeEffects)
        {
            GameObject target = entry.Key;
            List<ActiveStatusEffect> effects = entry.Value;
            
            // Skip if target is destroyed
            if (target == null) continue;
            
            // Track effects to remove
            List<ActiveStatusEffect> effectsToRemove = new List<ActiveStatusEffect>();
            
            // Update each effect
            foreach (var effect in effects)
            {
                // Update duration
                effect.remainingDuration -= Time.deltaTime;
                
                // Process ticks if applicable
                if (effect.tickInterval > 0)
                {
                    effect.timeSinceLastTick += Time.deltaTime;
                    if (effect.timeSinceLastTick >= effect.tickInterval)
                    {
                        effect.timeSinceLastTick = 0;
                        ProcessEffectTick(target, effect);
                    }
                }
                
                // Check if effect has expired
                if (effect.remainingDuration <= 0)
                {
                    effectsToRemove.Add(effect);
                    RemoveEffect(target, effect);
                }
            }
            
            // Remove expired effects
            foreach (var effect in effectsToRemove)
            {
                effects.Remove(effect);
            }
        }
        
        // Clean up entries with no effects
        List<GameObject> targetsToRemove = new List<GameObject>();
        foreach (var entry in activeEffects)
        {
            if (entry.Key == null || entry.Value.Count == 0)
            {
                targetsToRemove.Add(entry.Key);
            }
        }
        
        foreach (var target in targetsToRemove)
        {
            activeEffects.Remove(target);
        }
    }
    
    // Apply a status effect to a target
    public void ApplyStatusEffect(GameObject target, StatusEffectInfo effectInfo)
    {
        if (target == null) return;
        
        // Initialize effects list if needed
        if (!activeEffects.ContainsKey(target))
        {
            activeEffects[target] = new List<ActiveStatusEffect>();
        }
        
        // Check for existing effect of the same type
        bool effectExists = false;
        foreach (var effect in activeEffects[target])
        {
            if (effect.name == effectInfo.Name)
            {
                // Refresh the duration
                effect.remainingDuration = effectInfo.Duration;
                effectExists = true;
                break;
            }
        }
        
        // Add new effect if it doesn't exist
        if (!effectExists)
        {
            // Create visual instance if applicable
            GameObject visualInstance = null;
            if (effectInfo.VisualPrefab != null)
            {
                visualInstance = Instantiate(effectInfo.VisualPrefab, target.transform.position, Quaternion.identity, target.transform);
            }
            
            // Create and add the effect
            ActiveStatusEffect newEffect = new ActiveStatusEffect
            {
                id = effectInfo.ID,
                name = effectInfo.Name,
                type = effectInfo.Type,
                remainingDuration = effectInfo.Duration,
                potency = effectInfo.Potency,
                baseValue = effectInfo.BaseValue,
                source = effectInfo.Source,
                tickInterval = effectInfo.TickInterval,
                timeSinceLastTick = 0,
                visualInstance = visualInstance
            };
            
            activeEffects[target].Add(newEffect);
            
            // Apply immediate effects
            ApplyEffectImmediateEffects(target, newEffect);
        }
    }
    
    // Remove a status effect
    public void RemoveStatusEffect(GameObject target, string effectID)
    {
        if (target == null || !activeEffects.ContainsKey(target)) return;
        
        ActiveStatusEffect effectToRemove = null;
        foreach (var effect in activeEffects[target])
        {
            if (effect.id == effectID)
            {
                effectToRemove = effect;
                break;
            }
        }
        
        if (effectToRemove != null)
        {
            RemoveEffect(target, effectToRemove);
            activeEffects[target].Remove(effectToRemove);
        }
    }
    
    // Check if target has a status effect
    public bool HasStatusEffect(GameObject target, string effectName)
    {
        if (target == null || !activeEffects.ContainsKey(target)) return false;
        
        foreach (var effect in activeEffects[target])
        {
            if (effect.name == effectName)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Get all status effects on a target
    public List<ActiveStatusEffect> GetStatusEffects(GameObject target)
    {
        if (target == null || !activeEffects.ContainsKey(target))
            return new List<ActiveStatusEffect>();
            
        return new List<ActiveStatusEffect>(activeEffects[target]);
    }
    
    // Process effect tick (e.g. damage over time)
    private void ProcessEffectTick(GameObject target, ActiveStatusEffect effect)
    {
        switch (effect.type)
        {
            case GameEnums.StatusEffectType.DoT_Damage:
                // Apply damage tick
                var damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = Mathf.RoundToInt(effect.baseValue * effect.potency);
                    damageable.TakeDamage(new DamageInfo(effect.source, damage, GameEnums.DamageType.True));
                }
                break;
                
            case GameEnums.StatusEffectType.HoT_Healing:
                // Apply healing tick
                var healable = target.GetComponent<IDamageable>();
                if (healable != null)
                {
                    int healing = Mathf.RoundToInt(effect.baseValue * effect.potency);
                    healable.Heal(healing);
                }
                break;
                
            // Handle other effect types
        }
    }
    
    // Apply immediate effects when an effect is first applied
    private void ApplyEffectImmediateEffects(GameObject target, ActiveStatusEffect effect)
    {
        // Handle initial application of buffs/debuffs
        switch (effect.type)
        {
            case GameEnums.StatusEffectType.Buff_Speed:
          
                break;
                
            case GameEnums.StatusEffectType.Debuff_Defense:
                break;
            case GameEnums.StatusEffectType.HoT_Healing:
                break;
            case GameEnums.StatusEffectType.DoT_Damage:
                break;
                
        }
    }
    
    // Clean up when removing an effect
    private void RemoveEffect(GameObject target, ActiveStatusEffect effect)
    {
        // Clean up visual instance
        if (effect.visualInstance != null)
        {
            Destroy(effect.visualInstance);
        }
        
        // Revert any stat changes
        switch (effect.type)
        {
            case GameEnums.StatusEffectType.Buff_Speed:
                // Example: restore normal movement speed
                var movement = target.GetComponent<PlayerController>();
                if (movement != null)
                {
                    
                }
                break;
                
            case GameEnums.StatusEffectType.Debuff_Defense:
                // Example: restore normal defense
                break;
                
            // Handle other effect types
        }
    }
    
    // Class to track active status effects
    [System.Serializable]
    public class ActiveStatusEffect
    {
        public string id;
        public string name;
        public GameEnums.StatusEffectType type;
        public float remainingDuration;
        public float potency;
        public int baseValue;
        public ISpellCaster source;
        public float tickInterval;
        public float timeSinceLastTick;
        public GameObject visualInstance;
    }
}