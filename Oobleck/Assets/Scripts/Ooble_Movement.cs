using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_Movement : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Idle idle;
    [SerializeField] public Run run;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>() 
        { 
            new StateConnection(mCState.jump,() => mCState.input.FrameAllowanceSouthButton(10,true))
        };

        idle.InitializeAsStartState(this, new List<StateConnection>() { 
            new StateConnection(run,() => mCState.input.GetJoystickLeft() != Vector2.zero) 
        }, out startState);

        run.Initialize(this, new List<StateConnection>() {
            new StateConnection(idle,() => mCState.input.GetJoystickLeft() == Vector2.zero)
        });

        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]

public class Idle : CharacterSubState
{

}
[System.Serializable]
public class Run : CharacterSubState
{

}