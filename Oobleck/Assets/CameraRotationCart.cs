using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationCart : MonoBehaviour
{
    public CinemachineSmoothPath path;
    public Transform cameraTarget;

    public float rotationSmoothing = 5f;
    private float pathPos;
    private void FixedUpdate()
    {
        pathPos = path.FindClosestPoint(cameraTarget.position, 0, 100, 10);

        transform.position = path.EvaluatePosition(pathPos);

        transform.rotation = Quaternion.Slerp(transform.rotation, path.EvaluateOrientation(pathPos),
            Time.deltaTime * rotationSmoothing);
    }
}
