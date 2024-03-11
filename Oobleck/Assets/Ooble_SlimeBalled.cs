using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ooble_SlimeBalled : CharacterState
{
    [Header("Sub States")]
    [SerializeField] public Ooble_SlimeBalled_SS slimed;
    [SerializeField] public Ooble_HitStop hitStop;

    public Vector3 direction;
    public float hitForce;

    public delegate void HitWithSlime();
    public static HitWithSlime hitWithSlime;
    public void InitializeSlimeBall(Vector3 direction, BungoHitbox_Spawner slimeBall, float hitForce)
    {
        this.direction = direction;
        this.hitForce = hitForce;
        Destroy(slimeBall.gameObject, hitStop.duration.ConvertFramesToSeconds());
    }
    public override void Awake()
    {
        base.Awake();

        stateConnections = new List<StateConnection>()
        {
            new StateConnection(mCState.groundMovement, ()=> slimed.StateDurationOver(subHSM))
        };

        hitStop.InitializeAsStartState(this, new List<StateConnection>() { 
        new StateConnection(slimed,()=>hitStop.StateDurationOver(subHSM))
        }, out startState);

        slimed.Initialize(this, new List<StateConnection>());


        subHSM = new HierarchicalStateMachine(startState);
    }

    public override void OnEnter()
    {
        hitWithSlime?.Invoke();
        base.OnEnter();
    }
}

[System.Serializable]
public class Ooble_SlimeBalled_SS : CharacterSubState
{
    public float torqueForce;
    public override void OnEnter()
    {
        base.OnEnter();

        cState.mCState.rb.AddForce(cState.mCState.slimeBalledState.direction * (cState as Ooble_SlimeBalled).hitForce, ForceMode.Impulse);
        cState.mCState.rb.AddTorque(cState.mCState.slimeBalledState.direction * torqueForce, ForceMode.Impulse);

    }


}

[System.Serializable]
public class Ooble_HitStop : CharacterSubState
{
    [Header("Hitstop Visuals")]
    public Material hurtMaterial;
    public float flashDuration;
    public override void OnEnter()
    {
        base.OnEnter();
        MaterialChanger.flashMaterial?.Invoke(cState.gameObject, hurtMaterial, flashDuration);
    }

}