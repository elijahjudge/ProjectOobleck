using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_Jump : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Ooble_Jump_StartUp jumpStartUp;
    [SerializeField] public Ooble_Jump_Substate jump;

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
public class Ooble_Jump_StartUp : CharacterSubState
{
    public override void OnEnter()
    {
        SetDuration();

        Ooble_Jump.playerJumped?.Invoke(cState.mCState.gameObject);

        base.OnEnter();
    }

    public void SetDuration()
    {

    }
}
[System.Serializable]
public class Ooble_Jump_Substate : CharacterSubState
{

    public override void OnEnter()
    {
        base.OnEnter();
        AddInitialVelocityToBody();

    }

    public override void OnTick()
    {
        base.OnTick();

        cState.mCState.gravity.ChangeGravityBasedOnYVelocity(cState.mCState.CV.jumpHeight,
            new Vector3(cState.mCState.CV.riseDuration, cState.mCState.CV.hoverDuration, cState.mCState.CV.fallDuration),
            cState.mCState.CV.velocityForHoverWindow);

        cState.mCState.gravity.SimulateGravity();
    }
    private void AddInitialVelocityToBody()
    {
        float height = cState.mCState.CV.jumpHeight;

        float initialVelocity = Mathf.Sqrt(2f * height * GetRiseGravity());
        cState.mCState.rb.velocity = Vector3.Scale(cState.mCState.rb.velocity, new Vector3(1f, 0f, 1f));
        cState.mCState.rb.AddForce(Vector3.up * initialVelocity, ForceMode.VelocityChange);

        cState.mCState.rb.AddForce(cState.mCState.input.CalculateDesiredMovementDirection(), ForceMode.VelocityChange);
    }

    private float GetRiseGravity()
    {
        return cState.mCState.CV.jumpHeight / (2f * Mathf.Pow(cState.mCState.CV.riseDuration, 2f));
    }

}

