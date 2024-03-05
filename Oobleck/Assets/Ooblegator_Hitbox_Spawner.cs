using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooblegator_Hitbox_Spawner : BungoHitbox_Spawner
{
    public ManagerEnemyState mEState;
    public override void HitResponse(BungoHitBox attack, HitResponder hitResponder, Vector3 hitPosition)
    {
        mEState.HSM.ChangeState(mEState.chompHold);
    }
}
