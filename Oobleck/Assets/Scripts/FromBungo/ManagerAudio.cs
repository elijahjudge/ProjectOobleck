using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ManagerAudio : MonoBehaviour
{
    public AnimationCurve soundFading;

    public static ManagerAudio instance = null;
    private GameObject soundEffectHolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            soundEffectHolder = new GameObject();
            soundEffectHolder.name = "Holder Sound";

            DontDestroyOnLoad(instance);
            DontDestroyOnLoad(soundEffectHolder);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    public AudioSource Play(Sound sound)
    {
        if(sound.soundVariables.playOrder == SoundVariables.PlayOrder.PlayAllAtSameTime)
        {
            PlayAll(sound, sound.clips);
            return null;
        }

        GameObject soundPlayer = new GameObject(sound.name);
        soundPlayer.transform.parent = soundEffectHolder.transform;

        /*if (EditorApplication.isPlaying)
        {
        }
        else
        {
        }
#if UNITY_EDITOR
            soundPlayer.transform.parent = GameObject.Find("Audio Trash - Delete after testing").transform;
#endif
        */

        AudioSource source = soundPlayer.AddComponent<AudioSource>();
        int index = GetIndexOfCLipToPlay(sound,sound.soundVariables);
        source.clip = sound.clips[index];
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.volume = Random.Range(sound.soundVariables.volumeRange.x, sound.soundVariables.volumeRange.y);
        source.pitch = Random.Range(sound.soundVariables.pitchRange.x, sound.soundVariables.pitchRange.y);
        source.loop = sound.soundVariables.looping;
        source.Play();

        if (!sound.soundVariables.looping)
        {
            //if (EditorApplication.isPlaying)

            Destroy(soundPlayer, source.clip.length);
        }

        return source;
    }

    public AudioSource Play3D(Sound sound, Vector3 position)
    {
        if (sound.soundVariables.playOrder == SoundVariables.PlayOrder.PlayAllAtSameTime)
        {
            PlayAll(sound, sound.clips);
            return null;
        }

        GameObject soundPlayer = new GameObject(sound.name);
        soundPlayer.transform.parent = soundEffectHolder.transform;
        AudioSource source = soundPlayer.AddComponent<AudioSource>();
        int index = GetIndexOfCLipToPlay(sound, sound.soundVariables);
        source.clip = sound.clips[index];
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.volume = Random.Range(sound.soundVariables.volumeRange.x, sound.soundVariables.volumeRange.y);
        source.pitch = Random.Range(sound.soundVariables.pitchRange.x, sound.soundVariables.pitchRange.y);
        source.loop = sound.soundVariables.looping;
        //source.PlayClipAtPoint(source.clip, position);
        AudioSource.PlayClipAtPoint(source.clip, position);

        if (!sound.soundVariables.looping)
        {
            //if (EditorApplication.isPlaying)

            Destroy(soundPlayer, source.clip.length);
        }

        return source;
    }

    public AudioSource PlayRandomClip(List<Sound> sounds)
    {
        int index = Random.Range(0, sounds.Count);

        return Play(sounds[index]);
    }

    /// <summary>
    /// you can ignore adding mixer to soundvariable it will never change from original sound
    /// </summary>
    public AudioSource Play(Sound sound, SoundVariables soundVariables)
    {
        if (sound.soundVariables.playOrder == SoundVariables.PlayOrder.PlayAllAtSameTime)
        {
            PlayAll(sound, sound.clips);
            return null;
        }

        GameObject soundPlayer = new GameObject(sound.name);
        soundPlayer.transform.parent = soundEffectHolder.transform;
        AudioSource source = soundPlayer.AddComponent<AudioSource>();
        int index = GetIndexOfCLipToPlay(sound,soundVariables);
        source.clip = sound.clips[index];
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.volume = Random.Range(soundVariables.volumeRange.x, soundVariables.volumeRange.y);
        source.pitch = Random.Range(soundVariables.pitchRange.x, soundVariables.pitchRange.y);
        source.loop = soundVariables.looping;
        source.Play();

        if (!soundVariables.looping)
            Destroy(soundPlayer, source.clip.length);

        return source;
    }
    public void PlayAll(Sound sound, List<AudioClip> clips)
    {
        foreach (AudioClip item in clips)
        {
            GameObject soundPlayer = new GameObject(sound.name);

#if UNITY_EDITOR
                        soundPlayer.transform.parent = GameObject.Find("Audio Trash - Delete after testing").transform;
#else
                            soundPlayer.transform.parent = soundEffectHolder.transform;
#endif

            AudioSource source = soundPlayer.AddComponent<AudioSource>();
            int index = GetIndexOfCLipToPlay(sound, sound.soundVariables);
            source.clip = item;
            source.outputAudioMixerGroup = sound.mixerGroup;
            source.volume = Random.Range(sound.soundVariables.volumeRange.x, sound.soundVariables.volumeRange.y);
            source.pitch = Random.Range(sound.soundVariables.pitchRange.x, sound.soundVariables.pitchRange.y);
            source.loop = sound.soundVariables.looping;
            source.Play();

#if !UNITY_EDITOR
                                    Destroy(soundPlayer, source.clip.length);
#endif
        }
    }

    public void FadeSound(AudioSource source, float from, float to, float duration,bool destroyAfter)
    {
        if (source == null)
            return;

        StartCoroutine(SoundFading(duration, source, from, to,destroyAfter));
    }

    IEnumerator SoundFading(float duration, AudioSource source, float from, float to,bool destroyAfter)
    {
        float t = 0;
        while(t < duration)
        {
            if (source == null)
                yield break;

            source.volume = Mathf.Lerp(from,to,soundFading.Evaluate(t));
            t += Time.deltaTime;
            yield return null;
        }

        if(destroyAfter)
        {
            if (source != null)
                Destroy(source.gameObject);
        }
    }

    private int GetIndexOfCLipToPlay(Sound sound, SoundVariables soundVariables)
    {
        int index = 0;

        switch (soundVariables.playOrder)
        {
            case SoundVariables.PlayOrder.PlayFirst:
                break;
            case SoundVariables.PlayOrder.Random:
                index = Random.Range(0, sound.clips.Count);
                break;
            case SoundVariables.PlayOrder.InOrder:
                index = soundVariables.currentIndex;
                soundVariables.currentIndex += 1;
                soundVariables.currentIndex %= sound.clips.Count;
                break;
            case SoundVariables.PlayOrder.PlayAllAtSameTime:
                index = 111;
                break;

        }

        return index;
    }
    
}
