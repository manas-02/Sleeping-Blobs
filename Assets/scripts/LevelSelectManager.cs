using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public Button[] levelButtons; // Array of level buttons

    void Start()
    {
        foreach (Button levelButton in levelButtons)
        {
            // Find the TextMeshProUGUI component in each button
            TextMeshProUGUI scoreText = levelButton.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();

            if (scoreText != null)
            {
                // Get the level number from the button name
                string buttonName = levelButton.name;
                int levelNumber;

                // Check if the button name can be parsed to an integer
                if (int.TryParse(buttonName.Replace("Level ", ""), out levelNumber))
                {
                    // Load the star count from PlayerPrefs
                    int starCount = PlayerPrefs.GetInt($"Level{levelNumber}Stars", 0);

                    // Update the score text
                    scoreText.text = $"x{starCount}";
                    Debug.Log($"Updated button '{buttonName}' with {starCount} stars.");
                }
                else
                {
                    Debug.LogWarning($"Button name '{buttonName}' does not match expected format.");
                }
            }
            else
            {
                Debug.LogWarning($"ScoreText component not found on button '{levelButton.name}'.");
            }
        }
    }
}
