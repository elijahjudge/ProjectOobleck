using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Bounds bounds;
    public bool drawGizmos;

    public Transform player;
    public Transform mySpawnPosition;

    public delegate void CheckpointEvent(Vector3 positiion);

    public static CheckpointEvent checkPointTouched;

    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if(bounds.Contains(player.position))
        {
            checkPointTouched?.Invoke(mySpawnPosition.position);
        }
    }

    private void OnDrawGizmos()
    {
        if(drawGizmos)
        {
            bounds.center = transform.position;
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
