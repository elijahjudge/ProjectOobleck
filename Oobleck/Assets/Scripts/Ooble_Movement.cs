using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Ooble_Movement : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Idle idle;
    [SerializeField] public Run run;
    public StaminaRecharging staminaRecharging;
    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>() 
        { 
            new StateConnection(mCState.jump,() => mCState.input.FrameAllowanceSouthButton(4,true)
            && Time.time - timeEnteredCharacterState > 0f && (subHSM.currentState != staminaRecharging)),
            new StateConnection(mCState.oobleckMovement,() => mCState.groundRider.touchingOobleck &&
            (subHSM.currentState != staminaRecharging))

        };

        idle.InitializeAsStartState(this, new List<StateConnection>() { 
            new StateConnection(run,() => mCState.input.GetJoystickLeft() != Vector2.zero) 
        }, out startState);

        run.Initialize(this, new List<StateConnection>() {
            new StateConnection(idle,() => mCState.input.GetJoystickLeft() == Vector2.zero)
        });

        staminaRecharging.Initialize(this, new List<StateConnection>()
        {
            new StateConnection(idle,mCState.stamina.IsStaminaFull)
        });

        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnTick()
    {
        mCState.stamina.GainStaminaBack();
        base.OnTick();
    }

    public override void OnEnter()
    {
        mCState.animator.SetBool("GroundMovement", true);
        base.OnEnter();
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

[System.Serializable]
public class StaminaRecharging : CharacterSubState
{
    public override void OnEnter()
    {
        cState.mCState.animator.SetBool("StaminaRecharging",true);
        base.OnEnter();
    }

    public override void OnExit()
    {
        cState.mCState.animator.SetBool("StaminaRecharging", false);
        base.OnExit();
    }
}