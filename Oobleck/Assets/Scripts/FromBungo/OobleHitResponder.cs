using UnityEngine;

public class OobleHitResponder : HitResponder
{
    [SerializeField] private ManagerCharacterState mEState;
    public override void Respond(BungoHitBox attack, BungoHitbox_Spawner hitSpawnerResponder, Vector3 hitPosition)
    {
        base.Respond(attack, hitSpawnerResponder, hitPosition);

        switch(attack.type)
        {
            case BungoHitBox.HitBoxType.OobleckGator:
                Debug.Log("dats da aligator man");
                mEState.HSM.ChangeState(mEState.ooble_InAlligatorMouth);
                mEState.transform.position = hitPosition;
                break;
            case BungoHitBox.HitBoxType.OobleckShark:
                break;
            case BungoHitBox.HitBoxType.OobleckSlime:
                break;
        }
    }
}