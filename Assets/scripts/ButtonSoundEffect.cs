using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundEffect : MonoBehaviour
{
    public AudioClip clickSound; // Assign this in the Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
