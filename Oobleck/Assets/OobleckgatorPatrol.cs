using System.Collections.Generic;
using UnityEngine;

public class OobleckgatorPatrol : EnemyState
{
    [Header("Sub States")]
    [SerializeField] public OobleckgatorPatrol_Chill chill;
    [SerializeField] public OobleckgatorPatrol_GoToNewSpot goToNewSpot;

    public void Awake()
    {
        stateConnections = new List<StateConnection>()
        {
        };

        chill.InitializeAsStartState(this, new List<StateConnection>() { 
            new StateConnection(goToNewSpot,chill.FindNewChillSpot) }, out startState);

        goToNewSpot.Initialize(this, new List<StateConnection>()
        {
        new StateConnection(chill,() => goToNewSpot.StateDurationOver(subHSM))
        });

        subHSM = new HierarchicalStateMachine(startState);
       
    }

}

[System.Serializable]
public class OobleckgatorPatrol_Chill : EnemySubState
{
    public bool FindNewChillSpot()
    {
        return false;
    }

}

[System.Serializable]
public class OobleckgatorPatrol_GoToNewSpot : EnemySubState
{

}
