using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oobleckgator_Attack : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public Oobleckgator_Attack_Alerted alerted;
    [SerializeField] public Oobleckgator_Attack_Chase chasing;
    [SerializeField] public Oobleckgator_Attack_Chomp attacking;
    [SerializeField] public Oobleckgator_Attack_EndLag endLag;


    public float rangeRequiredForAttack;
    public float distanceBeforeOutOfRange;
    public void Awake()
    {
        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.patrolState,PlayerOutOfRange)
        };

        alerted.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(chasing,() => alerted.StateDurationOver(subHSM)) }, out startState);

        chasing.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(attacking,InAttackRange)
        });

        attacking.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(endLag,() => attacking.StateDurationOver(subHSM))
        });

        endLag.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(chasing,() => endLag.StateDurationOver(subHSM))
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

    public bool InAttackRange()
    {
        return Vector3.Distance(transform.position, mEState.player.position) < rangeRequiredForAttack;
    }
}

[System.Serializable]
public class Oobleckgator_Attack_Alerted : EnemySubState
{

}

[System.Serializable]
public class Oobleckgator_Attack_Chase : EnemySubState
{

}

[System.Serializable]
public class Oobleckgator_Attack_Chomp : EnemyAttackSubstate
{

}

[System.Serializable]
public class Oobleckgator_Attack_EndLag : EnemyAttackSubstate
{

}
