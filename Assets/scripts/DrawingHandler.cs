using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingHandler : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Vector2> drawnPoints = new List<Vector2>();
    public RectTransform zImageRectTransform; // Reference to the RectTransform of the Z Image
    public BlobSpawner blobSpawner; // Reference to the BlobSpawner script
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip correctSound; // Sound when the Z shape is correct
    // Remove the wrongSound variable
    // public AudioClip wrongSound; // Sound when the Z shape is wrong
    public Color correctColor = Color.green; // Color when the Z shape is correct
    public Color wrongColor = Color.red; // Color when the Z shape is wrong

    private List<Vector2> zPathPoints = new List<Vector2>
    {
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(0f, 0f),
        new Vector2(1f, 0f)
    };

    private float similarityThreshold = 0.7f; // 70% similarity
    private float maxSegmentError = 0.1f; // Maximum allowable error for segment match
    private bool isGameOver = false; // To track if the game is won or lost

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned. Please assign it in the Inspector.");
        }

        if (zImageRectTransform == null)
        {
            Debug.LogError("zImageRectTransform is not assigned. Please assign it in the Inspector.");
        }

        if (blobSpawner == null)
        {
            Debug.LogError("BlobSpawner is not assigned. Please assign it in the Inspector.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned. Please assign it in the Inspector.");
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        if (isGameOver)
            return; // Stop drawing if the game is over

        if (Input.GetMouseButtonDown(0))
        {
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
            lineRenderer.startColor = Color.white; // Reset color to white
            lineRenderer.endColor = Color.white;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 localMousePosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(zImageRectTransform, Input.mousePosition, Camera.main, out localMousePosition))
            {
                if (IsPointInsideRect(localMousePosition, zImageRectTransform))
                {
                    drawnPoints.Add(localMousePosition);
                    lineRenderer.positionCount = drawnPoints.Count;
                    Vector3 worldPosition = zImageRectTransform.TransformPoint(localMousePosition);
                    lineRenderer.SetPosition(drawnPoints.Count - 1, worldPosition);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (IsZShape(drawnPoints, zPathPoints, similarityThreshold))
            {
                // Shape is correct
                Debug.Log("Blob put to sleep.");
                lineRenderer.startColor = correctColor;
                lineRenderer.endColor = correctColor;
                audioSource.PlayOneShot(correctSound);

                // Stop the current blob's movement and tag the next blob
                blobSpawner.PutTaggedBlobToSleep();
                blobSpawner.TagNextBlob();
            }
            else
            {
                // Shape is incorrect
                Debug.Log("Try again.");
                lineRenderer.startColor = wrongColor;
                lineRenderer.endColor = wrongColor;
                // Remove the code to play the wrong sound
                // audioSource.PlayOneShot(wrongSound);
            }

            // Delay before clearing the drawing to show the color
            StartCoroutine(ClearDrawingAfterDelay(0.5f));
        }
    }

    private IEnumerator ClearDrawingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Clear the drawing after the delay
        drawnPoints.Clear();
        lineRenderer.positionCount = 0;
    }

    bool IsPointInsideRect(Vector2 point, RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        Vector2 localPoint = point - (Vector2)rectTransform.position;

        return localPoint.x >= rect.xMin && localPoint.x <= rect.xMax &&
               localPoint.y >= rect.yMin && localPoint.y <= rect.yMax;
    }

    bool IsZShape(List<Vector2> drawnPoints, List<Vector2> zPathPoints, float threshold)
    {
        if (drawnPoints.Count < 2) return false;

        List<Vector2> normalizedDrawnPoints = NormalizePoints(drawnPoints);
        List<Vector2> normalizedZPathPoints = NormalizePoints(zPathPoints);

        return CompareZShape(normalizedDrawnPoints, normalizedZPathPoints, threshold);
    }

    List<Vector2> NormalizePoints(List<Vector2> points)
    {
        float minX = Mathf.Min(points.ConvertAll(p => p.x).ToArray());
        float minY = Mathf.Min(points.ConvertAll(p => p.y).ToArray());
        float maxX = Mathf.Max(points.ConvertAll(p => p.x).ToArray());
        float maxY = Mathf.Max(points.ConvertAll(p => p.y).ToArray());

        List<Vector2> normalizedPoints = new List<Vector2>();

        foreach (var point in points)
        {
            float normalizedX = (point.x - minX) / (maxX - minX);
            float normalizedY = (point.y - minY) / (maxY - minY);
            normalizedPoints.Add(new Vector2(normalizedX, normalizedY));
        }

        return normalizedPoints;
    }

    bool CompareZShape(List<Vector2> drawnPoints, List<Vector2> zPathPoints, float threshold)
    {
        int matchingPoints = 0;
        int totalPoints = zPathPoints.Count;

        for (int i = 0; i < totalPoints; i++)
        {
            Vector2 zPoint = zPathPoints[i];

            foreach (var drawnPoint in drawnPoints)
            {
                if (Vector2.Distance(drawnPoint, zPoint) <= maxSegmentError)
                {
                    matchingPoints++;
                    break;
                }
            }
        }

        float similarity = (float)matchingPoints / totalPoints;
        return similarity >= threshold;
    }

    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;

        // Clear the current drawing to indicate the end of the game
        drawnPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}
