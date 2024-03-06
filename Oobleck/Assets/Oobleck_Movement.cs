using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            && (Time.time - mCState.oobleckJump.timeEnteredCharacterState) > 
            mCState.oobleckJump.framesBeforeICanJumpAgain.ConvertFramesToSeconds()
            ),
            new StateConnection(mCState.groundMovement,() => !mCState.groundRider.touchingOobleck 
            && mCState.stamina.GetStaminaNormalized() <= .7f, 
            (mCState.groundMovement as Ooble_Movement).staminaRecharging),

            new StateConnection(mCState.groundMovement,() => !mCState.groundRider.touchingOobleck,
            (mCState.groundMovement as Ooble_Movement).run)

        };

        run.InitializeAsStartState(this, new List<StateConnection>() {
            new StateConnection(idle,() => mCState.input.GetJoystickLeft() == Vector2.zero)
        }, out startState);

        idle.Initialize(this, new List<StateConnection>() {
            new StateConnection(run,() => mCState.input.GetJoystickLeft() != Vector2.zero)
        });

        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnEnter()
    {
        mCState.animator.SetBool("GroundMovement", false);
        base.OnEnter();
    }
}

[System.Serializable]

public class Oobleck_Idle : CharacterSubState
{
    public override void OnTick()
    {
        cState.mCState.stamina.LoseStaminaIdle();
        base.OnTick();
    }
}
[System.Serializable]
public class Oobleck_Run : CharacterSubState
{
    public BungoCurve speedFromStamina;
    public float maxSpeed;
    public float minSpeed;
    public override void OnTick()
    {
        stateMovement.movement.valueRange = new Vector2(0f, speedFromStamina.Evaluate(cState.mCState.stamina.GetStaminaNormalized()));

        cState.mCState.stamina.LoseStamina();
        base.OnTick();
    }
}