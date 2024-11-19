using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar; // Reference to the progress bar
    public Text progressText; // Reference to the progress text

    void Start()
    {
        // Start the loading process
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        // Get the name of the scene to load (passed from the previous scene)
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad");

        // Start loading the scene asynchronously
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneToLoad);

        // Prevent the scene from activating immediately
        gameLevel.allowSceneActivation = false;

        // While the scene is still loading
        while (gameLevel.progress < 0.9f)
        {
            // Update progress bar and text
            float progress = Mathf.Clamp01(gameLevel.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            // Yield until the next frame
            yield return null;
        }

        // Optionally, wait for some condition to allow the scene to activate
        yield return new WaitForSeconds(1); // Example: wait 1 second

        // Allow the scene to activate
        gameLevel.allowSceneActivation = true;
    }
}
