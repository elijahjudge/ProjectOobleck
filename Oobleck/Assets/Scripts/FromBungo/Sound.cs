using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound_", menuName = "ScriptableObjects/Sound")]

public class Sound : ScriptableObject
{
    [Header("Sounds")]
    public List<AudioClip> clips;

    [Header("Mixer Info")]
    public AudioMixerGroup mixerGroup;

    [Header("Sound Variables")]
    public SoundVariables soundVariables;
}

[System.Serializable]
public class SoundVariables
{
    [Header("Looping ?")]
    public bool looping;

    [Header("Variation")]
    public Vector2 volumeRange = new Vector2(1f,1f);
    public Vector2 pitchRange = new Vector2(1f,1f);

    [Header("Play Order")]
    public PlayOrder playOrder;

    [HideInInspector] public int currentIndex;
    public enum PlayOrder
    {
        PlayFirst,
        Random,
        InOrder,
        PlayAllAtSameTime
    }

}
