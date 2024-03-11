using UnityEngine;

public class OobleHitResponder : HitResponder
{
    [SerializeField] private ManagerCharacterState mCState;
    public override void Respond(BungoHitBox attack, BungoHitbox_Spawner hitSpawnerResponder, Vector3 hitPosition)
    {
        base.Respond(attack, hitSpawnerResponder, hitPosition);

        switch(attack.type)
        {
            case BungoHitBox.HitBoxType.OobleckGator:
                mCState.HSM.ChangeState(mCState.ooble_InAlligatorMouth);
                mCState.transform.position = hitPosition;
                break;
            case BungoHitBox.HitBoxType.OobleckShark:
                mCState.sharkMouthState.SharkInitialize((hitSpawnerResponder as Oobleshark_Hitbox_Spawner).sharkEatBone, hitPosition);
                mCState.HSM.ChangeState(mCState.sharkMouthState);
                break;
            case BungoHitBox.HitBoxType.OobleckSlime:
                Vector3 direction = (-hitPosition + mCState.transform.position).normalized;
                direction = new Vector3(direction.x,.45f,direction.z);
                mCState.slimeBalledState.InitializeSlimeBall(direction,hitSpawnerResponder, attack.stats.damage);
                //mCState.transform.position = (mCState.transform.position + (hitPosition + (Vector3.down * 2f)))/2f;
                mCState.HSM.ChangeState(mCState.slimeBalledState);
                break;
        }
    }


}