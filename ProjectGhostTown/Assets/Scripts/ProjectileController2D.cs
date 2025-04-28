using UnityEngine;

public class ProjectileController2D : MonoBehaviour
{
    private Character _caster;
    private Spell _spell;
    private Vector2 _targetPosition;
    private float _speed;
    
    [SerializeField] private LayerMask targetableLayers;
    [SerializeField] private LayerMask obstacleLayers;

    public void Initialize(Character caster, Spell spell, Vector2 targetPosition)
    {
        _caster = caster;
        _spell = spell;
        _targetPosition = targetPosition;
        _speed = spell.projectileSpeed;
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;
        
        Vector2 direction = (_targetPosition - currentPosition).normalized;
        transform.position = new Vector3(
            currentPosition.x + direction.x * _speed * Time.deltaTime,
            currentPosition.y + direction.y * _speed * Time.deltaTime,
            transform.position.z
        );
        
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        // Check if we've reached the target
        float distanceToTarget = Vector2.Distance(currentPosition, _targetPosition);
        if (distanceToTarget < 0.1f)
        {
            OnProjectileHit();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a valid target
        Character target = other.GetComponent<Character>();
        if (target != null)
        {
            // Apply spell effects to the target
            if (_spell.baseDamage > 0)
            {
                int damage = CalculateDamage();
                target.TakeDamage(damage);
            }
            
            // Create impact effect
            if (_spell.spellVisualPrefab != null)
            {
                GameObject visual = Instantiate(_spell.spellVisualPrefab, transform.position, Quaternion.identity);
                Destroy(visual, 2f);
            }
            
            Destroy(gameObject);
        }
        else if ((obstacleLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            //hit an obstacle
            OnProjectileHit();
        }
    }
    
    private void OnProjectileHit()
    {
        if (_spell.spellVisualPrefab != null)
        {
            GameObject visual = Instantiate(_spell.spellVisualPrefab, transform.position, Quaternion.identity);
            Destroy(visual, 2f);
        }
        
        if (_spell.isAOE)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position, 
                _spell.aoeRadius, 
                targetableLayers
            );
            
            foreach (var collider in hitColliders)
            {
                Character target = collider.GetComponent<Character>();
                if (target != null)
                {
                    if (_spell.baseDamage > 0)
                    {
                        int damage = CalculateDamage();
                        target.TakeDamage(damage);
                    }
                    
                }
            }
        }
        
        Destroy(gameObject);
    }
    
    private int CalculateDamage()
    {
        // Use the new attributes system
        float offensiveMultiplier = 1 + (_caster.Attributes[GameEnums.AttributeType.Offense] * 0.1f);
        return Mathf.RoundToInt(_spell.baseDamage * offensiveMultiplier);
    }
}