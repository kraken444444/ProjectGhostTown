using UnityEngine;

public class StatusEffectApplier : SpellEffect
{
    private LayerMask targetLayers;
    
    public StatusEffectApplier(LayerMask targetLayers)
    {
        this.targetLayers = targetLayers;
    }
    
    public override void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction)
    {
        if (spell.statusEffects == null || spell.statusEffects.Count == 0) return;
        
        // Process based on targeting type
        if (spell.targetType == GameEnums.TargetType.Self)
        {
            // Apply to self
            var selfTarget = (caster as MonoBehaviour)?.GetComponent<IStatusEffectTarget>();
            if (selfTarget != null)
            {
                foreach (var effect in spell.statusEffects)
                {
                    ApplyStatusEffect(selfTarget, effect, spell, caster);
                }
            }
        }
        else if (spell.isAOE)
        {
            // Apply to targets in area
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, spell.aoeRadius, targetLayers);
            
            int targetsAffected = 0;
            foreach (var collider in colliders)
            {
                // Skip if max targets reached
                if (spell.maxTargets > 0 && targetsAffected >= spell.maxTargets) break;
                
                var target = collider.GetComponent<IStatusEffectTarget>();
                if (target != null)
                {
                    foreach (var effect in spell.statusEffects)
                    {
                        ApplyStatusEffect(target, effect, spell, caster);
                    }
                    targetsAffected++;
                }
            }
        }
        else if (spell.targetType == GameEnums.TargetType.Direction)
        {
            // Apply to target hit by raycast
            RaycastHit2D hit = Physics2D.Raycast(caster.transform.position, direction, spell.range, targetLayers);
            if (hit.collider != null)
            {
                var target = hit.collider.GetComponent<IStatusEffectTarget>();
                if (target != null)
                {
                    foreach (var effect in spell.statusEffects)
                    {
                        ApplyStatusEffect(target, effect, spell, caster);
                    }
                }
            }
        }
    }
    
    private void ApplyStatusEffect(IStatusEffectTarget target, Spell.StatusEffect effect, Spell spell, ISpellCaster caster)
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
        
        target.ApplyStatusEffect(effectInfo);
    }
}
