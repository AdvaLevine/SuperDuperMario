
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject[] _playerPrefabs; 

    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject highScoreUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winScreenUI;
    [SerializeField] private GameObject timeUpUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject levelUpUI;
    
    
    [Header("Time Settings")]
    [SerializeField] private Text _timer; 
    private float elapsedTime = 0f;
    public GameObject timerText;
    [SerializeField] private float levelTime = 120f; 
    
    [Header("Difficulty Settings")]
    [SerializeField] private Button easyButton; 
    
    // Game state variables
    private bool playerHasWon = false;
    private bool isFirstGame = true;
    private bool isGamePaused = false;

    [Header("Final Score Settings")]
    [SerializeField] private Text _finalScore;
    [SerializeField] private Text _levelScore;
    private int TotalGameScore = 0;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip winSound; // The win sound clip
    [SerializeField] private AudioClip loseSound; // The lose sound clip
    [SerializeField] private AudioClip marioSelectSound; 
    [SerializeField] private  AudioClip shrekSelectSound; 
    private AudioSource audioSource;
    private MusicManager musicManager;
    
    private int numberOfPlayers = 1; //deafault 1 player
    private List<PlayerController> players = new List<PlayerController>(); // list of players in the game
    
    public List<PlayerController> Players => players;
    public int CharacterSelectionIndex { get; set; } = 0;
    public int NumberOfPlayers => numberOfPlayers;

    public enum Difficulty
    {
        Easy,
        Hard
    }

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;
    
    public bool HasPlayerWon()
    {
        return playerHasWon;
    }
    private void Awake()
    {
        ShowMainMenu();
        DontDestroyOnLoad(highScoreUI);
    }

    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); 
        audioSource.volume = 0.3f;
    }

    private void CreateDivider()
    {
        // יצירת Canvas
        GameObject canvasObject = new GameObject("DividerCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // הוספת Image שישמש כפס
        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(canvas.transform);

        // הגדרת RectTransform של הפס (Image)
        RectTransform rectTransform = divider.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(5, Screen.height); // פס בגודל 5 פיקסלים לגובה המסך
        rectTransform.anchoredPosition = new Vector2(0, 0); // מיקום הפס במרכז המסך

        // הוספת רכיב Image ושינוי צבעו
        Image image = divider.AddComponent<Image>();
        image.color = Color.black; // ניתן להחליף לצבע אחר
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
      
        // Start the level and instantiate the background
        LoadLevelWithBackground();
        CreateGroundFromPrefab();
        
        if (numberOfPlayers == 2)
        {
            CreateDivider();
        }

        SetUpPlayers();
        
        SetUpAudioInGame();
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }
    private void LoadLevelWithBackground()
    {
        // Load the next level
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevelIndex);
    }
    
    private void SetUpPlayers()
    {
        players.Clear();
        
        // Remove any existing AudioListeners before setting up new players
        
        AudioListener existingAudioListener = FindObjectOfType<AudioListener>();
       if (existingAudioListener != null)
       {
        Destroy(existingAudioListener.gameObject);
       }
        
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 spawnPosition = new Vector3(-15f + i * 0.5f, 0f, 0f); // מיקום התחלתי לכל שחקן
            GameObject playerObject;
            
            if (numberOfPlayers == 1)
                playerObject = Instantiate(_playerPrefabs[CharacterSelectionIndex], spawnPosition, Quaternion.identity);
            else
                playerObject = Instantiate(_playerPrefabs[i], spawnPosition, Quaternion.identity);
            
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            players.Add(playerController);
            
            //create cameras for each player
            GameObject cameraObject = new GameObject("PlayerCamera" + (i + 1));
            Camera playerCamera = cameraObject.AddComponent<Camera>();
            
            if (i == 0)
            {
                cameraObject.AddComponent<AudioListener>();
            }
            //connect the camera to the player
            cameraObject.transform.SetParent(playerObject.transform); // המצלמה תהיה Child של השחקן
            cameraObject.transform.localPosition = new Vector3(1f, 1f, -6f); // הגדרת מיקום המצלמה יחסית לשחקן
            playerCamera.orthographic = true; // מצלמה אורתוגרפית
            playerCamera.orthographicSize = 2.7f; // גודל התצוגה האנכית של המצלמה
            playerCamera.clearFlags = CameraClearFlags.SolidColor; // צבע רקע של המצלמה
            
            CalculateBoundsForCamera(playerCamera, playerObject.transform);

            // חלוקת מסך (Split-Screen)
            float width = 1f / numberOfPlayers; // מחשבים את החלק היחסי לכל שחקן במסך
            // Adjust the camera rect for player one and two
            if (numberOfPlayers == 2)
            {
                // Set Player 1's camera rect to the right and Player 2's to the left
                playerCamera.rect = new Rect((1 - width) - (i * width), 0, width, 1); // Invert the x position
            }
            else
            {
                playerCamera.rect = new Rect(i * width, 0, width, 1); // Default behavior for more players
            }
        }
        ResetPlayersMovement();
    }
    
    private void ResetPlayersMovement()
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                player.SetCanMove(true); 
            }
        }
    }
    public void OnMarioImageClick()
    {
        // Play Mario selection sound
        if (marioSelectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(marioSelectSound);
        }
        CharacterSelectionIndex = 0;
        Debug.Log("Mario selected");
    }
    
    public void OnShrekImageClick()
    {
        if (shrekSelectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shrekSelectSound);
        }
        CharacterSelectionIndex = 1;
        Debug.Log("Shrek selected");
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

    private void CalculateBoundsForCamera(Camera playerCamera, Transform playerTransform)
    {
        playerCamera.gameObject.AddComponent<CameraFollow>();
        CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
        cameraFollow.LeftBound = GameObject.Find("Left Bounds").transform;
        cameraFollow.RightBound = GameObject.Find("Right Bounds").transform;
        cameraFollow.PlayerTransform = playerTransform;
        cameraFollow.CalculateBounds(numberOfPlayers);
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
                TotalGameScore = 0;
                timeUpUI.SetActive(true);
                foreach (var player in players)
                {
                    player.Die();
                }
                gameOverUI.SetActive(false);
            }

            _timer.text = "00:00";
        }
    }
    
    public void SetNumberOfPlayers(int numPlayers)
    {
        numberOfPlayers = numPlayers;
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
        TotalGameScore = 0;
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
        TotalGameScore = 0;
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
        
        elapsedTime = 120f; // Resetting elapsedTime to 120 seconds

        // Use elapsed time to calculate time taken to win
        float timeTakenToWin = elapsedTime; 
        int levelScore = CalculateHighScore(ScoreManager.Instance.Score, timeTakenToWin);
        
        _levelScore.text = levelScore.ToString();
        TotalGameScore += levelScore;
        
        ScoreManager.Instance.AddHighScore(TotalGameScore); 
        ScoreManager.Instance.SaveHighScores(); //to save score only if won level
        Time.timeScale = 0f; 
       
        // Check if there are more levels available
        if (LevelManager.Instance.CurrentLevelIndex < LevelManager.Instance.levelBackgrounds.Length - 1)
        {
            if(levelUpUI != null)
            {
                levelUpUI.SetActive(true);
            }
            
            Time.timeScale = 1f; // Reset the time scale to allow the coroutine to run
            StartCoroutine(LoadNextLevelAfterDelay(3f)); // Call coroutine with a short delay
            PlayMusic();
        }
        else
        {
            _finalScore.text = TotalGameScore.ToString(); 
            if (winScreenUI != null)
            {
                winScreenUI.SetActive(true); 
            } 
        }
    }

    private void PlayMusic()
    {
        if (musicManager != null)
        {
            musicManager.PlayMusic();
        }
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        playerHasWon = false;
        
        // Remove the win screen UI
        if (levelUpUI != null)
        {
            levelUpUI.SetActive(false);
        }

        // Load the next level    
        LevelManager.Instance.LoadNextLevel();
        // Reset players' positions
        SetUpPlayers();
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

        TotalGameScore = 0;
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
        ScoreManager.Instance.SaveHighScores();

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

}
