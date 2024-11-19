using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health value
    public float currentHealth;
    public Image healthBarFill; // Reference to the Health Bar Fill Image
    public float timeInMinutes = 1f; // Time in minutes for health to reach zero
    public BlobSpawner blobSpawner; // Reference to the BlobSpawner script
    public Animator mommyBlobAnimator; // Reference to the Animator component for the Mommy Blob
    public AudioSource bgmAudioSource; // Reference to the AudioSource for BGM
    public AudioClip winBgm; // Background music for winning
    public AudioClip loseBgm; // Background music for losing

    public GameObject[] starSprites; // Array of star sprites to display in the scene
    public Text[] starTextLabels; // Array of text labels to show the number of stars achieved

    private float healthDecreaseRate; // Calculated health decrease rate
    private bool gameEnded = false; // Flag to check if the game has ended

    void Start()
    {
        currentHealth = maxHealth;
        CalculateHealthDecreaseRate();
        UpdateHealthBar();
        UpdateMommyBlobAnimation();
        
        
        SetStarSpritesActive(false);

        
        if (bgmAudioSource != null)
        {
            bgmAudioSource.loop = true; 
            bgmAudioSource.Play();
        }
    }

    void Update()
    {
        if (gameEnded) return; // Stop decreasing health if the game has ended

        // Decrease health based on time
        currentHealth -= healthDecreaseRate * Time.deltaTime;

        // Clamp health to be between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        UpdateMommyBlobAnimation(); // Update the animation based on current health

        // Check if health is depleted
        if (currentHealth <= 0)
        {
            HandleHealthDepletion();
        }
    }

    void CalculateHealthDecreaseRate()
    {
        // Convert time in minutes to seconds
        float timeInSeconds = timeInMinutes * 60f;

        // Calculate the decrease rate per second
        healthDecreaseRate = maxHealth / timeInSeconds;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void UpdateMommyBlobAnimation()
    {
        if (mommyBlobAnimator != null)
        {
           
            mommyBlobAnimator.SetFloat("Health", currentHealth);
        }
    }

    void HandleHealthDepletion()
    {
        
        gameEnded = true; // Set gameEnded to true
        blobSpawner.GameOver();
        ChangeBgm(loseBgm); // Play the lose BGM
        PlayLoseAnimation(); // Play the lose animation
        SaveStarCount(); // Save the star count

        
        UpdateStarSprites();
    }

    public void GameWon()
    {
        Debug.Log("Level Complete: Player put all blobs to sleep before mommy blob's health reached zero.");
        gameEnded = true; // Stop the health bar from decreasing
        ChangeBgm(winBgm); // Play the win BGM
        PlayWinAnimation(); // Play the win animation
        SaveStarCount(); // Save the star count

        // Update star sprites after the level ends
        UpdateStarSprites();
    }

    void ChangeBgm(AudioClip newClip)
    {
        if (bgmAudioSource != null && newClip != null)
        {
            bgmAudioSource.Stop(); // Stop the current BGM
            bgmAudioSource.clip = newClip; // Set the new BGM clip
            bgmAudioSource.loop = false; // Disable looping for win/lose BGM
            bgmAudioSource.Play(); // Play the new BGM
        }
    }

    void PlayLoseAnimation()
    {
        if (mommyBlobAnimator != null)
        {
            mommyBlobAnimator.SetTrigger("Lose"); // Set the trigger for losing animation
        }
    }

    void PlayWinAnimation()
    {
        if (mommyBlobAnimator != null)
        {
            mommyBlobAnimator.SetTrigger("Win"); // Set the trigger for winning animation
        }
    }

    void SaveStarCount()
    {
        // Calculate star count based on current health
        int starCount = 0;
        if (currentHealth > 70)
            starCount = 3;
        else if (currentHealth > 40)
            starCount = 2;
        else if (currentHealth > 0)
            starCount = 1;

        // Get the level number
        int levelNumber = GetCurrentLevelNumber(); 

        // Save the star count to PlayerPrefs
        PlayerPrefs.SetInt($"Level{levelNumber}Stars", starCount);

        // Print the star count to the debug log
        Debug.Log($"Level {levelNumber}: {starCount} stars saved.");
    }

    int GetCurrentLevelNumber()
    {
        
        return SceneManager.GetActiveScene().buildIndex;
    }

    void UpdateStarSprites()
    {
        // Determine the number of stars earned
        int starCount = 0;
        if (currentHealth > 70)
            starCount = 3;
        else if (currentHealth > 40)
            starCount = 2;
        else if (currentHealth > 0)
            starCount = 1;

       
        SetStarSpritesActive(true); // Ensure stars are active
        for (int i = 0; i < starSprites.Length; i++)
        {
            starSprites[i].SetActive(i < starCount);
        }

        // Update star text labels
        UpdateStarTextLabels(starCount);
    }

    void SetStarSpritesActive(bool isActive)
    {
        foreach (var star in starSprites)
        {
            star.SetActive(isActive);
        }
    }

    void UpdateStarTextLabels(int starCount)
    {
        // Ensure text labels array is set and has the correct number of elements
        if (starTextLabels != null && starTextLabels.Length > 0)
        {
            foreach (var textLabel in starTextLabels)
            {
                textLabel.text = $"x{starCount}";
            }
        }
    }
}
