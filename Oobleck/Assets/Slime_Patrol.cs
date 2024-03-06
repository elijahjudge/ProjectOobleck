using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Patrol : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Slime_Chilling chilling;

    public float distanceInRange;
    public bool touchingOobleck;
    public void Awake()
    {
        GroundRider.characterTouchedOobleck += PlayerTouchingOobleck;
        GroundRider.characterLeftOobleck += PlayerLeftOobleck;

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.aggroState,() => PlayerInRange())
        };

        chilling.InitializeAsStartState(this, new List<StateConnection>(), out startState);

        subHSM = new HierarchicalStateMachine(startState);
    }

    public void PlayerTouchingOobleck()
    {
        touchingOobleck = true;
    }
    public void PlayerLeftOobleck()
    {
        touchingOobleck = false;
    }

    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, mEState.player.position) <= distanceInRange;
    }
}

[System.Serializable]
public class Slime_Chilling : EnemySubState
{

}