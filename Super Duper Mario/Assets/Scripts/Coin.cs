using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private AudioClip coinCollectSound; // Coin collection sound

    private AudioSource audioSource; // AudioSource for playing sound
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            int totalCoinValue = Mathf.RoundToInt(coinValue * player.GetCoinMultiplier());
            ScoreManager.Instance.AddScore(totalCoinValue);
            // Play the coin sound
            PlayCoinSound();

            // Destroy the coin immediately
            Destroy(gameObject);
            
        }
    }
    
    private void PlayCoinSound()
    {
        // Create a temporary GameObject for the sound
        GameObject soundObject = new GameObject("CoinSound");
        AudioSource soundSource = soundObject.AddComponent<AudioSource>();
        soundSource.clip = coinCollectSound;
        soundSource.volume = 0.5f; // Adjust the volume if necessary
        soundSource.Play();

        // Destroy the sound object after the sound has played
        Destroy(soundObject, coinCollectSound.length);
    }
}
