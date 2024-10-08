using System.Collections;
using UnityEngine;

public class CastleEntrance : MonoBehaviour
{
    // To track the number of players that have entered the trigger
    private int playersInTrigger = 0;
    
    // Reference to the temporary camera prefab
    public GameObject tempCameraPrefab;
    
    // Keep track of the players that have entered
    private PlayerController player1;
    private PlayerController player2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Increment the counter for players that entered
            playersInTrigger++;
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            { 
                // Set the player reference
                if (player1 == null)
                {
                    player1 = player;
                }
                else if (player2 == null)
                {
                    player2 = player;
                }

                player.SetCanMove(false);
            }
            // Check if it's a two-player game
            if (GameManager.Instance.Players.Count == 2)
            {
                // Check if both players have entered the trigger
                if (playersInTrigger == 2)
                {
                    // Create a temporary camera before destroying the player
                    InstantiateTempCamera();
                    // Optionally destroy the players here if desired
                    Destroy(player1.gameObject);
                    Destroy(player2.gameObject);
                    // Start a coroutine to handle the win condition
                    StartCoroutine(HandlePlayerWin());
                }
            }
            else if (GameManager.Instance.Players.Count == 1)
            {
                // Create a temporary camera before destroying the player
                InstantiateTempCamera();
                
                Destroy(player1.gameObject);
                // For single player games, win immediately or handle differently
                StartCoroutine(HandlePlayerWin());
            }
        }
    }
    private void InstantiateTempCamera()
    {
        // Check if the main camera or another camera is still active before adding a new one
        if (Camera.main == null)
        {
            // Instantiate the temporary camera from a prefab
            if (tempCameraPrefab != null)
            {
                Instantiate(tempCameraPrefab);
            }
            else
            {
                // If no prefab is set, create a new camera and audio listener programmatically
                GameObject tempCamera = new GameObject("TempCamera");
                Camera cam = tempCamera.AddComponent<Camera>();
                cam.tag = "MainCamera"; // Make sure it has the MainCamera tag
                tempCamera.AddComponent<AudioListener>(); // Add an Audio Listener
            }
        }
    }
    private IEnumerator HandlePlayerWin()
    {
        // Wait for a frame to ensure the player is destroyed
        yield return new WaitForEndOfFrame();
        
        // Now call the PlayerWins method
        GameManager.Instance.PlayerWins();
        
        playersInTrigger = 0; // Reset the count for future triggers
    }
}