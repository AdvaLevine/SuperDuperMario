using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            int totalCoinValue = Mathf.RoundToInt(coinValue * player.GetCoinMultiplier());
            ScoreManager.Instance.AddScore(totalCoinValue);
            Debug.Log("Coin collected. Value: " + totalCoinValue);

            Destroy(gameObject);
        }
    }
}
