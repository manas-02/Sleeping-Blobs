using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject levelSelectCanvas;
    public GameObject instructionCanvas; // Reference to the InstructionCanvas

    void Start()
    {
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        startCanvas.SetActive(true);
        levelSelectCanvas.SetActive(false);
        instructionCanvas.SetActive(false); // Hide instructions initially
    }

    public void ShowLevelSelectScreen()
    {
        startCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);
        instructionCanvas.SetActive(false); // Hide instructions when selecting a level
    }

    public void ShowInstructionScreen()
    {
        startCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
        instructionCanvas.SetActive(true); // Show the instructions
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}

