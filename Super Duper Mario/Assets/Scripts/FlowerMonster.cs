
using UnityEngine;

public class FlowerMonster : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (collision.gameObject.CompareTag("Player"))
        {
            player.Die();
        }
    }
}
