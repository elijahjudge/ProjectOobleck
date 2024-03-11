using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_WaitForHatch : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Ooble_WaitForHatchSS waitForHatch;
    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
        };

        waitForHatch.InitializeAsStartState(this, new List<StateConnection>()
        {
        }, out startState);


        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]

public class Ooble_WaitForHatchSS : CharacterSubState
{
    public Vector3 hatchPosition;
    public float waitTime;

    public override void OnEnter()
    {
        cState.mCState.rb.isKinematic = true;
        base.OnEnter();
    }
    public override void OnExit()
    {
        cState.mCState.rb.isKinematic = false;
        cState.mCState.rb.velocity = Vector3.zero;
        cState.transform.position = hatchPosition;
        cState.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);

        base.OnExit();
    }

    public bool Hatched()
    {
        return StateDurationOver(cState.subHSM) && GetTimeInState() > waitTime;
    }

}