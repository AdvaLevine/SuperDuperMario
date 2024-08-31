using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    [SerializeField] private GameObject _playerPrefab;
    
    
    //[SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool facingRight = true;
    private float moveInput;
    private bool isGrounded;

    private Vector3 _initialPosition = new Vector3(0, 0, 0);
    private Rigidbody2D _rb;


    private void Awake()
    {
        GameObject player = Instantiate(_playerPrefab, _initialPosition , Quaternion.identity);
        _rb = player.GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = player.AddComponent<Rigidbody2D>();
        }

        //BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
        
        // // Ensure GroundCheck is assigned
        // if (groundCheck == null)
        // {
        //     // Create a new GroundCheck object if not assigned
        //     GameObject gc = new GameObject("GroundCheck");
        //     gc.transform.parent = transform;
        //     gc.transform.localPosition = new Vector3(0, -1f, 0); // Adjust position as needed
        //     groundCheck = gc.transform;
        // }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("Mario has landed on the floor.");
            // Here, ensure that velocity in the Y direction is set to zero
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

        if (Input.GetButtonDown("Jump"))
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
        // Apply jump force
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        // Optionally, trigger jump animation
        //if (animator != null)
        //    animator.SetTrigger("Jump");
    }
    
    private void FixedUpdate()
    {
        // Check if grounded
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Move the player
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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    
}
