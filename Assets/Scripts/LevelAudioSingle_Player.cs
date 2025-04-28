using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LevelAudioSingle_Player : MonoBehaviour
{
    public AudioClip musicClip;

    private AudioSource audioSource;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f; // Duration of fade in and fade out
    public float targetVolume = 1f;   // The normal max volume

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicClip == null)
        {
            Debug.LogWarning("Please assign an AudioClip to LevelAudioPlayer_Single.");
            return;
        }

        StartCoroutine(PlayMusicLoop());
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            audioSource.clip = musicClip;
            audioSource.volume = 0f;
            audioSource.Play();

            // Fade in
            yield return StartCoroutine(FadeAudio(0f, targetVolume, fadeDuration));

            // Wait for the clip minus fade out time
            yield return new WaitForSeconds(audioSource.clip.length - fadeDuration);

            // Fade out
            yield return StartCoroutine(FadeAudio(targetVolume, 0f, fadeDuration));

            // Clip finished, start again
        }
    }

    private IEnumerator FadeAudio(float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = endVolume;
    }
}
