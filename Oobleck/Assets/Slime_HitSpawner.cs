using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_HitSpawner : BungoHitbox_Spawner
{
    [SerializeField] private Rigidbody rb;
    public override void HitResponse(BungoHitBox attack, HitResponder hitResponder, Vector3 hitPosition)
    {
        rb.GetComponent<Rigidbody>().isKinematic = true;
    }
}
