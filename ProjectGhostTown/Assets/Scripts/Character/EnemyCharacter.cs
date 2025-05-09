// EnemyCharacter.cs

using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    [Header("Enemy Specifics")]
    [SerializeField] private int level = 1;
    
    [Header("Combat")]
    [SerializeField] private List<Spell> enemySpells = new List<Spell>();
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    
    // Private fields
    private float attackTimer;
    private Transform target;
    
    protected override void Awake()
    {
        base.Awake();
        attackTimer = 0f;
    }
    
     void Start()
    {
        
        // Find player
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    
    protected override void Update()
    {
        base.Update();
        
        if (IsDead() || target == null)
            return;
            
        // Update attack timer
        attackTimer -= Time.deltaTime;
        
        // Check if can attack
        if (attackTimer <= 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                // Attack
                AttackTarget();
                attackTimer = attackCooldown;
            }
        }
    }
    
    public void ConsumeResource(int amount)
    {
        // Enemies might not have a resource system
    }
    
    private void AttackTarget()
    {
        if (enemySpells.Count == 0)
            return;
            
        // Pick a random spell
        Spell spell = enemySpells[UnityEngine.Random.Range(0, enemySpells.Count)];
        
        // Cast the spell
        SpellManager.Instance.CastSpell(this, target.position, spell);
    }
    
    private void ApplyKnockback(DamageInfo damageInfo)
    {
        if (damageInfo.Source == null) return;
        
        Vector3 direction = (transform.position - damageInfo.Source.transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * damageInfo.KnockbackForce, ForceMode2D.Impulse);
        }
    }
    
    private void ApplyStun(float duration)
    {
        _isStunned = true;
        _stunDuration = duration;
    }
    
    protected override void Die()
    {
        base.Die();
        
        // Handle enemy-specific death behavior
        Debug.Log($"Enemy {characterName} has died!");
        
        // Drop loot
        DropLoot();
        
        // Destroy after delay for death animation
        Destroy(gameObject, 2f);
    }
    
    private void DropLoot()
    {
        // Implementation for dropping loot
    }
}
