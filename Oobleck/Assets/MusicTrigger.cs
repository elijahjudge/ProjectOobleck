using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public Bounds boundsMusicPlay;
    public Bounds boundsMusicStop;

    public Sound music;
    public float fadeInDuration;
    public float fadeOutDuration;
    public float musicVolume;

    private AudioSource source;

    public Transform player;
    public delegate void CheckpointEvent(Vector3 position, int checkPoint);

    public static CheckpointEvent checkPointTouched;

    private bool musicPlaying;
    private void Start()
    {
        boundsMusicPlay.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (boundsMusicPlay.Contains(player.position) && !musicPlaying)
        {
            musicPlaying = true;
            source = ManagerAudio.instance.Play(music);
            source.volume = 0f;
            LeanTween.value(gameObject, fadeVolume, source.volume, musicVolume, fadeInDuration);

        }

        if (boundsMusicStop.Contains(player.position) && musicPlaying)
        {
            musicPlaying = false;
            LeanTween.value(gameObject, fadeVolume, musicVolume, 0f, fadeOutDuration).setOnComplete(()=>Destroy(source.gameObject));
        }

    }

    private void fadeVolume(float volume)
    {
        source.volume = volume;
    }
    private void OnDrawGizmosSelected()
    {

        boundsMusicPlay.center = transform.position;
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(boundsMusicPlay.center, boundsMusicPlay.size);

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(boundsMusicStop.center, boundsMusicStop.size);


    }

    
}
