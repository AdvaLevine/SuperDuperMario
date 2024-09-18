
using System.Collections;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private GameObject _playerPrefab;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;//the speed of the player
    [SerializeField] private float jumpForce = 6f;//the force of the jump
    [SerializeField] private LayerMask groundLayer;//the layer the player can jump from
    [SerializeField] private float jumpGravityScale = 0.5f; // Lower gravity during jump

    // Score settings
    [SerializeField] private float distanceThreshold = 0.5f; // Distance to accumulate points
    [SerializeField] private int pointMultiplier = 10; // Points per distance traveled
    private Vector3 lastPosition;
    private float distanceTraveled = 0f;
    private float timer = 0f;
    private float scoreUpdateInterval = 5f; // Update score every 10 seconds

    // Animation settings
    private bool facingRight = true;//check if the player is facing right
    private float moveInput;//the input for the movement
    private bool isGrounded;//check if the player is on the ground

    private Vector3 _initialPosition = new Vector3(-15f, 0, 0); //the beginning position of the player (far left of the screen)
    private Rigidbody2D _rb;    //the rigidbody of the player
    GameObject _player;     //the player object
    
    private float jumpForceMultiplier = 1f;
    private float coinMultiplier = 1f;

    private Coroutine jumpForceCoroutine;
    private Coroutine coinMultiplierCoroutine;

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
        _rb.gravityScale = jumpGravityScale;
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce * jumpForceMultiplier);
    }


    private void Awake()
    {
        _player = Instantiate(_playerPrefab, _initialPosition , Quaternion.identity);

        _rb = _player.GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = _player.AddComponent<Rigidbody2D>();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = _player.transform.position; //for the scores calculation
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        
        // Check if the player is grounded
        isGrounded = IsGrounded();
        
        // Calculate distance traveled since last frame for the scores
        float distance = Vector3.Distance(_player.transform.position, lastPosition);
        
        if (distance > 0)
        {
            // Accumulate the distance traveled
            distanceTraveled += distance;

            // Update last position
            lastPosition = _player.transform.position;
        }

        // Timer to update score every interval
        timer += Time.deltaTime;
        if (timer >= scoreUpdateInterval)
        {
            CalculateAndAddScore();
            timer = 0f; // Reset timer
        }

        // Jump if grounded and the jump button is pressed
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (_rb.velocity.y <= 0) // When falling down
        {
            _rb.gravityScale = 1; // Reset gravity scale
        }
        //HandleAnimations();
    }

    private void HandleAnimations()
    {
        throw new System.NotImplementedException();
    }
    
    private void CalculateAndAddScore()
    {
        if (distanceTraveled >= distanceThreshold)
        {
            // You can adjust this to increase points based on distance traveled
            int pointsToAdd = Mathf.FloorToInt(distanceTraveled / distanceThreshold) * pointMultiplier;
            Debug.Log($"Score added: {pointsToAdd}");
            ScoreManager.Instance.AddScore(pointsToAdd);
            distanceTraveled = 0f; // Reset distance traveled after adding score
        }
    }
    
    // private void Jump()
    // {
    //     _rb.gravityScale = jumpGravityScale;
    //     _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    // }
    
    private void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        // Set horizontal velocity
        _rb.velocity = new Vector2(moveInput * moveSpeed, _rb.velocity.y);

        // Flip the sprite based on movement direction
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
        float rayLength = 0.5f;
    
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
        // נטרול השליטה בשחקן
        enabled = false;
        // אפשר להוסיף אנימציית מוות
        // הצגת מסך Game Over
        GameManager.Instance.GameOver();
    }

    public void SetPlayerActive(bool b)
    {
        _player.SetActive(b);
    }
}
