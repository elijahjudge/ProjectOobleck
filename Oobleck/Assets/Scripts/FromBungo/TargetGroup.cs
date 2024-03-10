using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;
    public AnimationCurve fadeRespawnPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
        CharacterTarget.targetAdded += AddPlayer;
    }

    public void AddPlayer(Transform transform)
    {
        targetGroup.AddMember(transform, 1, 2f);
    }


    public void FadePlayerForRespawn(float duration)
    {
        StartCoroutine(GetToRespawnPoint(duration));
    }

    IEnumerator GetToRespawnPoint(float duration)
    {
        float t = 0;
        float lerpPos = 0f;

        while (t < duration)
        {
            lerpPos = t / duration;
            t += Time.deltaTime;

            targetGroup.m_Targets[0].weight = fadeRespawnPlayer.Evaluate(lerpPos);
            
            yield return null;
        }

        targetGroup.m_Targets[0].weight = 1f;
    }
}
