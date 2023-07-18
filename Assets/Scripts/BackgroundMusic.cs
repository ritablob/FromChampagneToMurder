using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public List<AudioClip> musicTracks;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextTrack();
    }

    private void PlayNextTrack()
    {
        audioSource.Stop();
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();

        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
        StartCoroutine(WaitForTrackToEnd());
    }

    private IEnumerator WaitForTrackToEnd()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        PlayNextTrack();
    }
}
