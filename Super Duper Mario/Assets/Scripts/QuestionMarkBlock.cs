using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestionMarkBlock : MonoBehaviour
{
    private Vector3 originalPosition;
    public float jumpHeight = 0.2f;  
    public float jumpDuration = 0.1f;  
    
    public enum BlockEffect
    {
        HigherJump,
        LowerJump,
        AddPoints,
        SubtractPoints,
        DoubleCoins,
        HalfCoins
    }

    [SerializeField] private BlockEffect blockEffect;

    private bool _isHit = false;
    private Animator _animator;

    private void Start()
    {
        originalPosition = transform.position;
        _animator = GetComponent<Animator>();
        if (GameManager.Instance.CurrentDifficulty == GameManager.Difficulty.Easy)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (IsGoodEffect()) // if easy mode, make all blocks colored
            {
                renderer.color = Color.green; 
            }
            else
            {
                renderer.color = Color.red;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isHit && collision.gameObject.CompareTag("Player"))
        {
            // Check if player is hitting the block from below
            if (collision.contacts[0].normal.y > 0.5f)
            {
                _isHit = true;
                _animator.SetTrigger("Hit");
                StartCoroutine(JumpUpAndDown());
                // Identify which player is colliding and show the message
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if (player != null)
                {
                    // Check the type of player and set the message accordingly
                    string playerName = "";

                    if (player is MarioController)
                    {
                        playerName = "Mario";
                    }
                    else if (player is ShrekController)
                    {
                        playerName = "Shrek";
                    }

                    // Construct the message with the player's name
                    string message = playerName + ":\n";
                    
                    ApplyEffect(player, message);
                }
            }
        }
    }

    private void ApplyEffect(PlayerController player, string message)
    {
        switch (blockEffect)
        {
            case BlockEffect.HigherJump:
                player.SetJumpForceMultiplier(1.5f);
                message += "Higher Jump for 5 seconds!";
                break;
            case BlockEffect.LowerJump:
                player.SetJumpForceMultiplier(0.5f);
                message += "Lower Jump for 5 seconds!";
                break;
            case BlockEffect.AddPoints:
                ScoreManager.Instance.AddScore(50);
                message += "+ 50 points!";
                break;
            case BlockEffect.SubtractPoints:
                ScoreManager.Instance.AddScore(-50);
                message += "- 50 points!";
                break;
            case BlockEffect.DoubleCoins:
                player.SetCoinMultiplier(2, 10f);
                message += "Coins Doubled!";
                break;
            case BlockEffect.HalfCoins:
                player.SetCoinMultiplier(0.5f, 10f);
                message += "Coins Halved!";
                break;
        }
        // Show the message using the power-up text prefab
        GameManager.Instance.ShowPopUpMessage(message);
    }
    private bool IsGoodEffect()
    {
        return blockEffect == BlockEffect.HigherJump || blockEffect == BlockEffect.AddPoints || blockEffect == BlockEffect.DoubleCoins;
    }
    
    IEnumerator JumpUpAndDown()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + jumpHeight, transform.position.z);
        
        float elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
    
}
