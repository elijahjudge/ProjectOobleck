using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrowCamSpace : MonoBehaviour
{
    public Bounds bounds;

    public Transform player;
    public Animator cameraStateAnimator;

    bool usingNarrowCam;
    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (bounds.Contains(player.position))
        {
            if (!usingNarrowCam)
            {
                usingNarrowCam = true;
                cameraStateAnimator.Play("NarrowCam");
            }
        }
        else
        {
            if(usingNarrowCam)
            {
                cameraStateAnimator.Play("Default");
                usingNarrowCam = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        bounds.center = transform.position;
        Gizmos.color = Color.black;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
    }

}
