
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

    [Header("Time Settings")]
    [SerializeField] private Text _timer; 
    private float elapsedTime = 0f;
    public GameObject timerText;
    [SerializeField] private float levelTime = 120f; 
    [SerializeField] private Button easyButton; // כפתור ה-Easy
    
    private bool playerHasWon = false;

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
    }

    private void Update()
    {
        if (Time.timeScale > 0f)
        {
            UpdateTimer();

        }

    }
    
    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        elapsedTime = 0f;
        timerText.SetActive(true);
        ScoreManager.Instance.ScoreText.SetActive(true);
        ScoreManager.Instance.ResetScore();

        Instantiate(_BackgroundPrefab, new Vector3(-3.9f, -4.5f, 0), Quaternion.identity);
        GameObject ground = Instantiate(_groundPrefab, new Vector3(0, -4.7f, 0), Quaternion.identity);
        ground.transform.localScale = new Vector3(60, 1, 1);
        Material groundMaterial = new Material(Shader.Find("Unlit/Transparent"));
        groundMaterial.mainTexture = _groundPrefab.GetComponent<SpriteRenderer>().sprite.texture;
        groundMaterial.mainTextureScale = new Vector2(60, 1);
        ground.GetComponent<SpriteRenderer>().material = groundMaterial;

        _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        _cameraFollow.LeftBound = GameObject.Find("Left Bounds").transform;
        _cameraFollow.RightBound = GameObject.Find("Right Bounds").transform;
        _cameraFollow.CalculateBounds();

        _playerController = PlayerController.Instance;
        _playerController.SetCameraFollow(_cameraFollow);

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
                timeUpUI.SetActive(true);
                PlayerController.Instance.Die(); // So wont be able to move after time is up
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
        // Check if we are running in the Unity Editor
    #if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Quit the application in the build
        Application.Quit();
    #endif
    }

    public void SetDifficulty(int difficulty)
    {
        CurrentDifficulty = (Difficulty)difficulty;
    }

    public void ShowHighScore()
    {
        highScoreUI.SetActive(true);
    }

    public void CloseHighScore()
    {
        highScoreUI.SetActive(false);
    }
    
    public void GameOver()
    {
        Time.timeScale = 0f;
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void PlayerWins()
    {
        Time.timeScale = 0f; 
        playerHasWon = true; // Set the flag to true to track win state

        if (winScreenUI != null)
        {
            //todo: ADD animation for flag and mario dissapearing
            winScreenUI.SetActive(true); // Activate the win screen UI
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
}
