using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class OobleckgatorPatrol : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public OobleckgatorPatrol_Chill chill;
    [SerializeField] public OobleckgatorPatrol_GoToNewSpot goToNewSpot;

    public ManagerCharacterState characterState;
    public float distanceInRange;
    public bool touchingOobleck;
    public void Awake()
    {
        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mEState.aggroState,() => touchingOobleck && PlayerInRange())
        };

        chill.InitializeAsStartState(this, new List<StateConnection>() { 
            new StateConnection(goToNewSpot,chill.FindNewChillSpot) }, out startState);

        goToNewSpot.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(chill,() => goToNewSpot.StateDurationOver(subHSM))
        });

        subHSM = new HierarchicalStateMachine(startState);
       
    }


    public override void OnTick()
    {
        touchingOobleck = characterState.IsTouchingOobleck();
        base.OnTick();

    }

    public bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, mEState.player.position) <= distanceInRange;
    }
}

[System.Serializable]
public class OobleckgatorPatrol_Chill : EnemySubState
{
    public Vector2 waitRange;

    private float waitTime;

    public override void OnEnter()
    {
        waitTime = Random.Range(waitRange.x, waitRange.y);

        base.OnEnter();
    }
    public bool FindNewChillSpot()
    {
        return GetTimeInState() > waitTime;
    }

}

[System.Serializable]
public class OobleckgatorPatrol_GoToNewSpot : EnemySubState
{
    public float searchForNewSpotRadius;
    public float distanceToStop;
    Vector3 newSpotDestination;
    public override void OnEnter()
    {
        newSpotDestination = new Vector3(
            eState.transform.position.x + GetRanFloat(),
            eState.transform.position.y,
            eState.transform.position.z + GetRanFloat()
            );
        eState.mEState.navMesh.SetDestination(newSpotDestination);

        base.OnEnter();
    }

    public override void OnTick()
    {
        base.OnTick();

        eState.mEState.navMesh.speed = speed * movementPattern.Evaluate(Time.time);
        eState.mEState.navMesh.SetDestination(newSpotDestination);
    }

    private float GetRanFloat() => Random.Range(-searchForNewSpotRadius, searchForNewSpotRadius);

    public bool GoBackToChilling()
    {
        return eState.mEState.navMesh.remainingDistance < distanceToStop || StateDurationOver(eState.subHSM);
    }
}
