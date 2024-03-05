using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothing);
    }


}
