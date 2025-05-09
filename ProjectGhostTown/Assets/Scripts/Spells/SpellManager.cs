using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private LayerMask allyLayers;
    [SerializeField] private LayerMask obstacleLayers;
    
    // Dictionary to track cooldowns
    private Dictionary<string, float> spellCooldowns = new Dictionary<string, float>();
    
    // Cached spell effects
    private  DamageEffect damageEffect;
    private  HealingEffect healingEffect;
    private  StatusEffectApplier statusEffectApplier;
    private  ProjectileLauncher projectileLauncher;
    private  VisualEffectSpawner visualEffectSpawner;
    
    // Singleton pattern
    private static SpellManager _instance;
    public static SpellManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SpellManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SpellManager");
                    _instance = obj.AddComponent<SpellManager>();
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
        
        // Initialize effect instances
        damageEffect = new DamageEffect(enemyLayers);
        healingEffect = new HealingEffect(allyLayers);
        statusEffectApplier = new StatusEffectApplier(enemyLayers);
        projectileLauncher = new ProjectileLauncher();
        visualEffectSpawner = new VisualEffectSpawner();
    }

    void UpdateCooldowns()
    {
        var cooldownEntries = new Dictionary<string, float>(spellCooldowns);
        List<string> finishedCooldowns = new List<string>();
    
        // Work with the copy for updates
        foreach (var entry in cooldownEntries)
        {
            float newCooldown = spellCooldowns[entry.Key] - Time.deltaTime;
            spellCooldowns[entry.Key] = newCooldown;
        
            if (newCooldown <= 0)
            {
                finishedCooldowns.Add(entry.Key);
            }
        }
    
        // Remove finished cooldowns
        foreach (var key in finishedCooldowns)
        {
            spellCooldowns.Remove(key);
        }
    }
    
    private void Update()
    {
        // Update cooldowns
        UpdateCooldowns();
    }

    
    // Cast a spell
    public bool CastSpell(ISpellCaster caster, Vector3 targetPosition, Spell spell)
    {
        if (caster == null || spell == null)
            return false;
        
        // Check cooldown
        string cooldownKey = $"{caster.CasterID}_{spell.spellName}";
        if (spellCooldowns.ContainsKey(cooldownKey) && spellCooldowns[cooldownKey] > 0)
        {
            Debug.Log($"Spell {spell.spellName} is on cooldown: {spellCooldowns[cooldownKey]:F1}s remaining");
            return false;
        }
        
        // Consume resources
        caster.ConsumeResource(spell.resourceCost);
        
        // Set cooldown
        spellCooldowns[cooldownKey] = spell.cooldown;
        
        // Calculate direction for directional spells
        Vector3 direction = (targetPosition - caster.transform.position).normalized;
        
        // Execute spell effects based on type
        switch (spell.targetType)
        {
            case GameEnums.TargetType.Self:
                // Self-targeted effects
                if (spell.spellType == GameEnums.SpellType.Healing || spell.spellType == GameEnums.SpellType.Buff)
                {
                    healingEffect.Execute(spell, caster, caster.transform.position, direction);
                }
                
                if (spell.statusEffects.Count > 0)
                {
                    statusEffectApplier.Execute(spell, caster, caster.transform.position, direction);
                }
                
                visualEffectSpawner.Execute(spell, caster, caster.transform.position, direction);
                break;
                
            case GameEnums.TargetType.Position:
                // Position-targeted effects
                if (spell.projectilePrefab != null)
                {
                    // Use projectile
                    projectileLauncher.Execute(spell, caster, targetPosition, direction);
                }
                else
                {
                    // Direct effects
                    if (spell.spellType == GameEnums.SpellType.Attack || spell.spellType == GameEnums.SpellType.Debuff)
                    {
                        damageEffect.Execute(spell, caster, targetPosition, direction);
                    }
                    
                    if (spell.spellType == GameEnums.SpellType.Healing || spell.spellType == GameEnums.SpellType.Buff)
                    {
                        healingEffect.Execute(spell, caster, targetPosition, direction);
                    }
                    
                    if (spell.statusEffects.Count > 0)
                    {
                        statusEffectApplier.Execute(spell, caster, targetPosition, direction);
                    }
                    
                    visualEffectSpawner.Execute(spell, caster, targetPosition, direction);
                }
                break;
                
            case GameEnums.TargetType.Direction:
                // Direction-targeted effects
                if (spell.projectilePrefab != null)
                {
                    // Use projectile
                    projectileLauncher.Execute(spell, caster, targetPosition, direction);
                }
                else
                {
                    // Direct effects
                    if (spell.spellType == GameEnums.SpellType.Attack || spell.spellType == GameEnums.SpellType.Debuff)
                    {
                        damageEffect.Execute(spell, caster, targetPosition, direction);
                    }
                    
                    if (spell.statusEffects.Count > 0)
                    {
                        statusEffectApplier.Execute(spell, caster, targetPosition, direction);
                    }
                    
                    visualEffectSpawner.Execute(spell, caster, caster.transform.position + (direction * 2f), direction);
                }
                break;
        }
        
        // Play cast sound
        if (spell.castSound != null)
        {
            AudioSource.PlayClipAtPoint(spell.castSound, caster.transform.position, spell.soundVolume);
        }
        
        return true;
    }
    
    // Get remaining cooldown for a spell
    public float GetRemainingCooldown(ISpellCaster caster, Spell spell)
    {
        string cooldownKey = $"{caster.CasterID}_{spell.spellName}";
        if (spellCooldowns.ContainsKey(cooldownKey))
        {
            return spellCooldowns[cooldownKey];
        }
        return 0f;
    }
    
    // Reset cooldown for a spell
    public void ResetCooldown(ISpellCaster caster, Spell spell)
    {
        string cooldownKey = $"{caster.CasterID}_{spell.spellName}";
        if (spellCooldowns.ContainsKey(cooldownKey))
        {
            spellCooldowns.Remove(cooldownKey);
        }
    }
}