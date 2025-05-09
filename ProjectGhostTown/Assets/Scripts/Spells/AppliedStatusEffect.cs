using UnityEngine;

[System.Serializable]
public class AppliedStatusEffect
{
    public BaseCharacter caster;
    public BaseCharacter target;
    public Spell.StatusEffect statusEffect;
    public float remainingDuration;
    public float baseValue;
    public float tickTime;
    public float timeSinceLastTick;
    public GameObject visualInstance;
}