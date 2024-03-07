using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.localRotation, target.localRotation,
            Time.deltaTime * smoothing);
    }
}
