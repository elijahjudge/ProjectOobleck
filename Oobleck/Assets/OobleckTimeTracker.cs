using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckTimeTracker : MonoBehaviour
{
    public GroundRider groundRider;
    public float Stamina = 100f;
    private void FixedUpdate()
    {
        if(groundRider.touchingOobleck && groundRider.grounded)
        {

        }
        else
        {

        }
    }

}
