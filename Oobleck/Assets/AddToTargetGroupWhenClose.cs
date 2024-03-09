using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddToTargetGroupWhenClose : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;

    public float targetWeight = .1f;

    public float RadiusRequiredForTargetGroup;
    private float RemoveLeeway = 5f;

    public AnimationCurve targetFadeIn;
    public float fadeInDuration;
    public float allowFadeOut_InXXXSeconds;

    public AnimationCurve targetFadeOut;
    public float fadeOutDuration;
    public float allowFadeIn_InXXXSeconds;

    private int targetIndex;
    private float timeTargetAdded;
    private float timeTargetRemoved;


    private Coroutine fadingIn = null;
    private Coroutine fadingOut = null;
    void FixedUpdate()
    {
        if(Vector3.Distance(targetGroup.transform.position,transform.position) < RadiusRequiredForTargetGroup
            && (Time.time - timeTargetRemoved) > allowFadeIn_InXXXSeconds)
        {
            AddToTargetGroup();
        }
        else if(Vector3.Distance(targetGroup.transform.position, transform.position) > RadiusRequiredForTargetGroup + RemoveLeeway
            && (Time.time - timeTargetAdded) > allowFadeOut_InXXXSeconds)
        {
            RemoveFromTargetGroup();
        }
    }

    public void AddToTargetGroup()
    {
        if (targetGroup.FindMember(transform) == -1 && fadingIn == null && fadingOut == null)
            fadingIn = StartCoroutine(FadeCameraIn(fadeInDuration));              
    }

    public void RemoveFromTargetGroup()
    {
        if (targetGroup.FindMember(transform) != -1 && fadingOut == null && fadingIn == null)
            fadingOut = StartCoroutine(FadeCameraOut(fadeInDuration));
    }

    IEnumerator FadeCameraIn(float duration)
    {
        timeTargetAdded = Time.time;
        targetGroup.AddMember(transform, 0f, 10f);
        targetIndex = targetGroup.FindMember(transform);

        float t = 0f;
        float x = 0f;

        while(t < duration)
        {
            x = t / duration;
            t += Time.deltaTime;

            if(targetGroup.m_Targets.Length > targetIndex)
                targetGroup.m_Targets[targetIndex].weight = targetFadeIn.Evaluate(x) * targetWeight;

            yield return null;
        }

        fadingIn = null;
    }

    IEnumerator FadeCameraOut(float duration)
    {
        timeTargetRemoved = Time.time;
        targetIndex = targetGroup.FindMember(transform);

        if (targetIndex == -1)
            yield break;

        float t = 0f;
        float x = 0f;

        while (t < duration)
        {
            x = t / duration;
            t += Time.deltaTime;

            if (targetGroup.m_Targets.Length > targetIndex)
                targetGroup.m_Targets[targetIndex].weight = targetFadeOut.Evaluate(x) * targetWeight;
            yield return null;
        }

        targetGroup.RemoveMember(transform);
        fadingOut = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, RadiusRequiredForTargetGroup);
    }
}
