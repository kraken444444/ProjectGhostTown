using UnityEngine;

public abstract class SpellEffect
{
    public abstract void Execute(Spell spell, ISpellCaster caster, Vector3 position, Vector3 direction);
}
