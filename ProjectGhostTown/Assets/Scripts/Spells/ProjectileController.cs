using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float hitboxRadius = 0.25f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private LayerMask obstacleLayers;
    
    private Spell spell;
    private ISpellCaster caster;
    private Vector2 direction;
    private float speed;
    private float distanceTraveled = 0f;
    
    public void Initialize(Spell spell, ISpellCaster caster, Vector2 direction)
    {
        this.spell = spell;
        this.caster = caster;
        this.direction = direction.normalized;
        this.speed = spell.projectileSpeed;
        
        // Set rotation based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void Update()
    {
        if (spell == null || caster == null) return;
        
        // Move projectile
        Vector2 movement = direction * speed * Time.deltaTime;
        transform.position += new Vector3(movement.x, movement.y, 0);
        
        // Track distance and destroy if exceeded range
        distanceTraveled += movement.magnitude;
        if (distanceTraveled >= spell.range)
        {
            OnReachedMaxDistance();
            return;
        }
        
        // Check for collisions
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, hitboxRadius, targetLayers | obstacleLayers);
        foreach (var collider in colliders)
        {
            // Skip the caster's own collider
            if (collider.transform == caster.transform) continue;
            
            // Check if we hit a target
            var target = collider.GetComponent<IDamageable>();
            if (target != null)
            {
                OnHitTarget(target);
                return;
            }
            
            // Check if we hit an obstacle
            if ((obstacleLayers.value & (1 << collider.gameObject.layer)) != 0)
            {
                OnHitObstacle();
                return;
            }
        }
    }
    
    private void OnHitTarget(IDamageable target)
    {
        // Apply damage
        if (spell.baseDamage > 0)
        {
            int damage = spell.CalculateDamage(caster as BaseCharacter);
            
            // Calculate critical hit
            bool isCritical = false;
            if (spell.canCrit)
            {
                var attributes = caster.GetAttributes();
                float critChance = 5f + attributes[GameEnums.AttributeType.Expertise] * 0.5f + spell.criticalChanceBonus;
                isCritical = Random.Range(0f, 100f) <= critChance;
            }
            
            // Apply damage
            DamageInfo damageInfo = new DamageInfo(
                caster, 
                damage, 
                spell.damageType, 
                isCritical,
                spell.knockbackForce,
                spell.stunDuration
            );
            
            target.TakeDamage(damageInfo);
        }
        
        // Apply status effects
        var statusTarget = target as IStatusEffectTarget;
        if (statusTarget != null && spell.statusEffects.Count > 0)
        {
            foreach (var effect in spell.statusEffects)
            {
                int baseValue = effect.type == GameEnums.StatusEffectType.DoT_Damage 
                    ? spell.baseDamage 
                    : (effect.type == GameEnums.StatusEffectType.HoT_Healing ? spell.baseHealing : 0);
                    
                StatusEffectInfo effectInfo = new StatusEffectInfo(
                    effect.effectName,
                    effect.type,
                    effect.duration,
                    effect.potency,
                    caster,
                    baseValue,
                    spell.tickRate > 0 ? spell.tickRate : 1.0f,
                    effect.effectVisualPrefab
                );
                
                statusTarget.ApplyStatusEffect(effectInfo);
            }
        }
        
        // Apply AoE effects if applicable
        if (spell.isAOE)
        {
            ApplyAreaEffects(transform.position);
        }
        
        // Play impact sound
        if (spell.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(spell.impactSound, transform.position, spell.soundVolume);
        }
        
        // Spawn impact visual
        SpawnImpactVisual(transform.position);
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    private void OnHitObstacle()
    {
        // Play impact sound
        if (spell.impactSound != null)
        {
            AudioSource.PlayClipAtPoint(spell.impactSound, transform.position, spell.soundVolume);
        }
        
        // Spawn impact visual
        SpawnImpactVisual(transform.position);
        
        // Apply AoE effects if applicable
        if (spell.isAOE)
        {
            ApplyAreaEffects(transform.position);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    private void OnReachedMaxDistance()
    {
        // Apply AoE effects if applicable
        if (spell.isAOE)
        {
            ApplyAreaEffects(transform.position);
        }
        
        // Spawn impact visual
        SpawnImpactVisual(transform.position);
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    private void ApplyAreaEffects(Vector2 center)
    {
        // Find targets in area
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, spell.aoeRadius, targetLayers);
        
        int targetsAffected = 0;
        foreach (var collider in colliders)
        {
            // Skip if max targets reached
            if (spell.maxTargets > 0 && targetsAffected >= spell.maxTargets) break;
            
            // Skip the caster's own collider
            if (collider.transform == caster.transform) continue;
            
            // Apply damage
            var target = collider.GetComponent<IDamageable>();
            if (target != null)
            {
                int damage = spell.CalculateDamage(caster as BaseCharacter);
                
                // Calculate critical hit
                bool isCritical = false;
                if (spell.canCrit)
                {
                    var attributes = caster.GetAttributes();
                    float critChance = 5f + attributes[GameEnums.AttributeType.Expertise] * 0.5f + spell.criticalChanceBonus;
                    isCritical = Random.Range(0f, 100f) <= critChance;
                }
                
                DamageInfo damageInfo = new DamageInfo(
                    caster, 
                    damage, 
                    spell.damageType, 
                    isCritical,
                    spell.knockbackForce,
                    spell.stunDuration
                );
                
                target.TakeDamage(damageInfo);
                targetsAffected++;
            }
            
            // Apply status effects
            var statusTarget = collider.GetComponent<IStatusEffectTarget>();
            if (statusTarget != null && spell.statusEffects.Count > 0)
            {
                foreach (var effect in spell.statusEffects)
                {
                    int baseValue = effect.type == GameEnums.StatusEffectType.DoT_Damage 
                        ? spell.baseDamage 
                        : (effect.type == GameEnums.StatusEffectType.HoT_Healing ? spell.baseHealing : 0);
                        
                    StatusEffectInfo effectInfo = new StatusEffectInfo(
                        effect.effectName,
                        effect.type,
                        effect.duration,
                        effect.potency,
                        caster,
                        baseValue,
                        spell.tickRate > 0 ? spell.tickRate : 1.0f,
                        effect.effectVisualPrefab
                    );
                    
                    statusTarget.ApplyStatusEffect(effectInfo);
                }
            }
        }
    }
    
    private void SpawnImpactVisual(Vector2 position)
    {
        if (spell.spellVisualPrefab != null)
        {
            GameObject visual = Instantiate(spell.spellVisualPrefab, position, Quaternion.identity);
            Destroy(visual, 2f);
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
        
        // Draw AOE radius if applicable
        if (spell != null && spell.isAOE)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spell.aoeRadius);
        }
    }
}
