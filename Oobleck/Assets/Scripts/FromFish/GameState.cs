using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GameState : MonoBehaviour
{
    public ManagerCharacterState mCState;

    public List<GameObject> StartEnable = new List<GameObject>();
    public List<GameObject> StartDisable = new List<GameObject>();
    public List<GameObject> ExitEnable = new List<GameObject>();
    public List<GameObject> ExitDisable = new List<GameObject>();

    public float timeEnteredState;

    public List<Sound> soundsOnStartState = new List<Sound>();
    public List<Sound> durationalAudio = new List<Sound>();
    public float durationalAudioFadeDuration = 1;

    public List<SoundAtSecond> soundAtSeconds = new List<SoundAtSecond>();

    private List<AudioSource> durationalAudioSources = new List<AudioSource>();
    public virtual void OnStart()
    {
        timeEnteredState = Time.time;
        EnableList(StartEnable);
        DisableList(StartDisable);

        foreach(var s in soundsOnStartState)
        {
            ManagerAudio.instance.Play(s);
        }

        foreach (var s in durationalAudio)
        {
            durationalAudioSources.Add(ManagerAudio.instance.Play(s));
        }


        LeanTween.value(gameObject, FadeSound, 0f, 1f, durationalAudioFadeDuration);

    }

    public virtual void TickGameState()
    {
        foreach(var s in soundAtSeconds)
        {
            s.TryPlayingSound(Time.time - timeEnteredState);
        }

    }


    public virtual void OnExit()
    {
        EnableList(ExitEnable);
        DisableList(ExitDisable);

        LeanTween.value(gameObject, FadeSound, 1f, 0f, durationalAudioFadeDuration).setOnComplete(() => DestroyAudioSourceGameObjects());     
    }

    private void DestroyAudioSourceGameObjects()
    {
        int count = durationalAudioSources.Count;

        for (var i = count - 1; i >= 0; i--) 
        {
            Destroy(durationalAudioSources[i].gameObject);
        }

        durationalAudioSources.Clear();
    }
    private void FadeSound(float volume)
    {
        foreach (var s in durationalAudioSources)
        {
            s.volume = volume;
        }
    }
 
    private void EnableList(List<GameObject> enableUs)
    { 
        foreach (GameObject go in enableUs)
        {
            go.SetActive(true);
        }
    }

    private void DisableList(List<GameObject> enableUs)
    {
        foreach (GameObject go in enableUs)
        {
            go.SetActive(false);
        }
    }

    public virtual void ResetGameState()
    {
    }


}

[System.Serializable]
public class SoundAtSecond
{
    public bool played;
    public Sound sound;
    public float seconds;


    public void TryPlayingSound(float timeElapsed)
    {
        if (played)
            return;

        if (timeElapsed > seconds)
        {
            ManagerAudio.instance.Play(sound);
            played = true;

        }
    }
}
