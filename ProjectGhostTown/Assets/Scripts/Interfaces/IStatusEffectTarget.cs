using UnityEngine;

public interface IStatusEffectTarget
{
    void ApplyStatusEffect(StatusEffectInfo effectInfo);
    void RemoveStatusEffect(string effectID);
    bool HasStatusEffect(string effectID);
}