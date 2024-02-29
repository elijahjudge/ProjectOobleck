using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


#if UNITY_EDITOR

[CustomEditor(typeof(Sound))]
public class EditorSound : Editor
{
    GameObject holder = null;
    ManagerAudio audioManagerTestInstance = null;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Sound inspected = (Sound)target;
        CreateHiddenTestSoundHolder();

        GUILayout.Space(50f);

        if (GUILayout.Button("Test Sound"))
        {
            TestSound(inspected);
        }

        GUILayout.Space(50f);
        if (GUILayout.Button("Stop Sounds"))
        {
            DestroyImmediate(holder);

        }

    }

    private void OnDisable()
    {
        DestroyImmediate(holder);
        audioManagerTestInstance = null;
    }
    private void CreateHiddenTestSoundHolder()
    {
        holder = GameObject.Find("Audio Trash - Delete after testing");

        if (holder == null)
        {
            holder = new GameObject("Audio Trash - Delete after testing");
            audioManagerTestInstance = holder.AddComponent<ManagerAudio>();
        }

        holder.hideFlags = HideFlags.HideAndDontSave;
    }

    private void TestSound(Sound inspected)
    {
        if(inspected.clips.Count == 0)
        {
            Debug.LogWarning("You need to add a sound clip bozo!");
            return;
        }
        if (inspected.mixerGroup == null)
        {
            Debug.LogWarning("You need to add mixer group to the sound bozo!");
            return;
        }

        audioManagerTestInstance.Play(inspected);
        return;

        /*GameObject go = new GameObject();
        go.transform.parent = holder.transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = inspected.clip;
        source.outputAudioMixerGroup = inspected.mixerGroup;
        source.volume = Random.Range(inspected.soundVariables.volumeRange.x, inspected.soundVariables.volumeRange.y);
        source.pitch = Random.Range(inspected.soundVariables.pitchRange.x, inspected.soundVariables.pitchRange.y);
        source.loop = inspected.soundVariables.looping;
        source.Play();
        */
    }
}

#endif