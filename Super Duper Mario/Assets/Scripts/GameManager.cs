
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _BackgroundPrefab;
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private GameObject coinPrefab;

    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject highScoreUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winScreenUI;
    [SerializeField] private GameObject timeUpUI;
    [SerializeField] private GameObject pauseMenuUI;
    
    [Header("Time Settings")]
    [SerializeField] private Text _timer; 
    private float elapsedTime = 0f;
    public GameObject timerText;
    [SerializeField] private float levelTime = 120f; 
    [Header("Difficulty Settings")]
    [SerializeField] private Button easyButton; // כפתור ה-Easy
    
    private bool playerHasWon = false;
    private bool isFirstGame = true;
    private bool isGamePaused = false;

    [SerializeField] private Text _finalScore; 

    [Header("Audio Settings")]
    [SerializeField] private AudioClip winSound; // The win sound clip
    [SerializeField] private AudioClip loseSound; // The lose sound clip
    private AudioSource audioSource;
    private MusicManager musicManager;

    public enum Difficulty
    {
        Easy,
        Hard
    }

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;

    private CameraFollow _cameraFollow;
    private PlayerController _playerController;


    public bool HasPlayerWon()
    {
        return playerHasWon;
    }
    private void Awake()
    {
        ShowMainMenu();
        DontDestroyOnLoad(highScoreUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }

        if (!isGamePaused && Time.timeScale > 0f)
        {
            UpdateTimer();
        }
    }
    
    public void StartGame()
    {
        if (isFirstGame)
        {
            PlayerPrefs.DeleteKey("HighScores");
            isFirstGame = false;
        }
        
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        elapsedTime = 0f;
        timerText.SetActive(true);
        ScoreManager.Instance.ScoreText.SetActive(true);
        ScoreManager.Instance.ResetScore();
      
        Instantiate(_BackgroundPrefab, new Vector3(-3.9f, -4.5f, 0), Quaternion.identity);
        CreateGroundFromPrefab();
        CalculateBoundsForCamera();

        _playerController = PlayerController.Instance;
        _playerController.SetCameraFollow(_cameraFollow);
        
        SetUpAudioInGame();
    }

    private void SetUpAudioInGame()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        musicManager = FindObjectOfType<MusicManager>();
    }

    private void CalculateBoundsForCamera()
    {
        _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        _cameraFollow.LeftBound = GameObject.Find("Left Bounds").transform;
        _cameraFollow.RightBound = GameObject.Find("Right Bounds").transform;
        _cameraFollow.CalculateBounds();
    }

    private void CreateGroundFromPrefab()
    {
        GameObject ground = Instantiate(_groundPrefab, new Vector3(0, -4.7f, 0), Quaternion.identity);
        ground.transform.localScale = new Vector3(60, 1, 1);
        Material groundMaterial = new Material(Shader.Find("Unlit/Transparent"));
        groundMaterial.mainTexture = _groundPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        groundMaterial.mainTextureScale = new Vector2(60, 1);
        ground.GetComponent<SpriteRenderer>().material = groundMaterial;
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;

        int remainingTime = Mathf.FloorToInt(levelTime - elapsedTime);
        int remainingMinutes = Mathf.FloorToInt(remainingTime / 60f);
        int remainingSeconds = Mathf.FloorToInt(remainingTime % 60f);
        _timer.text = string.Format("{0:00}:{1:00}", remainingMinutes, remainingSeconds);

        if (remainingTime <= 0)
        {
            if (timeUpUI != null)
            {
                StopMusic();
                PlayLoseSound();
                timeUpUI.SetActive(true);
                PlayerController.Instance.Die();
                gameOverUI.SetActive(false);
            }

            _timer.text = "00:00";
        }
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        timerText.SetActive(false); 
        mainMenuUI.SetActive(true);
        ScoreManager.Instance.ScoreText.SetActive(false);
        EventSystem.current.SetSelectedGameObject(easyButton.gameObject);

    }

    public void ExitGame()
    {
        ScoreManager.Instance.DeleteHighScores();
        isFirstGame = true;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void SetDifficulty(int difficulty)
    {
        CurrentDifficulty = (Difficulty)difficulty;
    }

    public void ShowHighScore()
    {
        ScoreManager.Instance.ShowHighScores(); 
        highScoreUI.SetActive(true);
    }

    public void CloseHighScore()
    {
        highScoreUI.SetActive(false);
    }
    
    public void GameOver()
    {
        StopMusic();
        PlayLoseSound();
        ScoreManager.Instance.SaveHighScores();

        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }
    
    public void PlayerWins()
    {
        playerHasWon = true; 
        StopMusic();
        PlayWinSound();
        
        // Use elapsed time to calculate time taken to win
        float timeTakenToWin = elapsedTime; 
        int finalScore = CalculateHighScore(ScoreManager.Instance.Score, timeTakenToWin);

        ScoreManager.Instance.AddHighScore(finalScore); 
        _finalScore.text = finalScore.ToString(); 
        ScoreManager.Instance.SaveHighScores();
        
        Time.timeScale = 0f; 

        if (winScreenUI != null)
        {
            winScreenUI.SetActive(true); 
        }
    }

    private int CalculateHighScore(int score, float timeTaken)
    {
        // Calculate the time remaining
        float timeRemaining = Mathf.Max(0, levelTime - timeTaken);
    
        // Scale the time bonus based on how much time is left
        int timeBonus = Mathf.FloorToInt(timeRemaining); 

        // Calculate final score
        int finalScore = score + timeBonus; 
        return Mathf.Max(0, finalScore); 
    }

    
    private void StopMusic()
    {
        if (musicManager != null)
        {
            musicManager.StopMusic();
        }
    }
    private void PlayWinSound()
    {
        if (winSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winSound); 
        }

    }

    private void PlayLoseSound()
    {
        if (loseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(loseSound); 
        }
        
    }

    public void RestartGame()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        
        if(timeUpUI != null)
        {
            timeUpUI.SetActive(false);
        }
        
        if(winScreenUI != null)
        {
            winScreenUI.SetActive(false);
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        StopMusic();
        Time.timeScale = 0f;  
        pauseMenuUI.SetActive(true);  
    }

    private void ResumeGame()
    {
        ResumeMusic();
        Time.timeScale = 1f;  
        pauseMenuUI.SetActive(false); 
    }

    private void ResumeMusic()
    {
        if (musicManager != null)
        {
            musicManager.ResumeMusic(); 
        }
    }
}
