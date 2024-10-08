using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject[] levelBackgrounds; // Array of level backgrounds
    private int currentLevelIndex;
    private GameObject currentBackground;
    
    public int CurrentLevelIndex => currentLevelIndex; // Add this line to expose the current level index
    
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        // Check if the next level exists
        if (currentLevelIndex < levelBackgrounds.Length)
        {
            LoadLevel(currentLevelIndex);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        // Unload the current background if it exists
        if (currentBackground != null)
        {
            Destroy(currentBackground);
        }

        // Load the new background
        currentBackground= Instantiate(levelBackgrounds[levelIndex], new Vector3(-3.9f, -4.5f, 0), Quaternion.identity);
    }
}