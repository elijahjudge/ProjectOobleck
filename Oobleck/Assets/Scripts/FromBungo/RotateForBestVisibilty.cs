using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateForBestVisibilty : MonoBehaviour
{
    [SerializeField] private BungoCurve _angleFromDotProductXAxis;
    [SerializeField] private BungoCurve _angleFromDotProductYAxis;
    [SerializeField] private float _rotationSmoothing;
    [SerializeField] private Transform daddy;

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        Vector3 direction = daddy.forward;

        direction = Vector3.Scale(direction, new Vector3(1f, 0f, 1f));
        float dotProduct = Vector3.Dot(direction, Camera.main.transform.forward);
        float dotProductZ = Vector3.Dot(direction, Camera.main.transform.right);

        float angle = _angleFromDotProductXAxis.Evaluate(dotProduct);
        float angleZ = _angleFromDotProductYAxis.Evaluate(dotProductZ);

        Quaternion goalRotation = Quaternion.Euler(angle, angleZ, 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, goalRotation, Time.deltaTime * _rotationSmoothing);
    }

    public void ResetRotation()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

}
