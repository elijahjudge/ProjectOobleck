using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_Drown : CharacterState
{
    [Header("SubStates")]
    public DeathState_Drown drown;
    public Ooble_HideUnderSpawnPosition hideUnderSpawn;

    public DeathState_Respawn respawning;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection> ()
        {
            new StateConnection(mCState.jump, ()=> respawning.StateDurationOver(subHSM))
        };

        drown.InitializeAsStartState(this, new List<StateConnection>()
        {
            new StateConnection(hideUnderSpawn, () => drown.StateDurationOver(subHSM))
        }, out startState);

        hideUnderSpawn.Initialize(this, new List<StateConnection>()
        {
            new StateConnection(respawning, () => hideUnderSpawn.StateDurationOver(subHSM))
        });

        respawning.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]
public class DeathState_Drown : CharacterSubState
{
    public Collider physicalCollider;
    public override void OnEnter()
    {
        physicalCollider.enabled = false;
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
        physicalCollider.enabled = true;
    }
}

[System.Serializable]
public class DeathState_Respawn : CharacterSubState
{
    public override void OnEnter()
    {
        base.OnEnter();
        cState.transform.position = cState.mCState.spawnPosition.spawnPosition;
        cState.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public override void OnExit()
    {
        base.OnExit();
        cState.mCState.stamina.ResetStamina();
        cState.mCState.rb.isKinematic = false;

    }
}

[System.Serializable]
public class Ooble_HideUnderSpawnPosition : CharacterSubState
{
    public override void OnEnter()
    {
        base.OnEnter();
        cState.mCState.rb.velocity = Vector3.zero;
        cState.mCState.rb.isKinematic = true;
        cState.mCState.stamina.ResetStamina();
        Vector3 underSpawn = new Vector3(cState.mCState.spawnPosition.spawnPosition.x, -10f, cState.mCState.spawnPosition.spawnPosition.z);
        cState.transform.position = underSpawn;
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}