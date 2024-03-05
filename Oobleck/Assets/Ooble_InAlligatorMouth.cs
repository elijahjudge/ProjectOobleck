using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_InAlligatorMouth : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Ooble_InAlligatorMouth_SS inAlligatorMouth;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.groundMovement,() => inAlligatorMouth.StateDurationOver(subHSM))
        };

        inAlligatorMouth.InitializeAsStartState(this, new List<StateConnection>() {
        }, out startState);


        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]
public class Ooble_InAlligatorMouth_SS : CharacterSubState
{
    private Vector3 holdPosition;

    public override void OnTick()
    {
        base.OnTick();
        //cState.mCState.rb.velocity = Vector3.zero;
        //cState.mCState.rb.angularVelocity = Vector3.zero;
        cState.mCState.stamina.LoseStaminaChompHold();

    }
}