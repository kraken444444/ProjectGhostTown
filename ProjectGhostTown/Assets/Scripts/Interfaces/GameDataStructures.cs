using UnityEngine;
public struct DamageInfo
{
    public ISpellCaster Source { get; }
    public int Amount { get; }
    public GameEnums.DamageType DamageType { get; }
    public bool IsCritical { get; }
    public float KnockbackForce { get; }
    public float StunDuration { get; }

    public DamageInfo(
        ISpellCaster source, 
        int amount, 
        GameEnums.DamageType damageType, 
        bool isCritical = false,
        float knockbackForce = 0f,
        float stunDuration = 0f)
    {
        Source = source;
        Amount = amount;
        DamageType = damageType;
        IsCritical = isCritical;
        KnockbackForce = knockbackForce;
        StunDuration = stunDuration;
    }
}

// Status effect information
public struct StatusEffectInfo
{
    public string ID { get; }
    public string Name { get; }
    public GameEnums.StatusEffectType Type { get; }
    public float Duration { get; }
    public float TickInterval { get; }
    public float Potency { get; }
    public int BaseValue { get; }
    public ISpellCaster Source { get; }
    public GameObject VisualPrefab { get; }

    public StatusEffectInfo(
        string name,
        GameEnums.StatusEffectType type,
        float duration,
        float potency,
        ISpellCaster source,
        int baseValue = 0,
        float tickInterval = 0f,
        GameObject visualPrefab = null)
    {
        ID = System.Guid.NewGuid().ToString();
        Name = name;
        Type = type;
        Duration = duration;
        Potency = potency;
        Source = source;
        BaseValue = baseValue;
        TickInterval = tickInterval;
        VisualPrefab = visualPrefab;
    }
}