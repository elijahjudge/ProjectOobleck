using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCameraHeight : MonoBehaviour
{
    public Bounds bounds;

    public Transform player;
    public CharacterTarget cameraTarget;
    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (bounds.Contains(player.position))
        {
            cameraTarget.AdjustPlayerHeight();
        }
 
    }

    private void OnDrawGizmosSelected()
    {

        bounds.center = transform.position;
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(bounds.center, bounds.size);

    }
}
