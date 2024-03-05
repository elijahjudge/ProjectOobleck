using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/BungoAttackInfo")]
public class BungoAttackInfo : ScriptableObject
{
    [SerializeField] public BungoHitBox[] hitBoxes;
}


[System.Serializable]
public class BungoBox
{
    public float radius;
    public Vector3 positionOffset;

                         
}

public class BSphere : BungoBox
{
}

public class BCuboid : BungoBox
{
    public Vector3 axisScale;
}

[System.Serializable]
public class BCapsule : BungoBox
{
    public float distance;
    public Vector3 direction;
}

[System.Serializable]
public class BungoHitBox
{
    public string name;
    public enum HitBoxType
    {
        OobleckGator,
        OobleckShark,
        OobleckSlime
    }

    public BCapsule hitbox;

    public int frameStart;

    public int frameDuration;

    public HitBoxType type;

    public float minimumFramesInbetweenHit;

    public BungoAttackStats stats;
    // attack info like damage knockbak n scaling

    public int GetEndFrame() => frameStart + frameDuration;
}

[System.Serializable]

public class BungoWindBox
{

    public string name;

    public BCuboid windbox;

    public int frameStart;

    public int frameDuration;

    public float force;

    public bool worldSpace;
    public Vector3 direction;

}

[System.Serializable]
public class BungoAttackStats
{
    
    public Vector3 hitDirection;
    public DirectionChange hitDirectionChangeWithKnockback;
    public bool xzDirectionBasedOnPositionDifference;
    public AnchorPosition anchorPosition;
    public FixedKnockback useFixedKnockback;
    public KnockbackValues knockbackValues;
    public float windForce;
    public float damage;
    public int framesHitStopTaken;
    public int framesHitStopGiven;

    public BungoCurve chargeMultiplier;
}

[System.Serializable]
public class KnockbackValues
{
    public float baseAttackPower;
    public float attackPower;
    public float attackGrowth;
}

[System.Serializable]

public class FixedKnockback
{
    public bool fixedKnockback;
    public float knockback;
}

[System.Serializable]
public class AnchorPosition
{
    [Range(0f, 1f)]
    public float anchorStrength; // 1 means move to anchor position 0 ignores anchorPosition

    public Vector3 anchorPositionOffset;

    public bool ignoreX;
    public bool ignoreY;
    public bool ignoreZ;
}

[System.Serializable]
public class DirectionChange
{
    public bool changeDirectionWithKnockbackDistance;

    public BungoCurve knockbackToLerpPos;
    public Vector3 hitDirectionChange;
}