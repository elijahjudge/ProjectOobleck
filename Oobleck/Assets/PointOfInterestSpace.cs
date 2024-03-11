using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestSpace : MonoBehaviour
{
    public Bounds bounds;

    public Transform player;
    public CopyRotation playerAim;

    public Transform POI;
    public float weight;

    bool pointOfInterestAdded;
    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (bounds.Contains(player.position))
        {
            if (!pointOfInterestAdded)
            {
                pointOfInterestAdded = true;
                playerAim.AddPOI(POI, weight);
            }
        }
        else
        {
            if (pointOfInterestAdded)
            {
                pointOfInterestAdded = false;
                playerAim.RemovePOI();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        bounds.center = transform.position;
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
    }
}
