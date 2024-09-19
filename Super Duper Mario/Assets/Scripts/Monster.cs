using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D _rb;
    private Animator _animator;
    private bool movingRight = true;

    private void Start()
    {
        if(GameManager.Instance.CurrentDifficulty == GameManager.Difficulty.Easy && gameObject.CompareTag("MonsterHard"))
        {
            Destroy(gameObject);
        }
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("MonsterHard"))
        {
            movingRight = !movingRight;
            Flip();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            if (collision.contacts[0].normal.y < -0.5f)
            {
                player.Bounce();
                Die();
                ScoreManager.Instance.AddScore(50);
            }
            else
            {
                player.Die();
            }
        }
    }

  
    private void Die()
    {
        _animator.SetTrigger("Die");
        StartCoroutine(DieAfterAnimation()); 
    }

    private IEnumerator DieAfterAnimation()
    {
        // חכה למשך זמן האנימציה
        yield return new WaitForSeconds(0.15f);

        // השמדת האובייקט לאחר סיום האנימציה
        Destroy(gameObject);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}