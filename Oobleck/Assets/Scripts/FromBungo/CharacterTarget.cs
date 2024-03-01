using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTarget : MonoBehaviour
{
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float height = 0f;

    public GameObject target;

    public delegate void CharacterTargetAdded(Transform transform);
    public static CharacterTargetAdded targetAdded;

    private void Awake()
    {
        GroundRider.characterTouchedGround += AdjustPlayerHeight;
    }

    private void Start()
    {
        targetAdded?.Invoke(transform);
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(target.transform.position.x,height,target.transform.position.z),
            Time.deltaTime * smoothing);
    }

    public void AdjustPlayerHeight(bool oobleck)
    {
        height = target.transform.position.y;
    }
}
