using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : SerializedMonoBehaviour
{
    [BoxGroup("Stats"), PropertyRange(1, 1000)]
    [ProgressBar(0, "maxHealth", 1, 0, 0, Height = 20)]
    [OnValueChanged("ValidateCurrentHealth")]
    public int currentHealth;
    
    [BoxGroup("Stats"), PropertyRange(1, 1000)]
    public int maxHealth = 100;
    
    [BoxGroup("Drops")]
    [TableList(ShowIndexLabels = true)]
  //  public List<EnemyDrop> potentialDrops = new List<EnemyDrop>();
    
    private void ValidateCurrentHealth()
    {
        // Ensure current health never exceeds max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    
    [Button("Test Damage")]
    public void TestDamage(
        [Sirenix.OdinInspector.PropertyRange(1, 10000)] int damageAmount)
    {
        TakeDamage(damageAmount);
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}