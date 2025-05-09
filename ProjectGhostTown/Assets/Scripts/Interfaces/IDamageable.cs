using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
    void Heal(int amount);
    bool IsDead();
    Transform transform { get; } // For effect positioning
}