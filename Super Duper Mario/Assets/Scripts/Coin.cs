using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            int totalCoinValue = Mathf.RoundToInt(coinValue * player.GetCoinMultiplier());
            ScoreManager.Instance.AddScore(totalCoinValue);
            Destroy(gameObject);
        }
    }
}
