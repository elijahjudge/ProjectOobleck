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

    public RateOfThing jumpRate;
    public float jumpRateOutOf;
    public BungoCurve jumpRateToHeight;

    public override void Awake()
    {
        base.Awake();
        jumpRate = new RateOfThing(jumpRateOutOf);

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.groundMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && !mCState.groundRider.touchingOobleck),
            new StateConnection(mCState.oobleckMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && mCState.groundRider.touchingOobleck)
        };

        jumpStartUp.InitializeAsStartState(this, new List<StateConnection>() { new StateConnection(jump, () => jumpStartUp.StateDurationOver(subHSM)) }, out startState);

        jump.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnEnter()
    {

        base.OnEnter();
        jumpRate.AddThing();
    }

    public override void OnTick()
    {
        base.OnTick();
        jumpRate.UpdateRateTiming();
        Debug.Log("jump rate: " + jumpRate.GetRate());

    }

    public void RefreshJump()
    {
        jumpRate.Reset();
    }
}

[System.Serializable]

public class Oobleck_Jump_StartUp : CharacterSubState
{
    public override void OnEnter()
    {
        Ooble_Jump.playerJumped?.Invoke(cState.mCState.gameObject);
        base.OnEnter();
    }
}
[System.Serializable]
public class Oobleck_Jump_SS : CharacterSubState
{
    public override void OnEnter()
    {
        base.OnEnter();
        float jumpForce = cState.mCState.oobleckJump.jumpRateToHeight.Evaluate(cState.mCState.oobleckJump.jumpRate.GetRate());
        Debug.Log("JF! : " + jumpForce);
        cState.mCState.rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }
}