using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip jumpClip; // AudioClip for jump sound
    private AudioSource _jumpSource; // Separate AudioSource for jump sound
    
    public Slider volumeSlider;  // Slider for volume control
    public Button muteButton; // Button to mute the music
    private bool isMuted = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _jumpSource = gameObject.AddComponent<AudioSource>(); // Add a new AudioSource for jump sound
        _jumpSource.clip = jumpClip;
        
        if (_audioSource != null)
        {
            _audioSource.Play();  // Starts playing the music
            
            // Initialize volume slider
            if (volumeSlider != null)
            {
                volumeSlider.minValue = 0;
                volumeSlider.maxValue = 1; // Set slider range to 0-1
                volumeSlider.value = _audioSource.volume; // Set slider to current volume
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }

            // Initialize mute button
            if (muteButton != null)
            {
                muteButton.onClick.AddListener(ToggleMute);
            }
        }
    }

    // Play the music
    public void PlayMusic()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
    // Play the jump sound
    public void PlayJumpSound()
    {
        if (_jumpSource != null && jumpClip != null)
        {
            _jumpSource.PlayOneShot(jumpClip);
        }
    }
    public void StopMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();  // Stops the music
        }
    }

    public void PauseMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Pause();  // Pauses the music
        }
    }

    public void ResumeMusic()
    {
        _audioSource.UnPause();  // Resumes the music
    }
    // Set volume level
    public void SetVolume(float volume)
    {
        if (!isMuted) // Only update volume if not muted
        {
            _audioSource.volume = volume;
        }
        
    }
    
    // Toggle mute/unmute
    public void ToggleMute()
    {
        isMuted = !isMuted;
        _audioSource.mute = isMuted;

        // Update volume based on mute state
        if (isMuted)
        {
            _audioSource.volume = 0;
        }
        else
        {
            _audioSource.volume = volumeSlider.value;
        }    
    }
}