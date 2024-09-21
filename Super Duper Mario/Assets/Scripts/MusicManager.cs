using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public Slider volumeSlider;  
    public Button muteButton; 
    private bool isMuted = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (_audioSource != null)
        {
            _audioSource.Play();  
            
            if (volumeSlider != null)
            {
                volumeSlider.minValue = 0;
                volumeSlider.maxValue = 1; // Set slider range to 0-1
                volumeSlider.value = _audioSource.volume; // Set slider to current volume
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }

            if (muteButton != null)
            {
                muteButton.onClick.AddListener(ToggleMute);
            }
        }
    }

    public void PlayMusic()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();  
        }
    }

    public void PauseMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Pause();  
        }
    }

    public void ResumeMusic()
    {
        _audioSource.UnPause();  
    }
    public void SetVolume(float volume)
    {
        if (!isMuted) 
        {
            _audioSource.volume = volume;
        }
        
    }
    
    public void ToggleMute()
    {
        isMuted = !isMuted;
        _audioSource.mute = isMuted;

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