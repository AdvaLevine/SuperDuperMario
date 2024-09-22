using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private AudioClip coinCollectSound;
    
    private static bool isCoinMuted = false; 
    private AudioSource audioSource;
    
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

            if (coinCollectSound != null && !isCoinMuted)
            {
                PlayCoinSound();
            }

            Destroy(gameObject);
        }
    }
    
    private void PlayCoinSound()
    {
        GameObject soundObject = new GameObject("CoinSound");
        AudioSource soundSource = soundObject.AddComponent<AudioSource>();
        
        soundSource.clip = coinCollectSound;
        soundSource.volume = 0.5f; 
        soundSource.Play();
        Destroy(soundObject, coinCollectSound.length);
    }
    
    public void ToggleCoinMute()
    {
        isCoinMuted = !isCoinMuted;
    }
}
