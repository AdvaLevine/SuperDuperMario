
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _BackgroundPrefab;
    [SerializeField] private GameObject _monsterPrefab;
    
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject highScoreUI;
    [SerializeField] private GameObject gameOverUI;
    
    [SerializeField] private TMPro.TextMeshProUGUI _timer;  // Add reference to the UI Text
    private float elapsedTime = 0f;          // Time since the game started
    public GameObject timerText;
    
    [SerializeField] private float levelTime = 60f;  // 300 seconds for the level
    [SerializeField] private float startX = -4f; // נקודת ההתחלה של השלב
    [SerializeField] private float endX = 19f;   // נקודת הסיום של השלב
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
        if (Time.timeScale > 0f)  // Only update the timer when the game is running
        {
            UpdateTimer();
        }
    }
    
    
    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        elapsedTime = 0f;
        timerText.SetActive(true);  // Show "Time" UI when game starts

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
    }
    
    private void UpdateTimer()
    {
        // Increment the elapsed time
        elapsedTime += Time.deltaTime;

        // Convert the time to minutes and seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Update the text field with the formatted time
        _timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        timerText.SetActive(false); // Hide "Time" UI when in the menu
        mainMenuUI.SetActive(true);
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

        // הצגת ממשק הסיום
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // עצירת הזמן
        Time.timeScale = 0f;
    }

    public void PlayerWins()
    {
        // Stop the game or show the "You Win" UI
        Debug.Log("Player Wins!");
        Time.timeScale = 0f; // Freeze the game

        // You can display a win screen UI if you have one
        // For example, if you have a win UI element:
        
        gameOverUI.SetActive(true); // Or use a dedicated win UI
        // Optionally, you could show a custom "You Win" message here.
    }

    public void RestartGame()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
