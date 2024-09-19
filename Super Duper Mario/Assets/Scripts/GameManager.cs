
using UnityEngine;
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
    [SerializeField] private Text _timer; // Add reference to the UI Text
    private float elapsedTime = 0f; // Time since the game started
    public GameObject timerText;
    [SerializeField] private float levelTime = 60f; // 60 seconds for the level
    
    [Header("Coin Settings")]
    [SerializeField] private int numberOfCoins = 15; // Number of coins to spawn
    [SerializeField] private float xMin = -10f; // Min X position for spawning coins
    [SerializeField] private float xMax = 16f; // Max X position for spawning coins
    [SerializeField] private float yMin = 1f; // Min Y position for spawning coins
    [SerializeField] private float yMax = 2.5f; // Max Y position for spawning coins

    [SerializeField] private float startX = -4f; // נקודת ההתחלה של השלב
    [SerializeField] private float endX = 19f; // נקודת הסיום של השלב
    [SerializeField] private int numberOfMonsters = 10; // מספר המפלצות שתרצה ליצור

    public enum Difficulty
    {
        Easy,
        Hard
    }

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;

    private CameraFollow _cameraFollow;
    private PlayerController _playerController;



    private void Awake()
    {
        ShowMainMenu();
    }

    private void Update()
    {
        if (Time.timeScale > 0f) // Only update the timer when the game is running
        {
            UpdateTimer();

        }

    }


    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        elapsedTime = 0f;
        timerText.SetActive(true); // Show "Time" UI when game starts
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

        SpawnMonsters();
        SpawnCoins();

    }

    private void UpdateTimer()
    {
        // Increment the elapsed time
        elapsedTime += Time.deltaTime;

        // Convert the time to minutes and seconds up
        //int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        //int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        //_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // time remaining instead of time elapsed
        int remainingTime = Mathf.FloorToInt(levelTime - elapsedTime);
        int remainingMinutes = Mathf.FloorToInt(remainingTime / 60f);
        int remainingSeconds = Mathf.FloorToInt(remainingTime % 60f);
        _timer.text = string.Format("{0:00}:{1:00}", remainingMinutes, remainingSeconds);

        // Check if the time is up and handle game over
        if (remainingTime <= 0)
        {
            // Ensure timeUpUI is activated only once
            if (timeUpUI != null)
            {
                timeUpUI.SetActive(true);
            }

            _timer.text = "00:00";
        }
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        timerText.SetActive(false); // Hide "Time" UI when in the menu
        mainMenuUI.SetActive(true);
        ScoreManager.Instance.ScoreText.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
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
    
    private void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Generate a random position within specified bounds
            float x = Random.Range(xMin, xMax);
            float y = Random.Range(yMin, yMax);
            Vector3 spawnPosition = new Vector3(x, y, 0);

            // Instantiate the coin prefab at the generated position
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    private void SpawnMonsters()
    {
        for (int i = 0; i < numberOfMonsters; i++)
        {
            // בחר מיקום רנדומלי לאורך ציר ה-X בתוך גבולות השלב
            float randomX = Random.Range(startX, endX);

            // נניח שגובה הקרקע הוא y = -4.7f (כמו שהגדרת את הקרקע)
            float groundY = -4.20375f;

            // צור את המפלצת במיקום הרנדומלי
            Vector3 spawnPosition = new Vector3(randomX, groundY, 0);

            Collider2D hitCollider = Physics2D.OverlapCircle(spawnPosition, 0.5f);
            if (hitCollider == null)
            {
                Instantiate(_monsterPrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                // אם המיקום תפוס, ננסה שוב
                i--;
            }
        }
    }
    public void GameOver()
    {
        // // הסתרת השחקן
        // if (_playerController != null)
        // {
        //     _playerController.SetPlayerActive(false);
        // }
        //
        // // הסתרת כל המפלצות בסצנה
        // foreach (GameObject monster in GameObject.FindGameObjectsWithTag("Monster"))
        // {
        //     monster.SetActive(false);
        // }
        //
        // // הסתרת רקע וקרקע
        // GameObject background = GameObject.FindWithTag("Background");
        // if (background != null)
        // {
        //     background.SetActive(false);
        // }
        //
        // GameObject ground = GameObject.FindWithTag("Ground");
        // if (ground != null)
        // {
        //     ground.SetActive(false);
        // }
        
        Time.timeScale = 0f;
        
        // הצגת ממשק הסיום
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // עצירת הזמן
      
    }

    public void PlayerWins()
    {
        // Stop the game or show the "You Win" UI
        Time.timeScale = 0f; // Freeze the game

        // Show the win screen
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
