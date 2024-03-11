using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterTarget : MonoBehaviour
{
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float timeCameraTakesToGetBackToRespawnPosition = 2f;
    [SerializeField] private AnimationCurve backToSpawnXZAnimation;
    [SerializeField] private AnimationCurve backToSpawnYAnimation;

    public float heightOffset;
    [SerializeField] private float height = 1f;
    [SerializeField] private float respawnAnimationHeight;
    public ManagerCharacterState target;

    public delegate void CharacterTargetAdded(Transform transform);
    public static CharacterTargetAdded targetAdded;

    private Coroutine gettingToRespawnPoint;
    public Animator stateCamera;

    private void Awake()
    {
        GroundRider.characterTouchedGround += AdjustPlayerHeight;
        GroundRider.characterTouchedOobleck += AdjustPlayerHeight;
        Ooble_EatenByShark_SS.playerEatenByShark += PlayerDied;
    }

    private void Start()
    {
        ManagerCharacterState.playerDied += PlayerDied;
        targetAdded?.Invoke(transform);

    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        if(gettingToRespawnPoint != null)
            return;
        

        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(target.transform.position.x,height + heightOffset,target.transform.position.z),
            Time.deltaTime * smoothing);
    }

    public void AdjustPlayerHeight()
    {
        height = target.transform.position.y;
    }


    public void PlayerDied()
    {
        gettingToRespawnPoint = StartCoroutine(GetToRespawnPoint());
    }
    IEnumerator GetToRespawnPoint()
    {
        stateCamera.Play("Respawning");

        float t = 0;
        float lerpPos = 0f;
        Vector3 startPosition = transform.position;

        Vector3 aboveDeathPoint = new Vector3(startPosition.x,
            target.spawnPosition.spawnPosition.y,
            startPosition.z);

        while (t < .75f)
        {
            lerpPos = t / timeCameraTakesToGetBackToRespawnPosition;
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition,aboveDeathPoint,lerpPos);
            yield return null;
        }

        t = 0;
        lerpPos = 0f;

        while (t < timeCameraTakesToGetBackToRespawnPosition)
        {
            lerpPos = t / timeCameraTakesToGetBackToRespawnPosition;
            t += Time.deltaTime;

            float x = Mathf.Lerp(startPosition.x, target.spawnPosition.spawnPosition.x, backToSpawnXZAnimation.Evaluate(lerpPos));
            float z = Mathf.Lerp(startPosition.z, target.spawnPosition.spawnPosition.z, backToSpawnXZAnimation.Evaluate(lerpPos));
            float y = Mathf.Lerp(target.spawnPosition.spawnPosition.y, target.spawnPosition.spawnPosition.y + respawnAnimationHeight, backToSpawnYAnimation.Evaluate(lerpPos));

            transform.position = new Vector3 (x, y, z);
            yield return null;
        }

        height = target.spawnPosition.spawnPosition.y;

        gettingToRespawnPoint = null;

        stateCamera.Play("Default");
    }

}
