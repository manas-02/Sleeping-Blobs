using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonManager : MonoBehaviour
{
    // Method to quit to the Start Screen (in the "initiation" scene)
    public void QuitToStartScreen()
    {
        // Clear the flag before loading the scene
        PlayerPrefs.DeleteKey("GoToLevelSelect");
        SceneManager.LoadScene("initiation");
    }
    
    // Method to go to the Level Select Screen (in the "initiation" scene)
    public void GoToLevelSelectScreen()
    {
        // Set a flag to indicate that we want to go directly to the Level Select screen
        PlayerPrefs.SetInt("GoToLevelSelect", 1);
        SceneManager.LoadScene("initiation");
    }
    
    // Method to replay the current level
    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
