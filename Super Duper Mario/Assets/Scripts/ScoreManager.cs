using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _score = 0;
    public int Score { get { return _score; } }

    [SerializeField] private Text scoreText;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + _score;
        }
    }
}