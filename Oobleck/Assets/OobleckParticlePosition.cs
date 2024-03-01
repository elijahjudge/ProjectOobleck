using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckParticlePosition : MonoBehaviour
{
    public Transform player;
    public float smoothing;

    void Start()
    {
        if (player == null)
            Debug.LogWarning("OOBLECK NEEDS PLAYER TRANSFORM!");
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime * smoothing);
    }

}
