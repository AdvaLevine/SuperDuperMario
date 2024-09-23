

using UnityEngine;

public class CastleEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerWins();
            //PlayerController.Instance.Die();
        }
    }
}
