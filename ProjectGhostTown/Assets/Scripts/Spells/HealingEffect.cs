using UnityEngine;

public class HealingEffect : SpellEffect
{
    private LayerMask allyLayers;
    private bool healSelf;
    
    public HealingEffect(LayerMask allyLayers, bool healSelf = true)
    {
        this.allyLayers = allyLayers;
        this.healSelf = healSelf;
    }
    
    public override void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction)
    {
        if (spell.baseHealing <= 0) return;
        
        // Calculate healing amount
        int healing = spell.CalculateHealing(caster as PlayerCharacter);
        
        // Heal self if applicable
        if (healSelf && spell.targetType == GameEnums.TargetType.Self)
        {
            var selfTarget = (caster as MonoBehaviour)?.GetComponent<IDamageable>();
            if (selfTarget != null)
            {
                selfTarget.Heal(healing);
            }
        }
        
        if (spell.isAOE)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, spell.aoeRadius, allyLayers);
            
            int targetsHealed = 0;
            foreach (var collider in colliders)
            {
                // Skip if max targets reached
                if (spell.maxTargets > 0 && targetsHealed >= spell.maxTargets) break;
                
                var target = collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.Heal(healing);
                    targetsHealed++;
                }
            }
        }
    }
}
