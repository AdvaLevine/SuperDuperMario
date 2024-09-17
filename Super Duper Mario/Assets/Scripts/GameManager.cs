
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

    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
        
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

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
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
