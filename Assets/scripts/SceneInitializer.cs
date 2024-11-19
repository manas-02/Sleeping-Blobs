using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject levelSelectScreen;

    void Start()
    {
        
        if (PlayerPrefs.GetInt("GoToLevelSelect", 0) == 1)
        {
            // Show the level select screen
            startScreen.SetActive(false);
            levelSelectScreen.SetActive(true);

            // Clear the flag
            PlayerPrefs.DeleteKey("GoToLevelSelect");
        }
        else
        {
            // Show the start screen
            startScreen.SetActive(true);
            levelSelectScreen.SetActive(false);
        }
    }
}
