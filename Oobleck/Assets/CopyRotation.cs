using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    [Header("Debug Info")]
    public Transform POI;
    public float weight;

    private Quaternion goalRotation;
    private Quaternion goalPOIRotation;
    private void FixedUpdate()
    {
        goalRotation = Quaternion.Slerp(transform.localRotation, target.localRotation,
            Time.deltaTime * smoothing);

        if(POI == null)
        {
            transform.rotation = goalRotation;
        }
        else
        {
            Vector3 direction = (-Camera.main.transform.position + POI.position).normalized;
            direction = new Vector3 (direction.x, 0f, direction.z);

            goalPOIRotation = Quaternion.LookRotation(direction,Vector3.up);
            transform.rotation = goalRotation = Quaternion.Slerp(goalRotation, goalPOIRotation,weight);
        }

    }

    public void AddPOI(Transform transform, float weight)
    {
        POI = transform;
        this.weight = weight;
    }

    public void RemovePOI()
    {
        POI = null;
    }

}
