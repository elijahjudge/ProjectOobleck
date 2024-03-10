using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Aggro : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Slime_LookAtPlayerSpitefully lookAtPlayerSpitefully;
    [SerializeField] public Slime_SpitAtPlayer spitAtPlayer;
    [SerializeField] public Slime_HappyYourInOobleck happyYourInOobleck;

    public float distanceBeforeOutOfRange;

    public ManagerCharacterState mCState;
    public void Awake()
    {

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.patrolState,PlayerOutOfRange)
        };

        lookAtPlayerSpitefully.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(spitAtPlayer,() => lookAtPlayerSpitefully.SpitAtPlayer()) ,
            new StateConnection(happyYourInOobleck,() => mCState.HSM.currentState is Oobleck_Movement)

        }, out startState);

        spitAtPlayer.Initialize(this, new List<StateConnection>()
        {
            new StateConnection(lookAtPlayerSpitefully,()=> spitAtPlayer.StateDurationOver(subHSM)),
            new StateConnection(happyYourInOobleck,() => mCState.HSM.currentState is Oobleck_Movement)

        });

        happyYourInOobleck.Initialize(this, new List<StateConnection>()
        {
            new StateConnection(lookAtPlayerSpitefully,() => !(mCState.HSM.currentState is Oobleck_Movement))
        });


        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnTick()
    {
        base.OnTick();
    }

    public bool PlayerOutOfRange()
    {
        return Vector3.Distance(transform.position, mEState.player.position) > distanceBeforeOutOfRange &&
            subHSM.currentState != spitAtPlayer;
    }
}

[System.Serializable]
public class Slime_LookAtPlayerSpitefully : EnemySubState
{
    public Vector2 waitTime;

    private float waitFor = 999f;

    public override void OnEnter()
    {
        waitFor = Random.Range(waitTime.x, waitTime.y);

        base.OnEnter();
    }

    public bool SpitAtPlayer()
    {
        return GetTimeInState() > waitFor;
    }
}

[System.Serializable]
public class Slime_SpitAtPlayer : EnemyAttackSubstate
{
    public Projectile spitOobleck;
    public Transform spitPosition;

    public float timeSpitTakesToReachPlayer;
    public Vector3 aimOffset;

    public override void OnAttackStartFrame()
    {
        base.OnAttackStartFrame();
        Projectile p = GameObject.Instantiate(spitOobleck, spitPosition.transform.position, spitPosition.transform.rotation);
        p.ShootMeAtRigidbodyGoalDestination((eState as Slime_Aggro).mCState.rb,aimOffset, timeSpitTakesToReachPlayer);
    }
}

[System.Serializable]
public class Slime_HappyYourInOobleck : EnemySubState
{

}