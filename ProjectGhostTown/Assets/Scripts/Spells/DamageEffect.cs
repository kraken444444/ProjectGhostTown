using UnityEngine;

public class DamageEffect : SpellEffect
{
    private LayerMask targetLayers;

    public DamageEffect(LayerMask targetLayers)
    {
        this.targetLayers = targetLayers;
    }

    public override void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction)
    {
        if (spell.baseDamage <= 0) return;

        // Process based on targeting type
        if (spell.isAOE)
        {
            // AoE damage
            float radius = spell.aoeRadius;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, targetLayers);
            
            int targetsHit = 0;
            foreach (var collider in colliders)
            {
                // Skip if max targets reached
                if (spell.maxTargets > 0 && targetsHit >= spell.maxTargets) break;
                
                var target = collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    ApplyDamage(spell, caster, target);
                    targetsHit++;
                }
            }
        }
        else if (spell.targetType == GameEnums.TargetType.Direction)
        {
            // Raycast damage
            RaycastHit2D hit = Physics2D.Raycast(caster.transform.position, direction, spell.range, targetLayers);
            if (hit.collider != null)
            {
                var target = hit.collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    ApplyDamage(spell, caster, target);
                }
            }
        }
    }

    private void ApplyDamage(Spell spell, ISpellCaster caster, IDamageable target)
    {
        // Calculate base damage from spell
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
}
