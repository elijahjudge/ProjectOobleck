using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float seconds;
    private float enterTime;
    private bool destroyed;

    private void Start()
    {
        enterTime = Time.time;
    }
    private void FixedUpdate()
    {
        if (Time.time - enterTime > seconds && !destroyed)
        {
            Destroy(gameObject);
            destroyed = true;
        }
    }

}
