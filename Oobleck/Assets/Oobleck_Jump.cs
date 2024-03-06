using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oobleck_Jump : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Oobleck_Jump_StartUp jumpStartUp;
    [SerializeField] public Oobleck_Jump_SS jump;

    public float staminaLoss;
    public int framesBeforeICanJumpAgain;

    public delegate void Jump(GameObject character);

    public static Jump playerJumped;

    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.groundMovement,() => jump.StateDurationOver(subHSM) && mCState.stamina.GetStaminaNormalized() <= .7f 
            && mCState.groundRider.grounded && !mCState.groundRider.touchingOobleck,
            (mCState.groundMovement as Ooble_Movement).staminaRecharging),
            new StateConnection(mCState.groundMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && !mCState.groundRider.touchingOobleck,
            (mCState.groundMovement as Ooble_Movement).run),
            new StateConnection(mCState.oobleckMovement,() => jump.StateDurationOver(subHSM) && mCState.groundRider.grounded && mCState.groundRider.touchingOobleck)
        };

        jumpStartUp.InitializeAsStartState(this, new List<StateConnection>() { new StateConnection(jump, () => jumpStartUp.StateDurationOver(subHSM)) }, out startState);

        jump.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnEnter()
    {
        mCState.stamina.LoseStamina(staminaLoss);
        base.OnEnter();
    }

    public override void OnTick()
    {
        base.OnTick();

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

    public BungoCurve jumpHeightFromStamina;
    public BungoCurve speedFromStamina;
    public override void OnEnter()
    {
        base.OnEnter();
        float jumpForce = jumpHeightFromStamina.Evaluate(cState.mCState.stamina.GetStaminaNormalized());
        cState.mCState.rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }

    public override void OnTick()
    {
        stateMovement.movement.valueRange = new Vector2(0f, speedFromStamina.Evaluate(cState.mCState.stamina.GetStaminaNormalized()));
        base.OnTick();
    }
}