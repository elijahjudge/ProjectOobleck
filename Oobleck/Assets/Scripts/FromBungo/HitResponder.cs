using UnityEngine;

public class HitResponder : MonoBehaviour
{
    public enum HurtBoxType
    {
        Regular,
        Clash
    }
    // when recieving a hit
    //public GameObject owner; // so this can be put on a different game object but still link back to player
    public HurtBoxType type;

    public Transform owner; // when a character spawns multiple hitboxes this links those together and prevents self clashing

    private float timeOfLastHit;
    
    public virtual void Respond(BungoHitBox attack, BungoHitbox_Spawner hitSpawnerResponder, Vector3 hitPosition)
    {
        // important that the hitstop frames are taken into account since they are variable
        timeOfLastHit = Time.time + attack.stats.framesHitStopTaken.ConvertFramesToSeconds();
    }

    public virtual bool AllowHit(BungoHitBox attack)
    {
        // check spawnercharacter for the check for minimumFramesBetweenHits

        return true;
    }

    public void AssignType(HurtBoxType type)
    {
        this.type = type;
    }

    public void SetOwner(Transform newOwner)
    { 
        this.owner = newOwner;
    }
}
