using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationTime;
    [SerializeField] private float decelerationTime;
        
    // Component references
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipSpriteOnDirectionChange = true;
    
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string IS_MOVING = "IsMoving";
    

    [BoxGroup("Character Options")] [Required] [SerializeField]
    private CharacterManager manager;
    private Collider2D collider;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f; // Turn off gravity
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.freezeRotation = true;
        }

        if (manager != null)
        {
            manager = CharacterManager.Instance;
        }
    }
    
    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        movementDirection = new Vector2(horizontalInput, verticalInput).normalized;
        
        HandleSpriteDirection();
        
        UpdateAnimation();
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
        
        rb.linearVelocity = currentVelocity;
    }
    
    private void HandleSpriteDirection()
    {
        if (flipSpriteOnDirectionChange && spriteRenderer != null)
        {
            if (movementDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }


    private void TakeDamage( int damage)
    {
            manager.PlayerCharacter.CurrentHealth -= damage;
        
    }
    
    private void OnCollisionEnter2D(Collision other)
    {
        //spell shit here.
        
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat(HORIZONTAL, movementDirection.x);
            animator.SetFloat(VERTICAL, movementDirection.y);
            animator.SetBool(IS_MOVING, movementDirection.magnitude > 0.1f);
        }
    }
    
 

}