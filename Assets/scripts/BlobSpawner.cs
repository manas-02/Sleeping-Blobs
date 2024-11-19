using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlobSpawner : MonoBehaviour
{
    public LevelEndUIManager levelEndUIManager;
    public GameObject[] blobPrefabs; // Array of blob prefabs
    public int numberOfBlobs = 5; // Number of blobs to spawn
    public float minSpeed = 1.0f; // Minimum speed of blobs
    public float maxSpeed = 5.0f; // Maximum speed of blobs
    public BoxCollider2D boundaryCollider; // Reference to the boundary collider
    public Vector3 blobScale = new Vector3(1f, 1f, 1f); // Desired scale for blobs
    public GameObject redTrianglePrefab; // Reference to the red triangle prefab
    public HealthManager healthManager; // Reference to the HealthManager script
    public float spawnInterval = 1.0f; // Time interval between spawning blobs
    public Slider blobProgressSlider; // Reference to the UI Slider for blob progression
    public AudioSource blobAudioSource; // Reference to the AudioSource for playing sound effects
    public AudioClip blobSpawnClip; // The audio clip to play when each blob is spawned
    public float spawnSoundVolume = 1.0f; // Volume of the spawn sound effect

    private List<GameObject> blobs = new List<GameObject>();
    private int currentBlobIndex = -1; // Start tagging from no blob
    private GameObject redTriangle;
    private int blobsPutToSleep = 0;
    private int blobsSpawned = 0; // Track the number of blobs spawned
    private bool isTaggingInProgress = false; // Flag to track if tagging is in progress

    void Start()
    {
        Debug.Log("Current Time.timeScale: " + Time.timeScale);
        Time.timeScale = 1;
        if (boundaryCollider == null)
        {
            Debug.LogError("Boundary Collider is not assigned.");
            return;
        }

        if (healthManager == null)
        {
            Debug.LogError("HealthManager is not assigned.");
            return;
        }

        if (redTrianglePrefab == null)
        {
            Debug.LogError("Red Triangle Prefab is not assigned.");
            return;
        }

        if (blobPrefabs == null || blobPrefabs.Length == 0)
        {
            Debug.LogError("Blob Prefabs are not assigned.");
            return;
        }

        if (blobAudioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }

        // Set the slider max value to the total number of blobs
        if (blobProgressSlider != null)
        {
            blobProgressSlider.maxValue = numberOfBlobs;
            blobProgressSlider.value = 0; // Start at 0
        }

        // Instantiate the red triangle but keep it inactive initially
        redTriangle = Instantiate(redTrianglePrefab);
        redTriangle.SetActive(false);

        // Start the coroutine to spawn blobs gradually
        StartCoroutine(SpawnBlobsGradually());
    }

    IEnumerator SpawnBlobsGradually()
    {
        for (int i = 0; i < numberOfBlobs; i++)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            GameObject blobPrefab = blobPrefabs[Random.Range(0, blobPrefabs.Length)];
            GameObject blob = Instantiate(blobPrefab, spawnPosition, Quaternion.identity);

            // Set the scale for the blob
            blob.transform.localScale = blobScale;

            Rigidbody2D rb = blob.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0; // Ensure no gravity affects blobs
                rb.velocity = GetRandomDirection() * Random.Range(minSpeed, maxSpeed);
            }

            blobs.Add(blob);
            blobsSpawned++; // Increment the number of blobs spawned

            // Update the slider value to reflect the current number of blobs spawned
            if (blobProgressSlider != null)
            {
                blobProgressSlider.value = blobsSpawned;
            }

            // Tag the first blob immediately
            if (i == 0)
            {
                currentBlobIndex = 0;
                TagNextBlob();
            }

            // Play the spawn sound effect
            if (blobAudioSource != null && blobSpawnClip != null)
            {
                blobAudioSource.PlayOneShot(blobSpawnClip, spawnSoundVolume);
            }

            // Wait for the specified interval before spawning the next blob
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        if (boundaryCollider == null)
        {
            Debug.LogError("Boundary Collider is not assigned.");
            return Vector2.zero;
        }

        Bounds boundaryBounds = boundaryCollider.bounds;

        int edge = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;

        switch (edge)
        {
            case 0: // Top edge
                spawnPosition = new Vector2(Random.Range(boundaryBounds.min.x, boundaryBounds.max.x), boundaryBounds.max.y);
                break;
            case 1: // Bottom edge
                spawnPosition = new Vector2(Random.Range(boundaryBounds.min.x, boundaryBounds.max.x), boundaryBounds.min.y);
                break;
            case 2: // Left edge
                spawnPosition = new Vector2(boundaryBounds.min.x, Random.Range(boundaryBounds.min.y, boundaryBounds.max.y));
                break;
            case 3: // Right edge
                spawnPosition = new Vector2(boundaryBounds.max.x, Random.Range(boundaryBounds.min.y, boundaryBounds.max.y));
                break;
        }

        return spawnPosition;
    }

    Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y).normalized; // Ensure direction is normalized
    }

    public void PutTaggedBlobToSleep()
    {
        if (currentBlobIndex >= 0 && currentBlobIndex < blobs.Count && !isTaggingInProgress)
        {
            isTaggingInProgress = true;

            GameObject blob = blobs[currentBlobIndex];
            if (blob != null)
            {
                Rigidbody2D rb = blob.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero; // Stop the blob's movement
                }

                // Play sleep animation
                Animator blobAnimator = blob.GetComponent<Animator>();
                if (blobAnimator != null)
                {
                    blobAnimator.SetTrigger("SleepTrigger");
                }

                blobsPutToSleep++;
                CheckLevelCompletion();

                // After the current blob is put to sleep, move to the next one
                StartCoroutine(WaitForSleepAnimationAndTagNext());
            }
        }
    }

    IEnumerator WaitForSleepAnimationAndTagNext()
    {
        // Wait for a short time to ensure the sleep animation has time to play
        yield return new WaitForSeconds(0.5f);

        // Move to the next blob to tag
        currentBlobIndex++;
        Debug.Log("Current Blob Index: " + currentBlobIndex);
        if (currentBlobIndex < blobs.Count)
        {
            TagNextBlob(); // Tag the next blob
        }
        else
        {
            // No more blobs to tag
            redTriangle.SetActive(false);
            Debug.Log("Level Complete: All blobs have been tagged and put to sleep.");
            healthManager.GameWon();
        }

        isTaggingInProgress = false; // Allow tagging of the next blob
    }

    public void TagNextBlob()
    {
        if (currentBlobIndex >= 0 && currentBlobIndex < blobs.Count)
        {
            redTriangle.SetActive(true);
            redTriangle.transform.SetParent(blobs[currentBlobIndex].transform);
            redTriangle.transform.localPosition = new Vector3(0f, 0.15f, 0f); // Adjust the position above the blob
        }
    }

    void CheckLevelCompletion()
    {
        if (blobsPutToSleep >= numberOfBlobs)
        {
            Debug.Log("Level Complete: All blobs have been put to sleep.");
            if (levelEndUIManager != null)
            {
                levelEndUIManager.ShowLevelWon();
            }
            healthManager.GameWon();
            StopBlobSounds(); // Stop the blob sounds when the level is completed
        }
    }

    public void GameOver()
    {
        Debug.Log("Level Failed: Mommy blob's health reached zero.");
        if (levelEndUIManager != null)
        {
            levelEndUIManager.ShowLevelFailed();
        }
    }

    private void StopBlobSounds()
    {
        if (blobAudioSource != null)
        {
            blobAudioSource.Stop(); // Stop all audio playback
        }
    }
}
