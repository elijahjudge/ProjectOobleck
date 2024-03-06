using System.Collections.Generic;
using UnityEngine;

public class BungoHitbox_Spawner : MonoBehaviour
{
    public Transform owner;

    [Header("Visuals")]
    public bool DrawGizmos;

    [Header("Debug HitBoxes")]
    public List<BungoHitChecker> activeHitBoxes;

    public List<BungoHitChecker> StartAttack(BungoAttackInfo attack,float chargedTime = 0f)
    {
        if (attack == null)
            return null;

        List<BungoHitChecker> hitCheckers = new List<BungoHitChecker>();

        foreach (BungoHitBox hitBox in attack.hitBoxes)
        {
            BungoHitChecker bhc = SpawnHitBox(hitBox, chargedTime);

            if(bhc != null)
                hitCheckers.Add(bhc);
        }

        return hitCheckers;
    }
    protected virtual BungoHitChecker SpawnHitBox(BungoHitBox bungoHitBox,float chargedTime)
    {
        GameObject hitChecker = new GameObject();
        hitChecker.name = "Hitbox " + bungoHitBox.name;
        hitChecker.layer = ManagerHits.instance.hitboxLayer;
        hitChecker.transform.parent = ManagerHits.instance.hitBoxHolder.transform;

        
        BungoHitChecker bungoHitChecker = hitChecker.AddComponent<BungoHitChecker>();
        bungoHitChecker.InitializeMe(bungoHitBox, this, null);
        activeHitBoxes.Add(bungoHitChecker);
        return bungoHitChecker;
        
    }

    public void ClearAllHitBoxes()
    {
        int length = activeHitBoxes.Count;

        for (int i = 0; i < length; i++)
        {
            Destroy(activeHitBoxes[i].gameObject);
        }

        activeHitBoxes.Clear();
    }

    private void OnDestroy()
    {
        ClearAllHitBoxes();
    }

    private void OnDisable()
    {
        ClearAllHitBoxes();
    }
    public virtual void HitResponse(BungoHitBox attack, HitResponder hitResponder, Vector3 hitPosition)
    {
        
    }

    public virtual Vector3 GetDirection(BungoHitBox attackInfo,float knockbackDistance)
    {
        // makes sure the hit is aligned in local space

        Vector3 knockbackDirection = transform.localRotation * attackInfo.stats.hitDirection;

        if (attackInfo.stats.hitDirectionChangeWithKnockback.changeDirectionWithKnockbackDistance)
        {
            Vector3 knockbackDirectionFinal = transform.localRotation * attackInfo.stats.hitDirectionChangeWithKnockback.hitDirectionChange;
            float directionLerp = attackInfo.stats.hitDirectionChangeWithKnockback.knockbackToLerpPos.Evaluate(knockbackDistance);
            knockbackDirection = Vector3.Lerp(knockbackDirection, knockbackDirectionFinal, directionLerp);
        }

        if (attackInfo.stats.xzDirectionBasedOnPositionDifference)
        {
            Vector3 positionDifference = (-transform.position + transform.position).normalized;
            float xzStrength = new Vector2(knockbackDirection.x, knockbackDirection.z).magnitude;
            knockbackDirection = new Vector3(positionDifference.x * xzStrength, knockbackDirection.y, positionDifference.z * xzStrength);
        }

        return knockbackDirection;
    }

    public virtual float GetKnockbackDistance (BungoHitBox attackInfo, float damagePercent)
    {
        float distance;

        if (attackInfo.stats.useFixedKnockback.fixedKnockback)
        {
            distance = attackInfo.stats.useFixedKnockback.knockback;
        }
        else
        {
            distance = MathPlump.KnockbackFormula(
            damagePercent,
            attackInfo.stats.knockbackValues.attackPower,
            attackInfo.stats.knockbackValues.baseAttackPower,
            attackInfo.stats.knockbackValues.attackGrowth, 1f);
        }

        return distance;
    }
    public virtual bool CheckHitbox(BungoHitBox attack, float timeSinceAttack)
    {
        return (timeSinceAttack >= attack.frameStart.ConvertFramesToSeconds() && timeSinceAttack <= attack.GetEndFrame().ConvertFramesToSeconds());
    }
    public void SetOwner(Transform owner)
    {
        this.owner = owner;
    }
}
