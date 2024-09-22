using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    [Header("Score Settings")]
    [SerializeField] private Text _score; // Text object to display the score
    [SerializeField] private GameObject scoreText; // Text object to display the score word
    [SerializeField] private GameObject scorePrefab; // Prefab for score entries
    [SerializeField] private Transform content; // Content area of the Scroll View

    private List<int> highScores = new List<int>(); // List to store high scores
    public GameObject ScoreText => scoreText; 
    private int score;
    public int Score => score; 
    
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

    public void AddHighScore(int newScore)
    {
        // Only add the new score if it's higher than the lowest in the top 5 or if we have fewer than 5 scores
        if (highScores.Count < 10 || newScore > highScores[highScores.Count - 1])
        {
            highScores.Add(newScore);
            highScores.Sort((a, b) => b.CompareTo(a)); // Sort in descending order
            
            // Keep only the top 5 scores
            if (highScores.Count > 10)
            {
                highScores.RemoveAt(10);
            }

            UpdateHighScoreUI();
            SaveHighScores(); 
        }
    }
    
    private void UpdateHighScoreUI()
    {
        // Clear the content area
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Add the high scores to the content area
        for (int i = 0; i < highScores.Count; i++)
        {
            int highScore = highScores[i];
            GameObject scoreEntry = Instantiate(scorePrefab, content);

            // Set the text to include the index (1-based) and the score
            scoreEntry.GetComponent<Text>().text = $"{highScore} .{i + 1}";
        }
    }
    
    public void ClearHighScores()
    {
        highScores.Clear();
        UpdateHighScoreUI();
    }
    
    public void SaveHighScores()
    {
        string highScoresString = string.Join(",", highScores);
        PlayerPrefs.SetString("HighScores", highScoresString);
        PlayerPrefs.Save();
    }
    
    public void LoadHighScores()
    {
        string highScoresString = PlayerPrefs.GetString("HighScores", "");
        string[] highScoresArray = highScoresString.Split(',');
        highScores.Clear();
        foreach (string scoreString in highScoresArray)
        {
            if (int.TryParse(scoreString, out int score))
            {
                highScores.Add(score);
            }
        }
        UpdateHighScoreUI();
    }
    
    public void DeleteHighScores()
    {
        PlayerPrefs.DeleteKey("HighScores");
        highScores.Clear();
        UpdateHighScoreUI();
    }
    
    private void OnApplicationQuit()
    {
        DeleteHighScores();
    }
    public void ShowHighScores()
    {
        LoadHighScores();
        content.parent.gameObject.SetActive(true);
    }
}