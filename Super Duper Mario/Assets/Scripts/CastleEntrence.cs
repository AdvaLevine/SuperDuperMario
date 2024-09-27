using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CastleEntrance : MonoBehaviour
{
    // To track the number of players that have entered the trigger
    private int playersInTrigger = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Increment the counter for players that entered
            playersInTrigger++;
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            { 
                player.SetCanMove(false);
                Destroy(player.gameObject); // Stop the player from moving
            }
            // Check if it's a two-player game
            if (GameManager.Instance.Players.Count == 2)
            {
                // Check if both players have entered the trigger
                if (playersInTrigger == 2)
                {
                    // Start a coroutine to handle the win condition
                    StartCoroutine(HandlePlayerWin());
                }
            }
            else if (GameManager.Instance.Players.Count == 1)
            {
                // For single player games, win immediately or handle differently
                StartCoroutine(HandlePlayerWin());
            }
        }
    }

    private IEnumerator HandlePlayerWin()
    {
        // Wait for a frame to ensure the player is destroyed
        yield return new WaitForEndOfFrame();
        
        // Now call the PlayerWins method
        GameManager.Instance.PlayerWins();
        
    }
}