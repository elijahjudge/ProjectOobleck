using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oobleck_Movement : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Oobleck_Idle idle;
    [SerializeField] public Oobleck_Run run;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.oobleckJump,() => mCState.input.FrameAllowanceSouthButton(5,true)
            ),
            new StateConnection(mCState.groundMovement,() => !mCState.groundRider.touchingOobleck)

        };

        run.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(idle,() => mCState.input.GetJoystickLeft() == Vector2.zero)
        }, out startState);

        idle.Initialize(this, new List<StateConnection>() {
            new StateConnection(run,() => mCState.input.GetJoystickLeft() != Vector2.zero)
        });

        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnTick()
    {
        base.OnTick();
    }
}

[System.Serializable]

public class Oobleck_Idle : CharacterSubState
{

}
[System.Serializable]
public class Oobleck_Run : CharacterSubState
{

}