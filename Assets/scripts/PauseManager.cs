using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // The menu UI Image to be shown when the game is paused
    public Image pauseButtonImage; // Reference to the Image component of the pause button
    public Sprite pauseSprite; // The sprite for the pause button
    public Sprite crossSprite; // The sprite for the cross button
    private bool isPaused = false; // To track the paused state

    void Start()
    {
        // Ensure the pause menu is hidden at the start
        pauseMenu.SetActive(false);

        // Ensure the pause button image is set to the pause sprite initially
        if (pauseButtonImage != null)
        {
            pauseButtonImage.sprite = pauseSprite;
        }
        else
        {
            Debug.LogError("PauseButtonImage is not assigned. Please assign it in the Inspector.");
        }

        if (pauseSprite == null || crossSprite == null)
        {
            Debug.LogError("PauseSprite or CrossSprite is not assigned. Please assign them in the Inspector.");
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Freeze the game
        pauseMenu.SetActive(true); // Show the pause menu
        if (pauseButtonImage != null)
        {
            pauseButtonImage.sprite = crossSprite; // Change button sprite to cross
        }
        isPaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        pauseMenu.SetActive(false); // Hide the pause menu
        if (pauseButtonImage != null)
        {
            pauseButtonImage.sprite = pauseSprite; // Change button sprite back to pause
        }
        isPaused = false;
    }
}
