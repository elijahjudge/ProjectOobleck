using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : CharacterState
{
    [Header("SubStates")]
    public DeathState_Drown drown;
    public DeathState_Respawn respawning;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection> ()
        {
            new StateConnection(mCState.groundMovement, ()=> respawning.StateDurationOver(subHSM))
        };

        drown.InitializeAsStartState(this, new List<StateConnection>()
        {
            new StateConnection(respawning, () => drown.StateDurationOver(subHSM))
        }, out startState);

        respawning.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]
public class DeathState_Drown : CharacterSubState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }
}

[System.Serializable]
public class DeathState_Respawn : CharacterSubState
{
    public override void OnEnter()
    {
        base.OnEnter();
        cState.mCState.stamina.ResetStamina();
        cState.transform.position = cState.mCState.spawnPosition.spawnPosition;
    }
}
