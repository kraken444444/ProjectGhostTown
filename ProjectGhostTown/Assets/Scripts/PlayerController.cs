using Sirenix.OdinInspector;
using UnityEngine;

using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, ITargetingSystem
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationTime = 0.1f;
    [SerializeField] private float decelerationTime = 0.2f;
        
    // Component references
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    
    [Header("Character Reference")]
    [SerializeField] private PlayerCharacter character;
    
    [Header("Spell Settings")]
    [SerializeField] private List<Spell> equippedSpells = new List<Spell>();
    [SerializeField] private int currentSpellIndex = 0;
    
    // Targeting system implementation
    private bool isTargeting;
    private Vector2 targetPosition;
    private Spell pendingSpell;
    
    [BoxGroup("Spell Options")]
    [SerializeField] private bool showSpellDebugInfo = true;
    
    private Camera mainCamera;
    
    // ITargetingSystem events
    public event System.Action<Vector2> OnTargetSelected;
    public event System.Action OnTargetingCancelled;
    
    // ITargetingSystem properties
    public bool IsTargeting => isTargeting;
    public Vector2 CurrentTargetPosition => targetPosition;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Try to get character component if not assigned
        if (character == null)
            character = GetComponent<PlayerCharacter>();
            
        mainCamera = Camera.main;
    }
    
    private void Start()
    {
        if (character == null)
        {
            Debug.LogError("PlayerController requires a PlayerCharacter component!");
            enabled = false;
            return;
        }
    }
    
    private void Update()
    {
        // Get movement input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Set movement direction
        movementDirection = new Vector2(horizontalInput, verticalInput).normalized;
        
        // Handle spell casting input
        HandleSpellInput();
        
        // Update targeting if active
        if (isTargeting && pendingSpell != null)
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            UpdateTargetingVisual(pendingSpell);
        }
    }
    
    private void FixedUpdate()
    {
        targetVelocity = movementDirection * moveSpeed;
        
        if (movementDirection != Vector2.zero)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, accelerationTime);
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, decelerationTime);
        }
        
        // Apply movement to rigidbody
        rb.linearVelocity = currentVelocity;
    }
    
    private void HandleSpellInput()
    {
        // Cancel targeting with right-click or escape
        if (isTargeting && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CancelTargeting();
            return;
        }
        
        // Cast currently selected spell with left click
        if (Input.GetMouseButtonDown(0))
        {
            if (isTargeting)
            {
                // Confirm target and cast spell
                Vector2 finalTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnTargetSelected?.Invoke(finalTarget);
                
                // Pass to SpellManager to handle casting
                SpellManager.Instance.CastSpell(character, finalTarget, pendingSpell);
                isTargeting = false;
                pendingSpell = null;
            }
            else if (currentSpellIndex >= 0 && currentSpellIndex < equippedSpells.Count)
            {
                // Start new spell cast
                CastSpellAtMouse(currentSpellIndex);
            }
        }
        
        // Number keys to select spells (1-9)
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < equippedSpells.Count)
            {
                currentSpellIndex = i;
                break;
            }
        }
        
        // Alternative: Scroll wheel to cycle through spells
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0 && equippedSpells.Count > 0)
        {
            currentSpellIndex += scrollDelta > 0 ? 1 : -1;
            if (currentSpellIndex < 0) currentSpellIndex = equippedSpells.Count - 1;
            if (currentSpellIndex >= equippedSpells.Count) currentSpellIndex = 0;
        }
    }
    
    private void CastSpellAtMouse(int spellIndex)
    {
        if (character == null || spellIndex < 0 || spellIndex >= equippedSpells.Count) return;
        
        Spell spell = equippedSpells[spellIndex];
        
        // Check if we need targeting for this spell
        if (spell.targetType == GameEnums.TargetType.Self)
        {
            // Self-targeting spells don't need a targeting phase
            SpellManager.Instance.CastSpell(character, transform.position, spell);
        }
        else
        {
            // Start targeting for position/direction based spells
            StartTargeting(spell);
        }
    }
    
    // ITargetingSystem implementation
    public void StartTargeting(Spell spell)
    {
        isTargeting = true;
        pendingSpell = spell;
        targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public void CancelTargeting()
    {
        isTargeting = false;
        pendingSpell = null;
        OnTargetingCancelled?.Invoke();
    }
    
    public void UpdateTargetingVisual(Spell spell)
    {
        // This would update any UI/visual elements for targeting
        // Could be implemented with line renderers or other visual feedback
    }
    
    // Optional: Add a method to visualize targeting when implementing a UI
    private void OnDrawGizmos()
    {
        if (!showSpellDebugInfo || !Application.isPlaying) return;
        
        // Draw current mouse target position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mousePos, 0.2f);
        
        // If targeting, draw appropriate visualization
        if (isTargeting && pendingSpell != null)
        {
            Gizmos.color = Color.yellow;
            
            if (pendingSpell.isAOE)
            {
                Gizmos.DrawWireSphere(mousePos, pendingSpell.aoeRadius);
            }
            
            // Draw line from player to target
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, mousePos);
        }
    }
}
