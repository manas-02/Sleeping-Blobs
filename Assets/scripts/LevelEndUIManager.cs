using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelEndUIManager : MonoBehaviour
{
    public GameObject levelFailedImage; // Assign in Inspector
    public GameObject levelWonImage;    // Assign in Inspector
    public GameObject replayButton;     // Assign in Inspector
    public GameObject levelSelectButton; // Assign in Inspector
    public GameObject quitButton;       // Assign in Inspector

    public DrawingHandler drawingHandler; // Reference to the DrawingHandler script

    void Start()
    {
        // Ensure images and buttons are hidden at the start
        if (levelFailedImage != null) levelFailedImage.SetActive(false);
        if (levelWonImage != null) levelWonImage.SetActive(false);
        if (replayButton != null) replayButton.SetActive(false);
        if (levelSelectButton != null) levelSelectButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        if (drawingHandler == null)
        {
            Debug.LogError("DrawingHandler reference is not assigned.");
        }
    }

    public void ShowLevelFailed()
    {
        if (levelFailedImage != null) levelFailedImage.SetActive(true);
        
        // Stop drawing when the level fails
        if (drawingHandler != null)
        {
            drawingHandler.SetGameOver(true);
        }

        StartCoroutine(ShowButtonsAfterDelay());
    }

    public void ShowLevelWon()
    {
        if (levelWonImage != null) levelWonImage.SetActive(true);
        
        // Stop drawing when the level is won
        if (drawingHandler != null)
        {
            drawingHandler.SetGameOver(true);
        }

        StartCoroutine(ShowButtonsAfterDelay());
    }

    private IEnumerator ShowButtonsAfterDelay()
    {
        // Wait for 1 second before showing the buttons
        yield return new WaitForSeconds(1f);

        if (replayButton != null) replayButton.SetActive(true);
        if (levelSelectButton != null) levelSelectButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);
    }
}

