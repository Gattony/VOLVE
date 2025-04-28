using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LevelAudioPlayer_Double : MonoBehaviour
{
    public AudioClip[] musicClips = new AudioClip[2];

    private AudioSource[] audioSources = new AudioSource[2];
    private int currentTrackIndex = 0;
    private int currentSourceIndex = 0;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f; 
    public float targetVolume = 1f;   

    private void Start()
    {
        // Create two AudioSources
        audioSources[0] = GetComponent<AudioSource>();
        audioSources[1] = gameObject.AddComponent<AudioSource>();

        foreach (var src in audioSources)
        {
            src.playOnAwake = false;
            src.loop = false;
            src.volume = 0f;
        }

        if (musicClips.Length < 2 || musicClips[0] == null || musicClips[1] == null)
        {
            Debug.LogWarning("Please assign two AudioClips to LevelAudioPlayer_Double.");
            return;
        }

        StartCoroutine(PlayMusicSequence());
    }

    private IEnumerator PlayMusicSequence()
    {
        // Start playing the first track
        AudioSource activeSource = audioSources[currentSourceIndex];
        activeSource.clip = musicClips[currentTrackIndex];
        activeSource.volume = 0f;
        activeSource.Play();
        yield return StartCoroutine(FadeAudio(activeSource, 0f, targetVolume, fadeDuration));

        while (true)
        {
            // Wait until the clip is almost finished (but not completely)
            yield return new WaitForSeconds(activeSource.clip.length - fadeDuration);

            // Prepare next track
            currentTrackIndex = (currentTrackIndex + 1) % musicClips.Length;
            int nextSourceIndex = 1 - currentSourceIndex;
            AudioSource nextSource = audioSources[nextSourceIndex];

            nextSource.clip = musicClips[currentTrackIndex];
            nextSource.volume = 0f;
            nextSource.Play();

            // Crossfade
            StartCoroutine(FadeAudio(activeSource, targetVolume, 0f, fadeDuration));
            StartCoroutine(FadeAudio(nextSource, 0f, targetVolume, fadeDuration));

            yield return new WaitForSeconds(fadeDuration);

            // After crossfade is done, stop the old source
            activeSource.Stop();

            currentSourceIndex = nextSourceIndex;
            activeSource = audioSources[currentSourceIndex];
        }
    }

    private IEnumerator FadeAudio(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        source.volume = endVolume;
    }
}
