using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oobleck_Jump : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Oobleck_Jump_StartUp jumpStartUp;
    [SerializeField] public Oobleck_Jump_SS jump;

    public delegate void Jump(GameObject character);

    public static Jump playerJumped;


    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.groundMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && !mCState.groundRider.touchingOobleck),
            new StateConnection(mCState.oobleckMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && mCState.groundRider.touchingOobleck)
        };

        jumpStartUp.InitializeAsStartState(this, new List<StateConnection>() { new StateConnection(jump, () => jumpStartUp.StateDurationOver(subHSM)) }, out startState);

        jump.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }

}

[System.Serializable]

public class Oobleck_Jump_StartUp : CharacterSubState
{

}
[System.Serializable]
public class Oobleck_Jump_SS : CharacterSubState
{

}