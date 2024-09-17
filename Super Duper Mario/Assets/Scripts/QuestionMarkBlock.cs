using UnityEngine;

public class QuestionMarkBlock : MonoBehaviour
{
    public enum BlockEffect
    {
        HigherJump,
        LowerJump,
        AddPoints,
        SubtractPoints,
        DoubleCoins,
        HalveCoins
    }

    [SerializeField] private BlockEffect blockEffect;

    private bool _isHit = false;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        // שינוי צבע בהתאם לדרגת הקושי והאפקט
        if (GameManager.Instance.CurrentDifficulty == GameManager.Difficulty.Easy)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (IsGoodEffect())
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
            // בדיקה אם הפגיעה היא מלמטה
            if (collision.contacts[0].normal.y > 0.5f)
            {
                _isHit = true;
                _animator.SetTrigger("Hit");
                ApplyEffect(collision.gameObject.GetComponent<PlayerController>());
            }
        }
    }

    private void ApplyEffect(PlayerController player)
    {
        switch (blockEffect)
        {
            case BlockEffect.HigherJump:
                player.SetJumpForceMultiplier(1.5f);
                break;
            case BlockEffect.LowerJump:
                player.SetJumpForceMultiplier(0.75f);
                break;
            case BlockEffect.AddPoints:
                ScoreManager.Instance.AddScore(10);
                break;
            case BlockEffect.SubtractPoints:
                ScoreManager.Instance.AddScore(-10);
                break;
            case BlockEffect.DoubleCoins:
                player.SetCoinMultiplier(2, 10f);
                break;
            case BlockEffect.HalveCoins:
                player.SetCoinMultiplier(0.5f, 10f);
                break;
        }
    }

    private bool IsGoodEffect()
    {
        return blockEffect == BlockEffect.HigherJump || blockEffect == BlockEffect.AddPoints || blockEffect == BlockEffect.DoubleCoins;
    }
}
