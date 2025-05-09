using UnityEngine;

public class VisualEffectSpawner : SpellEffect
{
    public override void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction)
    {
        if (spell.spellVisualPrefab == null) return;
        
        GameObject visual = Object.Instantiate(
            spell.spellVisualPrefab,
            position,
            Quaternion.identity
        );
        
        // Set lifetime based on spell type
        float lifetime = spell.duration > 0 ? spell.duration : 2f;
        Object.Destroy(visual, lifetime);
    }
}