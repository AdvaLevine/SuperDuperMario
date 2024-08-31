
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private GameObject _playerPrefab;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;//the speed of the player
    [SerializeField] private float jumpForce = 6f;//the force of the jump
    [SerializeField] private LayerMask groundLayer;//the layer the player can jump from
    [SerializeField] private float jumpGravityScale = 0.5f; // Lower gravity during jump


    private bool facingRight = true;//check if the player is facing right
    private float moveInput;//the input for the movement
    private bool isGrounded;//check if the player is on the ground

    private Vector3 _initialPosition = new Vector3(-15f, 0, 0); //the beginning position of the player (far left of the screen)
    private Rigidbody2D _rb;    //the rigidbody of the player
    GameObject _player;     //the player object


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
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        
        // Check if the player is grounded
        isGrounded = IsGrounded();

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

    private void Jump()
    {
        _rb.gravityScale = jumpGravityScale;
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }
    
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
}
