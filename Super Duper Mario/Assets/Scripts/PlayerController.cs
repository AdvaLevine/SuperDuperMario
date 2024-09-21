
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private GameObject[] _playerPrefab;
    public int CharacterSelectionIndex { get; set; } = 0;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float jumpForce = 6f; 
    [SerializeField] private LayerMask groundLayer; //the layer the player can jump from
    [SerializeField] private float jumpGravityScale = 0.5f; // Lower gravity during jump

    
    // Score settings
    [Header("Score Settings")]
    [SerializeField] private int pointMultiplier = 20; // Points per distance traveled
    private Vector3 lastPosition;
    private float distanceTraveled = 0f;
    private float timer = 0f;
    private float scoreUpdateInterval = 1f; // Update score every 1 seconds

    // Animation settings
    private bool facingRight = true; //check if the player is facing right
    private float moveInput; //the input for the movement
    private bool jumpInput; //the input for the jump
    private bool isGrounded; //check if the player is on the ground

    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound; 
    private AudioSource audioSource;
    private bool isJumpMuted = false; 

    private Vector3 _initialPosition = new Vector3(-15f, 0, 0); 
    private Rigidbody2D _rb;    
    GameObject _player;     
    
    private float jumpForceMultiplier = 1f;
    private float coinMultiplier = 5f;

    private Coroutine jumpForceCoroutine;
    private Coroutine coinMultiplierCoroutine;
    
    private Animator _animator;
   

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

    private void Jump()
    {
        PlayJumpSound();
        HandleJumpAnimations();
        _rb.gravityScale = jumpGravityScale;
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce * jumpForceMultiplier);

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

    private void Awake()
    {
        _player = Instantiate(_playerPrefab[CharacterSelectionIndex], _initialPosition , Quaternion.identity);

        _rb = _player.GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = _player.AddComponent<Rigidbody2D>();
        }
    }
    
    public void OnMarioImageClick()
    {
        SwitchCharacter();
        CharacterSelectionIndex = 0;
        Debug.Log("Mario selected");
    }
    
    public void OnShrekImageClick()
    {
        SwitchCharacter();
        CharacterSelectionIndex = 1;
        Debug.Log("Shrek selected");
    }
    
    public void SwitchCharacter()
    {
        if (_player != null)
        {
            Destroy(_player); 
        }
    
        _player = Instantiate(_playerPrefab[CharacterSelectionIndex], _initialPosition, Quaternion.identity);
        _rb = _player.GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            _rb = _player.AddComponent<Rigidbody2D>();
        }

        _animator = _player.GetComponent<Animator>();
        
    }
    
    void Start()
    {
        _animator = _player.GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>(); 
        audioSource.volume = 0.3f;
        lastPosition = _player.transform.position;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        
        isGrounded = IsGrounded();
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpInput = true;
        }
        
        if(_animator.GetBool("IsJumping") && isGrounded)
        {
            _animator.SetBool("IsJumping", false);
        }
        
        float distanceX = _player.transform.position.x - lastPosition.x;
    
        if (distanceX > 0) // Only add distance if moving forward
        {
            distanceTraveled += distanceX;
            lastPosition = _player.transform.position;
        }

        // Timer to update score every interval
        timer += Time.deltaTime;
        if (timer >= scoreUpdateInterval)
        {
            CalculateAndAddScore();
            timer = 0f; // Reset timer
        }
    }

    private void HandleWalkingAnimations()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

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

    
    private void FixedUpdate()
    {
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
        Vector3 scale = _player.transform.localScale;
        scale.x *= -1;
        _player.transform.localScale = scale;
    }
    
    private bool IsGrounded()
    {
        float rayLength = 0.2f;
    
        RaycastHit2D hit = Physics2D.Raycast(_player.transform.position, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }


    public void SetCameraFollow(CameraFollow cameraFollow)
    {
        cameraFollow.PlayerTransform = _player.transform;
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
