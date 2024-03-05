using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_EatenByShark : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Ooble_EatenByShark_SS eatenByShark;
    public Ooble_HideUnderSpawnPosition hideUnderSpawnPosition;
    public DeathState_Respawn respawn;

    public Transform sharksMouth;
    public Vector3 sharkMouthPosition;
    public Vector3 sharkMouthRotation;
    public void SharkInitialize(Transform sharksMouth,Vector3 hitPosition)
    {
        this.sharksMouth = sharksMouth;
    }
    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.jump, ()=> respawn.StateDurationOver(subHSM))
        };

        eatenByShark.InitializeAsStartState(this, new List<StateConnection>()
        {
            new StateConnection(hideUnderSpawnPosition, ()=> eatenByShark.StateDurationOver(subHSM))
        }, out startState);

        hideUnderSpawnPosition.Initialize(this, new List<StateConnection>()
        {
            new StateConnection(respawn, ()=> hideUnderSpawnPosition.StateDurationOver(subHSM))
        });

        respawn.Initialize(this, new List<StateConnection>());

        subHSM = new HierarchicalStateMachine(startState);
    }
}

[System.Serializable]
public class Ooble_EatenByShark_SS : CharacterSubState
{
    public delegate void SharkDelegate();
    public static SharkDelegate playerEatenByShark;

    public Collider physicalCollider;
    public override void OnEnter()
    {
        base.OnEnter();
        physicalCollider.enabled = false;
        cState.mCState.rb.isKinematic = true;
        cState.transform.parent = cState.mCState.sharkMouthState.sharksMouth;
        cState.transform.localPosition = Vector3.zero;
        cState.transform.localRotation = Quaternion.Euler(0f,0f,0f);
        playerEatenByShark?.Invoke();
    }

    public override void OnExit()
    {
        base.OnExit();
        cState.transform.parent = null;
        physicalCollider.enabled = true;
        cState.mCState.rb.isKinematic = false;

    }
}

