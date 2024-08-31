
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private GameObject _playerPrefab;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private LayerMask groundLayer;


    private bool facingRight = true;
    private float moveInput;
    private bool isGrounded;

    private Vector3 _initialPosition = new Vector3(-10f, 0, 0);
    private Rigidbody2D _rb;
    GameObject _player;


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

        //HandleAnimations();
    }

    private void HandleAnimations()
    {
        throw new System.NotImplementedException();
    }

    private void Jump()
    {
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
