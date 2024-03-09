using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTarget : MonoBehaviour
{
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float timeCameraTakesToGetBackToRespawnPosition = 2f;
    public float heightOffset;
    [SerializeField] private float height = 1f;

    public ManagerCharacterState target;

    public delegate void CharacterTargetAdded(Transform transform);
    public static CharacterTargetAdded targetAdded;

    private Coroutine gettingToRespawnPoint;
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

        float t = 0;
        float x = 0f;
        Vector3 startPosition = transform.position;

        while(t < timeCameraTakesToGetBackToRespawnPosition)
        {
            x = t / timeCameraTakesToGetBackToRespawnPosition;
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, target.spawnPosition.spawnPosition, x);
            yield return null;
        }

        height = target.spawnPosition.spawnPosition.y;

        gettingToRespawnPoint = null;
    }

}
