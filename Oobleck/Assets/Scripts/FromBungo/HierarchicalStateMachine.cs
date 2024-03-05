using System;
using System.Collections.Generic;
using UnityEngine;

public class HierarchicalStateMachine
{
    public State currentState { get; private set; }

    public HierarchicalStateMachine(State startState)
    {
        currentState = startState;
        //currentState.OnEnter();
    }

    public void ChangeState(State newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
  
    public void InterruptState(State newState)
    {
        if(currentState is InterruptableState)
        {
            (currentState as InterruptableState).OnInterrupt();
        }

        currentState = newState;
        currentState.OnEnter();
    }

    public void ReturnToState(State returnedState)
    {
       
        currentState.OnExit();
        currentState = returnedState;

        if(returnedState is InterruptableState)
        {
            (returnedState as InterruptableState).OnReturn();
        }
    }

    public void ReturnToState(CharacterState newState, CharacterSubState subState)
    {

        currentState.OnExit();
        newState.subHSM.ResetState(subState);
        currentState = newState;

        if (currentState is InterruptableState)
        {
            (currentState
                as InterruptableState).OnReturn();
        }
    }
    // for when you want to enter a state starting at a specific substate 
    public void ChangeState(CharacterState newState,  CharacterSubState subState)
    {
        currentState.OnExit();
        newState.subHSM.ResetState(subState);
        currentState = newState;
        currentState.OnEnter();
    }

    public void ChangeState(EnemyState newState, EnemySubState subState)
    {
        currentState.OnExit();
        newState.subHSM.ResetState(subState);
        currentState = newState;
        currentState.OnEnter();
    }

    public void ChangeState(EnemyState newState, State subState)
    {
        currentState.OnExit();
        newState.subHSM.ResetState(subState);
        currentState = newState;
        currentState.OnEnter();
    }

    public void ResetState(State resetStateTo)
    {
        currentState = resetStateTo;
    }
}

public interface State
{
    public void OnEnter();
    public void OnTick();
    public void OnExit();
}

public interface InterruptableState : State
{
    public void OnInterrupt();
    public void OnReturn();
}


public class CharacterState : MonoBehaviour, InterruptableState
{
    [HideInInspector] public ManagerCharacterState mCState;

    public HierarchicalStateMachine subHSM;

    [HideInInspector] public CharacterSubState startState;

    protected List<StateConnection> stateConnections = new List<StateConnection>();

    [HideInInspector] public float timeEnteredCharacterState;
    public virtual void Initialize(ManagerCharacterState mCState)
    {
        this.mCState = mCState;
    }

    public virtual void Awake()
    {
        
    }
    public virtual void OnEnter()
    {
        timeEnteredCharacterState = Time.time;
        subHSM.currentState.OnEnter();
    }

    public virtual void OnExit()
    {
        subHSM.currentState.OnExit();
        subHSM.ResetState(startState);
    }

    public virtual void OnTick()
    {
        foreach (StateConnection sc in stateConnections)
        {
            if (sc.ConditionMetForGoingToState())
            {
                LeaveState(sc.state,sc.subState);
                return;
            }
        }

        subHSM.currentState.OnTick();
    }

    public void LeaveState(State newState, State subState) 
    {
        if (subState == null)
            mCState.HSM.ChangeState(newState);
        else
            mCState.HSM.ChangeState(newState as CharacterState,subState as CharacterSubState);

        if(subState == null)
            ManagerCharacterState.stateChanged?.Invoke(mCState.gameObject,(newState as CharacterState).startState);
        else
            ManagerCharacterState.stateChanged?.Invoke(mCState.gameObject,subState);
    }

    public void OnReturn()
    {
        if(subHSM.currentState is InterruptableState)
        {
            (subHSM.currentState as InterruptableState).OnReturn();
        }
        
    }

    public void OnInterrupt()
    {
        if (subHSM.currentState is InterruptableState)
        {
            (subHSM.currentState as InterruptableState).OnInterrupt();
        }
    }

}

[System.Serializable]
public class CharacterSubState : InterruptableState
{
    [Header("Frame Data")]
    public int duration;

    [Header("Movement")]
    public bool useCustomMovementLogic;
    public StateMovement stateMovement;

    [Header("Rotation")]
    public bool useCustomRotationLogic;
    public StateRotation stateRotation;

    [Header("Gravity")]
    public bool simulateGravity;
    public float stateGravity;

    [Header("Animation")]
    public string enterAnimation;

    [Header("Effects")]
    public StateEffects effects;

    protected List<StateConnection> stateConnections = new List<StateConnection>();
    // references to player
    [HideInInspector] public CharacterState cState;

    private List<GameObject> durationParticleEffects = new List<GameObject>();
    private List<ParticleSystem> durationParticleEffectSystems = new List<ParticleSystem>();
    private List<List<ParticleSystem>> allPS = new List<List<ParticleSystem>>();
    private List<AudioSource> durationSounds = new List<AudioSource>();

    protected float timeEntered;
    protected float timeInterrupted;
    protected float timeReturned;

    public virtual void Initialize(CharacterState characterState, List<StateConnection> stateConnections)
    {
        this.cState = characterState;
        this.stateConnections = stateConnections;
    }

    public virtual void InitializeAsStartState(CharacterState characterState, List<StateConnection> stateConnections, out CharacterSubState subState)
    {
        this.cState = characterState;
        this.stateConnections = stateConnections;
        subState = this;
    }
    public virtual void OnEnter()
    {
        ResetStateTime();
        StateRotationEnter();
        StateMovementEnter();

        foreach (ParticleEffectWithTransform effect in effects.effectsOnStart)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();

            if(effect.transformOverride == null)
                ManagerParticleEffects.instance.Play(effect.particleEffect, cState.mCState.transform,out particleSystems);
            else
                ManagerParticleEffects.instance.Play(effect.particleEffect, effect.transformOverride, out particleSystems);

            allPS.Add(particleSystems);
        }

        foreach (Sound sound in effects.soundsOnStart)
        {
            ManagerAudio.instance.Play(sound);
        }

        foreach (ParticleEffectWithTransform dEffect in effects.durationalEffects)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();
            GameObject pe;

            if (dEffect.transformOverride == null)
                pe = ManagerParticleEffects.instance.Play(dEffect.particleEffect, cState.mCState.transform, out particleSystems);
            else
                pe = ManagerParticleEffects.instance.Play(dEffect.particleEffect, dEffect.transformOverride, out particleSystems);

            allPS.Add(particleSystems);
            durationParticleEffects.Add(pe);
            durationParticleEffectSystems.Add(pe.GetComponentInChildren<ParticleSystem>());

        }

        foreach (ParticleEffect oob in effects.oobleckDurationEffects)
        {
            if (cState.mCState.groundRider.oobleckParticlePosition == null)
            {
                Debug.LogWarning("OOBLECK PARTICLE POSITION WAS NULL LOWKEY");
                return;
            }

            List<ParticleSystem> particleSystems = new List<ParticleSystem>();
            GameObject pe;


            pe = ManagerParticleEffects.instance.Play(oob, cState.mCState.groundRider.oobleckParticlePosition, out particleSystems);

            allPS.Add(particleSystems);
            durationParticleEffects.Add(pe);
            durationParticleEffectSystems.Add(pe.GetComponentInChildren<ParticleSystem>());
        }

        foreach (Sound sEffect in effects.durationalSounds)
        {
            AudioSource s = ManagerAudio.instance.Play(sEffect);
            durationSounds.Add(s);
        }

        if (enterAnimation != null)
        {
            if (enterAnimation == "")
                return;

            cState.mCState.animator.Play(enterAnimation, 0, 0f);
        }

    }

    private void ResetStateTime()
    {
        timeEntered = Time.time;
        timeInterrupted = 0f;
        timeReturned = 0f;
    }

    public virtual void OnExit()
    {
        foreach (ParticleEffectWithTransform effect in effects.effectsOnExit)
        {
            if (effect.transformOverride == null)
                ManagerParticleEffects.instance.Play(effect.particleEffect, cState.mCState.transform);
            else
                ManagerParticleEffects.instance.Play(effect.particleEffect, effect.transformOverride);
        }

        foreach (Sound sound in effects.soundsOnExit)
        {
            ManagerAudio.instance.Play(sound);
        }

        foreach (ParticleSystem effect in durationParticleEffectSystems)
        {
            if(effect != null)
                effect.Stop(true);
        }
        foreach (GameObject effect in durationParticleEffects)
        {
            if (effect != null)
            {
                effect.transform.parent = null;
                GameObject.Destroy(effect, 2f);
            }
        }

        foreach (AudioSource effect in durationSounds)
        {
            if(effect != null)
                ManagerAudio.instance.FadeSound(effect, effect.volume, 0f, .75f, true);
        }
    }

    public virtual void OnInterrupt()
    {
        timeInterrupted = Time.time;

        foreach (List<ParticleSystem> effect in allPS)
        {
            foreach(ParticleSystem e in effect)
            {
                if(e == null)
                    continue;

                ParticleSystem.MainModule main = e.main;
                main.simulationSpeed = .05f;

            }

        }

        foreach (AudioSource effect in durationSounds)
        {
            if (effect != null)
                effect.Pause();
        }

    }
    public virtual void OnReturn()
    {
        timeReturned = Time.time;

        foreach (List<ParticleSystem> effect in allPS)
        {
            foreach (ParticleSystem e in effect)
            {
                if (e == null)
                    continue;

                ParticleSystem.MainModule main = e.main;
                main.simulationSpeed = 1f;

            }

        }

        foreach (AudioSource effect in durationSounds)
        {
            if (effect != null)
                effect.UnPause();
        }
    }
    public virtual void OnTick()
    {
        StateRotationTick();
        StateMovementTick();
        SimulateGravity();

        foreach (StateConnection sc in stateConnections)
        {
            if(sc.ConditionMetForGoingToState())
            {
                LeaveState(sc.state);
            }
        }


    }
    public virtual void LeaveState(State newSubState)    
    {
        ManagerCharacterState.stateChanged?.Invoke(cState.mCState.gameObject,newSubState);
        cState.subHSM.ChangeState(newSubState);
    }

    public bool StateDurationOver(HierarchicalStateMachine hsm)
    {
        bool inThisState = hsm.currentState == this;
        return (GetTimeInState() > ((float)duration / 60f)) && inThisState;
    }

    public float GetTimeInState()
    {
        return (Time.time - timeEntered) - (timeReturned - timeInterrupted);
    }

    public void StateMovementEnter()
    {
        if (useCustomMovementLogic)
            return;


        switch (stateMovement.enterVelocity)
        {
            case StateMovement.HandleVelocity.KeepVelocity:
                break;
            case StateMovement.HandleVelocity.ResetXZVelocity:
                cState.mCState.movement.ResetXZVelocity();
                break;
            case StateMovement.HandleVelocity.ResetYVelocity:
                cState.mCState.movement.ResetYVelocity();
                break;
            case StateMovement.HandleVelocity.ResetAllVelocity:
                cState.mCState.movement.ResetVelocity();
                break;
            case StateMovement.HandleVelocity.Multiply:
                cState.mCState.rb.velocity *= stateMovement.multiplier;
                break;

        }


        Vector3 localRotatedImpulse = cState.transform.localRotation * stateMovement.initialForceImpulse;
        Vector3 newImpulse = new Vector3(localRotatedImpulse.x, stateMovement.initialForceImpulse.y, localRotatedImpulse.z);
        cState.mCState.rb.AddForce(newImpulse, ForceMode.Impulse);
    }
    public void StateMovementTick()
    {
        if (useCustomMovementLogic)
            return;

        if (stateMovement.GroundRide)
            cState.mCState.groundRider.GroundRide();


        switch (stateMovement.movementType)
        {
            case StateMovement.StateMovementType.None:
                break;
            case StateMovement.StateMovementType.TransformDirection:
                cState.mCState.movement.ApplyBungoCurveForcesToTransformDirection(stateMovement.movement, GetTimeInState(), duration);
                break;
            case StateMovement.StateMovementType.InputDirection:
                cState.mCState.movement.ApplyBungoCurveForcesToInputDirection(stateMovement.movement, GetTimeInState(), duration);
                break;
            case StateMovement.StateMovementType.InputDirectionOrTransform:
                cState.mCState.movement.ApplyBungoCurveForcesToInputOrTransformDirection(stateMovement.movement, GetTimeInState(), duration);
                break;
            case StateMovement.StateMovementType.InputConserveMomentum:
                cState.mCState.movement.ApplyBungoCurveForcesToInputDirectionConservative(stateMovement.movement, GetTimeInState(), duration);
                break;
        }

    }

    public void SimulateGravity()
    {
        if(simulateGravity)
        {
            cState.mCState.gravity.SetGravity(stateGravity);
            cState.mCState.gravity.SimulateGravity();
        }
    }
    public void StateRotationEnter()
    {
        if (useCustomRotationLogic)
            return;

        if(stateRotation.resetAngularVelocity)
            cState.mCState.rotater.ResetAngularVelocity();

        if (stateRotation.rotateTowardsInput.rotateTowardsDirection)
            cState.mCState.rotater.RotateLimitedAngleTowardsInputDirection(stateRotation.rotateTowardsInput.allowedRotation);
    }
    public void StateRotationTick()
    {
        if (useCustomRotationLogic)
            return;

        switch(stateRotation.rotationType)
        {
            case (StateRotation.StateRotationType.None):
                break;
            case (StateRotation.StateRotationType.Input):
                cState.mCState.rotater.ApplyRotation(stateRotation.rotationForce);
                break;
            case (StateRotation.StateRotationType.InputCustomSpringVariables):
                cState.mCState.rotater.ApplyRotation(stateRotation.rotationForce,stateRotation.springDamper,stateRotation.springStrength);
                break;
            case (StateRotation.StateRotationType.Upright):
                cState.mCState.rotater.ApplyUprightForces(stateRotation.rotationForce);
                break;

        }

    }
}

//require component bungo hitbox spawner
/*[System.Serializable]
public class CharacterAttackSubState : CharacterSubState
{
    [Header("Attack")]
    public BungoAttackInfo attack;

    private List<BungoHitChecker> hitChecker = new List<BungoHitChecker> ();
    public override void OnEnter()
    {
        base.OnEnter();
        hitChecker = cState.mCState.hitSpawner.StartAttack(attack);
        //spawn attack
    }

    public override void OnInterrupt()
    {
        base.OnInterrupt();
        InterruptHitboxes(hitChecker);
    }

    public override void OnReturn()
    {
        base.OnReturn();
        ReturnToHitboxes(hitChecker);
    }
    public override void OnTick()
    {
        base.OnTick();
        TickHitboxes(hitChecker);
    }
    public override void OnExit()
    {
        base.OnExit();
        DestroyHitboxes(hitChecker);
    }

    protected void TickHitboxes(List<BungoHitChecker> hitboxes)
    {
        if (hitboxes == null)
            return;

        int count = hitboxes.Count;

        for (int i = 0; i < count; i++)
        {
            if (hitboxes[i] != null)
                hitboxes[i].TickHitbox();
        }
    }

    protected void DestroyHitboxes(List<BungoHitChecker> hitboxes)
    {
        if (hitboxes == null)
            return;

        int count = hitboxes.Count;

        for (int i = 0; i < count; i++)
        {
            if (hitboxes[i] != null)
            {
                hitboxes[i].DestroyMe();
            }
        }

    }

    protected void InterruptHitboxes(List<BungoHitChecker> hitboxes)
    {
        if (hitboxes == null)
            return;

        int count = hitboxes.Count;

        for (int i = 0; i < count; i++)
        {
            if (hitboxes[i] != null)
            {
                hitboxes[i].Interrupt();
            }
        }

    }

    protected void ReturnToHitboxes(List<BungoHitChecker> hitboxes)
    {
        if (hitboxes == null)
            return;

        int count = hitboxes.Count;

        for (int i = 0; i < count; i++)
        {
            if (hitboxes[i] != null)
            {
                hitboxes[i].Return();
            }
        }

    }
}*/

public class StateConnection
{
    public State state;
    public State subState;
    public Func<bool> condition;

    public StateConnection(State state, Func<bool> condition, State subState = null)
    {
        this.state = state;
        this.condition = condition;
        this.subState = subState;
    }

    public bool ConditionMetForGoingToState()
    {
        return condition();
    }
}

[System.Serializable]
public class StateEffects
{
    [Header("Particles")]
    public List<ParticleEffectWithTransform> effectsOnStart = new List<ParticleEffectWithTransform>();
    public List<ParticleEffectWithTransform> effectsOnExit = new List<ParticleEffectWithTransform>();
    public List<ParticleEffectWithTransform> durationalEffects = new List<ParticleEffectWithTransform>();
    public List<ParticleEffect> oobleckDurationEffects = new List<ParticleEffect>();

    [Header("Sounds")]
    public List<Sound> soundsOnStart = new List<Sound>();
    public List<Sound> soundsOnExit = new List<Sound>();
    public List<Sound> durationalSounds = new List<Sound>();

}


[System.Serializable]
public class StateMovement
{
    [Header("On Enter")]
    public HandleVelocity enterVelocity;
    public float multiplier;
    public Vector3 initialForceImpulse;

    [Header("On Tick")]
    public StateMovementType movementType;
    public AmplitudeCurve movement;
    public bool GroundRide;

    [System.Serializable]
    public enum StateMovementType { None, TransformDirection, InputDirection, InputDirectionOrTransform,InputConserveMomentum }

    [System.Serializable]
    public enum HandleVelocity { KeepVelocity, ResetXZVelocity, ResetYVelocity, ResetAllVelocity,Multiply}
}


[System.Serializable]
public class StateRotation
{
    [Header("On Enter")]
    public bool resetAngularVelocity;
    public AllowedRotation rotateTowardsInput;

    [Header("On Tick")]
    public StateRotationType rotationType;
    public float rotationForce;
    public float springStrength;
    public float springDamper;

    [System.Serializable]
    public enum StateRotationType{ None,Input,InputCustomSpringVariables,Upright }
}

[System.Serializable]
public class AllowedRotation
{
    public bool rotateTowardsDirection;
    public float allowedRotation = 1f;
}
