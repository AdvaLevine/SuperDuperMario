
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float jumpForce = 4f; 
    [SerializeField] private LayerMask groundLayer; //the layer the player can jump from
    [SerializeField] private float jumpGravityScale = 0.5f; // Lower gravity during jump

    [Header("Score Settings")]
    [SerializeField] private int pointMultiplier = 20; // Points per distance traveled
    private Vector3 lastPosition;
    private float distanceTraveled = 0f;
    private float timer = 0f;
    private float scoreUpdateInterval = 1f; // Update score every 1 seconds

    // Animation settings
    private bool facingRight = true; //check if the player is facing right
    protected float moveInput; //the input for the movement
    protected bool jumpInput; //the input for the jump
    protected bool isGrounded; //check if the player is on the ground

    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound; 
    private AudioSource audioSource;
    private bool isJumpMuted = false; 

    private Rigidbody2D _rb;    
    
    // Power-up settings
    private float jumpForceMultiplier = 1f;
    private float coinMultiplier = 5f;

    // Coroutines for power-ups
    private Coroutine jumpForceCoroutine;
    private Coroutine coinMultiplierCoroutine;
    
    // Animation settings
    private Animator _animator;
    
    // Flag to control player movement
    private bool canMove = true; 

    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _animator = gameObject.GetComponent<Animator>();
        lastPosition = transform.position; // Initialize lastPosition
    }

    protected virtual void Update()
    {
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>(); 
        audioSource.volume = 0.3f;
        lastPosition = transform.position;
    }
    
    private void FixedUpdate()
    {
        if (!canMove)
        {
            _rb.velocity = Vector2.zero;
            _rb.constraints = RigidbodyConstraints2D.FreezePosition;
            return;
        } 
        
        Move();
        
        if (jumpInput)
        {
            Jump();
            jumpInput = false;
        }
        
        if (_rb.velocity.y <= 0) 
        {
            _rb.gravityScale = 1;
        }
    }
    
    private void Move()
    {
        _rb.velocity = new Vector2(moveInput * moveSpeed, _rb.velocity.y);
        HandleWalkingAnimations();
       
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private void Jump()
    {
        PlayJumpSound();
        HandleJumpAnimations();
        _rb.gravityScale = jumpGravityScale;
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce * jumpForceMultiplier);
    }

    protected void UpdatePlayer(int playerID, int totalPlayers)
    {
        string horizontalAxis;
        string jumpButton;

        // Check if it's single player or multiplayer
        if (totalPlayers == 1)
        {
            // Use default controls for single player
            horizontalAxis = "Horizontal"; // Assuming default input axes
            jumpButton = "Jump";           // Assuming default jump button
        }
        else
        {
            // Use player-specific controls for multiplayer
            horizontalAxis = "Player" + playerID + "_Horizontal";
            jumpButton = "Player" + playerID + "_Jump";
        }
        
        moveInput = Input.GetAxisRaw(horizontalAxis);
        isGrounded = IsGrounded();
    
        if (Input.GetButtonDown(jumpButton) && isGrounded)
        {
            jumpInput = true;
        }
    
        if (_animator.GetBool("IsJumping") && isGrounded)
        {
            _animator.SetBool("IsJumping", false);
        }
        
        float distanceX = transform.position.x - lastPosition.x;
    
        if (distanceX > 0) // Only add distance if moving forward
        {
            distanceTraveled += distanceX;
            lastPosition = transform.position;
        }

        // Timer to update score every interval
        timer += Time.deltaTime;

        if (timer >= scoreUpdateInterval)
        {
            CalculateAndAddScore();
            timer = 0f; // Reset timer
        }
    }

    // Method to set canMove flag
    public void SetCanMove(bool value)
    {
        canMove = value;
        
        if (!canMove)
        {
            _rb.velocity = Vector2.zero; // Stop any current movement
            _rb.constraints = RigidbodyConstraints2D.FreezePosition; // Freeze the player in place
        }
    }
    
    public void SetJumpForceMultiplier(float multiplier, float duration = 5f)
    {
        if (jumpForceCoroutine != null)
        {
            StopCoroutine(jumpForceCoroutine);
        }
        jumpForceMultiplier = multiplier;
        jumpForceCoroutine = StartCoroutine(ResetJumpForceMultiplierAfterTime(duration));
    }

    private IEnumerator ResetJumpForceMultiplierAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpForceMultiplier = 1f;
    }

    public void SetCoinMultiplier(float multiplier, float duration)
    {
        if (coinMultiplierCoroutine != null)
        {
            StopCoroutine(coinMultiplierCoroutine);
        }
        coinMultiplier = multiplier;
        coinMultiplierCoroutine = StartCoroutine(ResetCoinMultiplierAfterTime(duration));
    }

    private IEnumerator ResetCoinMultiplierAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        coinMultiplier = 1f;
    }

    public float GetCoinMultiplier()
    {
        return coinMultiplier;
    }
    
    public void ToggleJumpMute()
    {
        isJumpMuted = !isJumpMuted;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void PlayJumpSound()
    {
        if (jumpSound != null && !isJumpMuted) 
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
    
    private void HandleWalkingAnimations()
    {
        if (moveInput != 0)
        {
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
    }
    
    private void HandleJumpAnimations()
    {
        if (_animator.GetBool("IsWalking"))
        {
            _animator.SetBool("IsJumping", false);
        }
        
        _animator.SetBool("IsJumping", true);
    }
    
    private void CalculateAndAddScore()
    {
        // Calculate the score based on the distance traveled
        int points = (int)(distanceTraveled * pointMultiplier);
        // Add the score to the score manager
        ScoreManager.Instance.AddScore(points);
        distanceTraveled = 0f; // Reset distance traveled

    }
    
    private bool IsGrounded()
    {
        float rayLength = 0.2f;
    
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
    
    public void Bounce()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce * 0.5f);
    }

    public void Die()
    {
        if (GameManager.Instance.HasPlayerWon()) return;
        enabled = false;
        GameManager.Instance.GameOver();
    }
}
