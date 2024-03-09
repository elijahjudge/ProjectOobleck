using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPoint;
    public Bounds bounds;
    public bool drawGizmos;

    public Transform player;
    public Transform mySpawnPosition;
    public delegate void CheckpointEvent(Vector3 position,int checkPoint);

    public static CheckpointEvent checkPointTouched;

    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if(bounds.Contains(player.position))
        {
            checkPointTouched?.Invoke(mySpawnPosition.position,checkPoint);
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
