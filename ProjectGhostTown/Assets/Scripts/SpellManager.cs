using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum TargetType
{
    Self,
    Position, 
    Direction
}

public class SpellManager : SerializedMonoBehaviour
{
    [BoxGroup("References")]
    [Required, AssetsOnly]
    [PropertyTooltip("Reference to the player character")]
    public Character playerCharacter;
    
    // Cooldown tracking
    [OdinSerialize]
    [DictionaryDrawerSettings(KeyLabel = "Spell ID", ValueLabel = "Cooldown")]
    [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
    private Dictionary<string, float> _spellCooldowns = new Dictionary<string, float>();
    
    [BoxGroup("Targeting Settings")]
    [Required]
    public LayerMask enemyLayer;
    
    [BoxGroup("Targeting Settings")]
    [Required]
    public LayerMask obstacleLayers;
    
    // Spell library
    [BoxGroup("Spell Library"), AssetsOnly, ListDrawerSettings(ShowFoldout = true)]
    [PropertyTooltip("All available spells in the game")]
    [Searchable]
    public List<Spell> availableSpells = new List<Spell>();

    [BoxGroup("Active Spells"), ListDrawerSettings(ShowItemCount = true, DraggableItems = true)]
    [PropertyTooltip("Currently equipped spells")]
    [ValidateInput("ValidateActiveSpells", "Player doesn't meet requirements for some spells!")]
    [PreviewField(60, ObjectFieldAlignment.Left)]
    public List<Spell> activeSpells = new List<Spell>();
    
    private bool ValidateActiveSpells(List<Spell> spells, ref string errorMessage)
    {
        if (playerCharacter == null || spells == null)
            return true;
            
        foreach (var spell in spells)
        {
            if (spell.levelRequirement > playerCharacter.Level)
            {
                errorMessage = $"Spell '{spell.spellName}' requires level {spell.levelRequirement}";
                return false;
            }
            
            if (spell.classRequirement != playerCharacter.Class.Type)
            {
                errorMessage = $"Spell '{spell.spellName}' requires {spell.classRequirement} class";
                return false;
            }
        }
        
        return true;
    }
    
    [Button("Refresh Cooldowns"), GUIColor(0.3f, 0.8f, 0.3f)]
    private void RefreshCooldowns()
    {
        _spellCooldowns.Clear();
        Debug.Log("All spell cooldowns have been reset");
    }
    
    // A method to cast a spell with visual feedback in the editor
    [Button("Cast Selected Spell"), EnableIf("@UnityEngine.Application.isPlaying")]
    private void CastSelectedSpell(
        [ValueDropdown("activeSpells")] Spell spell,
        [SuffixLabel("(World Space)")] Vector2 targetPosition)
    {
        if (playerCharacter == null || spell == null)
            return;
            
    }
    
    public void CastSpell(Character caster, Vector2 targetPosition, Spell spell)
    {
        Vector2 casterPosition = caster.transform.position;
        
        float distanceToTarget = Vector2.Distance(casterPosition, targetPosition);
        
        if (spell.targetType != TargetType.Self)
        {
            Vector2 direction = targetPosition - casterPosition;
            RaycastHit2D hit = Physics2D.Raycast(casterPosition, direction, distanceToTarget, obstacleLayers);
        }
        
        // Cooldown implementation (commented out until you have a proper spellID system)
        // string cooldownKey = caster.ID + "_" + spell.spellID;
        // _spellCooldowns[cooldownKey] = spell.cooldown;
        
        caster.UseResource(spell.resourceCost);
        
        switch (spell.targetType)
        {
            case TargetType.Self:
                ApplySpellEffects(caster, caster, spell);
                
                if (spell.spellVisualPrefab != null)
                {
                    GameObject visual = Instantiate(
                        spell.spellVisualPrefab, 
                        caster.transform.position, 
                        Quaternion.identity
                    );
                    Destroy(visual, 2f);
                }
                break;
                
            case TargetType.Position:
                if (spell.isAOE)
                {
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                        targetPosition, 
                        spell.aoeRadius
                    );
                    
                    foreach (var collider in hitColliders)
                    {
                        Character target = collider.GetComponent<Character>();
                        if (target != null)
                        {
                            ApplySpellEffects(caster, target, spell);
                        }
                    }
                    
                    if (spell.spellVisualPrefab != null)
                    {
                        GameObject visual = Instantiate(
                            spell.spellVisualPrefab, 
                            new Vector3(targetPosition.x, targetPosition.y, 0), 
                            Quaternion.identity
                        );
                        Destroy(visual, 2f);
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(
                        casterPosition, 
                        targetPosition - casterPosition, 
                        spell.range
                    );
                    
                    if (hit.collider != null)
                    {
                        Character target = hit.collider.GetComponent<Character>();
                        if (target != null)
                        {
                            ApplySpellEffects(caster, target, spell);
                        }
                    }
                    
                    // For projectile spells
                    if (spell.projectilePrefab != null)
                    {
                        LaunchProjectile(caster, targetPosition, spell);
                    }
                    else if (spell.spellVisualPrefab != null)
                    {
                        GameObject visual = Instantiate(
                            spell.spellVisualPrefab, 
                            new Vector3(targetPosition.x, targetPosition.y, 0), 
                            Quaternion.identity
                        );
                        Destroy(visual, 2f);
                    }
                }
                break;
                
            case TargetType.Direction:
                Vector2 direction = (targetPosition - casterPosition).normalized;
                
                if (spell.projectilePrefab != null)
                {
                    LaunchProjectile(caster, casterPosition + direction * spell.range, spell);
                }
                else
                {
                    RaycastHit2D dirHit = Physics2D.Raycast(
                        casterPosition, 
                        direction, 
                        spell.range
                    );
                    
                    if (dirHit.collider != null)
                    {
                        Character target = dirHit.collider.GetComponent<Character>();
                        if (target != null)
                        {
                            ApplySpellEffects(caster, target, spell);
                        }
                        
                        if (spell.spellVisualPrefab != null)
                        {
                            GameObject visual = Instantiate(
                                spell.spellVisualPrefab, 
                                new Vector2(dirHit.point.x, dirHit.point.y), 
                                Quaternion.identity
                            );
                            Destroy(visual, 2f);
                        }
                    }
                }
                break;
        }
        
        if (spell.castSound != null)
        {
            AudioSource.PlayClipAtPoint(spell.castSound, new Vector2(casterPosition.x, casterPosition.y));
        }
    }
    
    private void ApplySpellEffects(Character caster, Character target, Spell spell)
    {
        if (spell.baseDamage > 0)
        {
            int damage = CalculateDamage(caster, spell);
            target.TakeDamage(damage);
        }
        
        // Add more spell effects here as needed
    }
    
    private void LaunchProjectile(Character caster, Vector2 targetPosition, Spell spell)
    {
        GameObject projectile = Instantiate(
            spell.projectilePrefab, 
            caster.transform.position, 
            Quaternion.identity
        );
        
        ProjectileController2D controller = projectile.GetComponent<ProjectileController2D>();
        if (controller != null)
        {
            controller.Initialize(caster, spell, targetPosition);
        }
        else
        {
            // Simple movement if no controller is present
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (targetPosition - (Vector2)caster.transform.position).normalized;
                rb.linearVelocity = direction * spell.projectileSpeed;
            }
            
            Destroy(projectile, spell.range / spell.projectileSpeed + 1f);
        }
    }
    
    private int CalculateDamage(Character caster, Spell spell)
    {
        // Use the Attributes system for damage calculation
        float offensiveMultiplier = 1 + (caster.Attributes[GameEnums.AttributeType.Offense] * 0.1f);
        return Mathf.RoundToInt(spell.baseDamage * offensiveMultiplier);
    }
    
    public float GetRemainingCooldown(Character caster, Spell spell)
    {
        // Cooldown implementation (commented out until you have a proper spellID system)
        // string cooldownKey = caster.ID + "_" + spell.spellID;
        // if (_spellCooldowns.ContainsKey(cooldownKey))
        // {
        //     return _spellCooldowns[cooldownKey];
        // }
        // return 0f;
        return 0f;
    }
}
