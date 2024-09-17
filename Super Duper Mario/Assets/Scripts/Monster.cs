using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D _rb;
    private bool movingRight = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (movingRight)
        {
            _rb.velocity = new Vector2(moveSpeed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(-moveSpeed, _rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Monster"))
        {
            movingRight = !movingRight;
            Flip();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            if (collision.contacts[0].normal.y < -0.5f)
            {
                Die();
                ScoreManager.Instance.AddScore(50);
                player.Bounce();
            }
            else
            {
                player.Die();
            }
        }
    }

    private void Die()
    {
        // אפשר להוסיף אנימציית מוות
        Destroy(gameObject);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}