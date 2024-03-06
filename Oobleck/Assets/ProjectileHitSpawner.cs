using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitSpawner : BungoHitbox_Spawner
{
    [SerializeField] private Rigidbody rb;
    public BungoAttackInfo slimeBallAttack;

    private void Start()
    {
        StartAttack(slimeBallAttack);
    }


    public override void HitResponse(BungoHitBox attack, HitResponder hitResponder, Vector3 hitPosition)
    {
        rb.GetComponent<Rigidbody>().isKinematic = true;
    }
    

}
