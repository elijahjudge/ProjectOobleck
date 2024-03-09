using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckLightDirection : MonoBehaviour
{

    public Transform cameraTarget;
    public float smoothing;

    public Vector3 rotationOffset;

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(rotationOffset) * cameraTarget.transform.rotation;
    }
}
