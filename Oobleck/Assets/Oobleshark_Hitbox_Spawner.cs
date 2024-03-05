using UnityEngine;

public class Oobleshark_Hitbox_Spawner : BungoHitbox_Spawner
{
    public ManagerEnemyState mEState;
    public Transform sharkEatBone;
    public override void HitResponse(BungoHitBox attack, HitResponder hitResponder, Vector3 hitPosition)
    {
        //mEState.HSM.ChangeState(mEState.chompHold);
        Debug.Log("i ate player lol");
    }
}