using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Settings")]
    [SerializeField] private float moveSpeed = 2f;
    
    
    private float idleTimeBeforeFlip = 0.1f; // זמן ללא תנועה לפני שהמפלצת מסתובבת
    private Rigidbody2D _rb;
    private Animator _animator;
    private bool movingRight = true;
    private float idleTime;
    private Vector2 lastPosition;

    private void Start()
    {
        if(GameManager.Instance.CurrentDifficulty == GameManager.Difficulty.Easy && (gameObject.CompareTag("MonsterHard") || gameObject.CompareTag("MonsterMedium")))
        {
            Destroy(gameObject);
        }
        if(GameManager.Instance.CurrentDifficulty == GameManager.Difficulty.Medium && gameObject.CompareTag("MonsterHard"))
        {
            Destroy(gameObject);
        }
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        lastPosition = transform.position; 

    }

    private void Update()
    {
        Move();
        CheckIfIdle();
    }
    
    private void CheckIfIdle()
    {
        // בודקים אם המיקום בציר ה-X לא השתנה בצורה משמעותית
        if (Mathf.Abs(transform.position.x - lastPosition.x) < 0.01f)
        {
            idleTime += Time.deltaTime; // צוברים זמן אם המפלצת לא זזה

            if (idleTime >= idleTimeBeforeFlip) // אם עבר מספיק זמן שהמפלצת לא זזה
            {
                movingRight = !movingRight; // מחליפים כיוון
                Flip();
                idleTime = 0f; // מאפסים את הזמן
            }
        }
        else
        {
            idleTime = 0f; // אם המפלצת זזה, מאפסים את הזמן
        }

        lastPosition = transform.position; // מעדכנים את המיקום האחרון
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
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("MonsterHard") || collision.gameObject.CompareTag("MonsterMedium"))
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
    
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    private void Die()
    {
        _animator.SetTrigger("Die");
        StartCoroutine(DieAfterAnimation()); 
    }

    private IEnumerator DieAfterAnimation()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
}