﻿using System.Collections.Generic;
using UnityEngine;

public class Oobleshark_Attack : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Oobleshark_Attack_Alerted alerted;
    [SerializeField] public Oobleshark_Attack_CirclePlayer circlingPlayer;
    [SerializeField] public Oobleshark_Attack_StartUp attackStartup;
    [SerializeField] public Oobleshark_Attack_Dive diveAttack;
    [SerializeField] public Oobleshark_HideUnderNavMesh hideUnderNavMesh;
    [SerializeField] public Oobleshark_Attack_EndLag endLag;

    public float distanceBeforeOutOfRange;
    public bool busyEating;
    public void Awake()
    {
        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.patrolState,() => endLag.StateDurationOver(subHSM))
        };

        alerted.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(circlingPlayer,() => alerted.StateDurationOver(subHSM)) }, out startState);

        circlingPlayer.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(attackStartup,() => circlingPlayer.AttackPlayer())
        });

        attackStartup.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(diveAttack,() => attackStartup.StateDurationOver(subHSM))
        });

        diveAttack.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(hideUnderNavMesh,() => diveAttack.StateDurationOver(subHSM))
        });

        hideUnderNavMesh.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(endLag,() => hideUnderNavMesh.StateDurationOver(subHSM))
        });

        endLag.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(circlingPlayer,() => endLag.StateDurationOver(subHSM))
        });


        subHSM = new HierarchicalStateMachine(startState);

    }


    public override void OnTick()
    {
        base.OnTick();
    }

    public bool PlayerOutOfRange()
    {
        return Vector3.Distance(transform.position, mEState.player.position) > distanceBeforeOutOfRange
            && !busyEating;
    }

   
}

[System.Serializable]

public class Oobleshark_Attack_Alerted : EnemySubState
{

}

[System.Serializable]

public class Oobleshark_Attack_CirclePlayer : EnemySubState
{
    public Transform sharkCircleTransform;
    private Transform playerTransform;

    public Vector2 attackWaitIslandRange;
    public Vector2 attackWaitOobleckRange;

    private float waitBeforeAttackIsland;
    private float waitBeforeAttackOobleck;

    private float waitBeforeAttack;

    public ManagerCharacterState characterState;
    public override void OnEnter()
    {
        playerTransform = eState.mEState.player;
        eState.mEState.player = sharkCircleTransform;

        waitBeforeAttackIsland = Random.Range(attackWaitIslandRange.x, attackWaitIslandRange.y);
        waitBeforeAttackOobleck = Random.Range(attackWaitOobleckRange.x, attackWaitOobleckRange.y);

        waitBeforeAttack = waitBeforeAttackOobleck;
        base.OnEnter();
    }

    public override void OnTick()
    {
        base.OnTick();

        if(characterState.IsTouchingOobleck())
        {
            waitBeforeAttack = waitBeforeAttackOobleck;
        }
        else
        {
            waitBeforeAttack = waitBeforeAttackIsland;
        }
    }
    public bool AttackPlayer()
    {
        return StateDurationOver(eState.subHSM) && GetTimeInState() > waitBeforeAttack;
    }

    public override void OnExit()
    {
        eState.mEState.player = playerTransform;

        base.OnExit();
    }
}

[System.Serializable]

public class Oobleshark_Attack_StartUp: EnemySubState
{

}

[System.Serializable]
public class Oobleshark_Attack_Dive : EnemyAttackSubstate
{
    public Rigidbody rb;
    public float jumpHorizontalForce;
    public float jumpVerticalForce;
    public float gravity;
    public override void OnEnter()
    {
        rb.isKinematic = false;

        (eState as Oobleshark_Attack).busyEating = true;
        rb.velocity = Vector3.zero;
        base.OnEnter();
        Vector3 direction = (-eState.transform.position + eState.mEState.player.position).normalized;
        //direction = Vector3.Scale(direction, new Vector3(1f, 0f, 1f));

        rb.AddForce(direction * jumpHorizontalForce, ForceMode.Impulse);

        rb.AddForce(Vector3.up * jumpVerticalForce, ForceMode.Impulse);
    }

    public override void OnTick()
    {
        base.OnTick();
        rb.AddForce(Vector3.down * gravity);

    }

    public override void OnExit()
    {
        base.OnExit();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        
    }
}

[System.Serializable]
public class Oobleshark_HideUnderNavMesh : EnemySubState
{
    public override void OnEnter()
    {
        base.OnEnter();
        eState.transform.position = new Vector3(eState.transform.position.x,-25f, eState.transform.position.z);
    }
    public override void OnExit()
    {
        base.OnExit();
        eState.mEState.enemyAnimator.PlayAnimation("ComeBack");
        eState.mEState.navMesh.Warp(eState.transform.position);
        eState.mEState.navMesh.updatePosition = true;
    }
}


[System.Serializable]
public class Oobleshark_Attack_EndLag : EnemySubState
{
    public override void OnExit()
    {
        base.OnExit();
        (eState as Oobleshark_Attack).busyEating = false;
    }

}

