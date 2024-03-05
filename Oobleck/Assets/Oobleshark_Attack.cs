using System.Collections.Generic;
using UnityEngine;

public class Oobleshark_Attack : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Oobleshark_Attack_Alerted alerted;
    [SerializeField] public Oobleshark_Attack_CirclePlayer circlingPlayer;
    [SerializeField] public Oobleshark_Attack_StartUp attackStartup;
    [SerializeField] public Oobleshark_Attack_Dive diveAttack;
    [SerializeField] public Oobleshark_Attack_EndLag endLag;

    public float distanceBeforeOutOfRange;
    public void Awake()
    {
        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.patrolState,PlayerOutOfRange)
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
        new StateConnection(endLag,() => diveAttack.StateDurationOver(subHSM))
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
        return Vector3.Distance(transform.position, mEState.player.position) > distanceBeforeOutOfRange;
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

    public Vector2 attackWaitRange;

    private float waitBeforeAttack;

    public override void OnEnter()
    {
        playerTransform = eState.mEState.player;
        eState.mEState.player = sharkCircleTransform;

        waitBeforeAttack = Random.Range(attackWaitRange.x, attackWaitRange.y);
        base.OnEnter();
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
public class Oobleshark_Attack_Dive : EnemySubState
{
    public Rigidbody rb;
    public float jumpHorizontalForce;
    public float jumpVerticalForce;
    public float gravity;
    public override void OnEnter()
    {
        rb.velocity = Vector3.zero;
        eState.mEState.navMesh.updatePosition = false;
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

        eState.mEState.enemyAnimator.PlayAnimation("ComeBack");
        eState.mEState.navMesh.Warp(eState.transform.position);
        eState.mEState.navMesh.updatePosition = true;

    }
}

[System.Serializable]
public class Oobleshark_Attack_EndLag : EnemySubState
{

}

