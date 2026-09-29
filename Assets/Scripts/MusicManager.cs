using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip[] music;
    public bool ChoseRandom;
    [field: SerializeField]
    public bool Play { get; private set; }

    AudioSource audioSource;
    List<AudioClip> audioClips;
    int index = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioClips = new List<AudioClip>(music);
        if (ChoseRandom) Shuffle(audioClips);
    }

    public void StartPlayingMusic()
    {
        Play = true;
    }

    public void StopPlaying()
    {
        Play = false;
        audioSource.Stop();
    }

    void Update()
    {
        // play music only if Play is true
        if (!Play) return;

        // do not interrupt 
        if (audioSource.isPlaying) return;

        // play current music and add 1 to index
        audioSource.clip = audioClips[index];
        audioSource.Play();
        index++;

        Debug.Log("Now playing " + audioSource.clip.name);
    }

    void Shuffle(List<AudioClip> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            AudioClip temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}