using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private Text _score; // Reference to the child Text component (e.g., "Score")
    [SerializeField] private GameObject scoreText; // Reference to the parent Text component (e.g., "ScoreText")
    
    public GameObject ScoreText => scoreText; // Public property to access the scoreText

    private int score;
    
    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (_score != null)
        {
            _score.text = score.ToString();
        }
    }
}