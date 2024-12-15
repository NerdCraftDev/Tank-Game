using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip[] loopedMusic; // Array to hold your four MP3 files
    private int currentTrackIndex = 0;

    void Start()
    {
        currentTrackIndex = Random.Range(0, loopedMusic.Length);
        PlayLoopedMusic();
    }

    void PlayLoopedMusic()
    {
        if (loopedMusic.Length > 0)
        {
            musicSource.loop = false;
            musicSource.clip = loopedMusic[currentTrackIndex];
            musicSource.Play();
        }
    }

    void Update()
    {
        // Check if the current music track has finished playing and loop to the next track
        if (!musicSource.isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % loopedMusic.Length;
            PlayLoopedMusic();
        }
    }
}
