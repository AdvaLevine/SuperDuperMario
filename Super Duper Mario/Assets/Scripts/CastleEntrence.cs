

using UnityEngine;

public class CastleEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the trigger
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerWins();
            PlayerController.Instance.Die(); // So wont be able to move after winning
        }
    }
}
