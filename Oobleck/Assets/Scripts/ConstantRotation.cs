using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Vector3 angularSpeed;


    private void FixedUpdate()
    {
        transform.Rotate(angularSpeed,Space.Self);
    }
}

