using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ooblegator_Chomp_Hold : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Ooblegator_ChompHold_SS chompHold;
    [SerializeField] public Ooblegator_PostChompClarity postChompClarity;

    public Oobleckgator_Attack aggroState;
    public Collider physicalCollider;

    private float hitTime;
    public void Awake()
    {
        Ooble_SlimeBalled.hitWithSlime += PlayerHitWithSlime;

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.aggroState, () => postChompClarity.StateDurationOver(subHSM), aggroState.chasing),
            new StateConnection(mEState.aggroState, SlimeHitInterrupt, aggroState.chasing)

        };

        chompHold.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(postChompClarity,() => chompHold.StateDurationOver(subHSM)) }, out startState);

        postChompClarity.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);

    }

    public override void OnEnter()
    {
        physicalCollider.enabled = false;
        base.OnEnter();
    }

    public override void OnExit()
    {
        physicalCollider.enabled = true;

        base.OnExit();
    }

    public void PlayerHitWithSlime()
    {
        hitTime = Time.time;
    }

    public bool SlimeHitInterrupt()
    {
        return (Time.time - hitTime) <= .5f;
    }
}

[System.Serializable]
public class Ooblegator_ChompHold_SS : EnemySubState
{
    Transform originalParent;
    public override void OnEnter()
    {
        originalParent = eState.transform.parent;
        eState.transform.parent = eState.mEState.player;
        base.OnEnter();
    }

    public override void OnExit()
    {
        eState.transform.parent = originalParent;
        base.OnExit();
    }
}

[System.Serializable]
public class Ooblegator_PostChompClarity : EnemySubState
{

}