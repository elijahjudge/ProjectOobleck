using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

public class ManagerEnemyState : MonoBehaviour
{
    public string currentState;

    public Transform player;
    public HierarchicalStateMachine HSM { get; private set; }

    [Header("Enemy States")]
    public EnemyState patrolState;
    public EnemyState aggroState;
    public EnemyState chompHold;

    [HideInInspector] public NavMeshAgent navMesh;
    [HideInInspector] public EnemyAnimator enemyAnimator;
    [HideInInspector] public BungoHitbox_Spawner hitSpawner;

    private Vector3 oldPosition;
    // Start is called before the first frame update
    void Awake()
    {
        InitializeAllCharacterStates();
        GetAllReferences();


        HSM = new HierarchicalStateMachine(patrolState);
    }

    private void Start()
    {
        patrolState.OnEnter();      
    }
    private void InitializeAllCharacterStates()
    {
        patrolState.Initialize(this);
        aggroState.Initialize(this);

        if(chompHold != null)
            chompHold.Initialize(this);
    }

    private void GetAllReferences()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();
        navMesh = GetComponent<NavMeshAgent>();
        hitSpawner = GetComponent<BungoHitbox_Spawner>();
    }

    private void FixedUpdate()
    {
        currentState = (HSM.currentState as EnemyState).subHSM.currentState.ToString();

        HSM.currentState.OnTick();
    }

    private void LateUpdate()
    {
        oldPosition = transform.position;
    }
    public Vector3 GetVelocity()
    {
        return -oldPosition + transform.position;
    }
}

public class EnemyState : MonoBehaviour, State
{
    [HideInInspector] public ManagerEnemyState mEState;

    public HierarchicalStateMachine subHSM;

    [HideInInspector] public EnemySubState startState;

    protected List<StateConnection> stateConnections = new List<StateConnection>();

    protected float timeEntered;
    public virtual void Initialize(ManagerEnemyState mEState)
    {
        this.mEState = mEState;
    }

    public virtual void OnEnter()
    {
        timeEntered = Time.time;
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
                LeaveState(sc.state as EnemyState, sc.subState as EnemySubState);
                return;
            }
        }

        subHSM.currentState.OnTick();
    }

    public void LeaveState(EnemyState newState, EnemySubState newStateSubState)
    {
        if (newStateSubState == null)
            mEState.HSM.ChangeState(newState);
        else
            mEState.HSM.ChangeState(newState, newStateSubState);    
    }

    public void OnReturn()
    {
        if (subHSM.currentState is InterruptableState)
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

public class EnemySubState : State
{
    [Header("Frame Data")]
    public int duration;

    [Header("NavMesh")]
    public bool updateNavMeshPosition;
    public bool updateNavMeshRotation;

    [Header("Movement")]
    public bool goToPlayer;
    public float speed;
    public BungoCurve movementPattern;
    private float movementPatternOffset;

    [Header("Rotation")]
    public bool facePlayerOnEnter;
    public float allowedAngle;
    public bool alwaysLookAtPlayer;
    public float angularSpeed;

    [Header("Animation")]
    public string enterAnimation;
    public bool hardPlayAnimation;

    [Header("Effects")]
    public StateEffects effects;

    protected List<StateConnection> stateConnections = new List<StateConnection>();
    // references to player
    [HideInInspector] public EnemyState eState;

    private List<GameObject> durationParticleEffects = new List<GameObject>();
    private List<ParticleSystem> durationParticleEffectSystems = new List<ParticleSystem>();
    private List<List<ParticleSystem>> allPS = new List<List<ParticleSystem>>();
    private List<AudioSource> durationSounds = new List<AudioSource>();

    protected float timeEntered;
    protected float timeInterrupted;
    protected float timeReturned;

    public virtual void Initialize(EnemyState enemyState, List<StateConnection> stateConnections)
    {
        this.eState = enemyState;
        this.stateConnections = stateConnections;
    }

    public virtual void InitializeAsStartState(EnemyState enemyState, List<StateConnection> stateConnections, out EnemySubState subState)
    {
        this.eState = enemyState;
        this.stateConnections = stateConnections;
        subState = this;
    }
    public virtual void OnEnter()
    {
        movementPatternOffset = Random.Range(movementPattern.timeRange.x, movementPattern.timeRange.y);

        if(facePlayerOnEnter)
        {
            Quaternion goalRotation = Quaternion.LookRotation(-eState.transform.position + eState.mEState.player.transform.position, Vector3.up);

            eState.transform.rotation = Quaternion.Slerp(eState.transform.rotation, goalRotation, allowedAngle);
            
        }

       

        SetNavMeshVariables();

        ResetStateTime();

        foreach (ParticleEffectWithTransform effect in effects.effectsOnStart)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();

            if (effect.transformOverride == null)
                ManagerParticleEffects.instance.Play(effect.particleEffect, eState.mEState.transform, out particleSystems);
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
                pe = ManagerParticleEffects.instance.Play(dEffect.particleEffect, eState.mEState.transform, out particleSystems);
            else
                pe = ManagerParticleEffects.instance.Play(dEffect.particleEffect, dEffect.transformOverride, out particleSystems);

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
      
            eState.mEState.enemyAnimator.SetAnimationVariable(enterAnimation, true);

            if(hardPlayAnimation)
            {
                eState.mEState.enemyAnimator.PlayAnimation(enterAnimation);
            }
        }

    }

    private void SetNavMeshVariables()
    {
        eState.mEState.navMesh.updatePosition = updateNavMeshPosition;
        eState.mEState.navMesh.updateRotation = updateNavMeshRotation;
        eState.mEState.navMesh.speed = speed;
        eState.mEState.navMesh.angularSpeed = angularSpeed;
    }

    private void ResetStateTime()
    {
        timeEntered = Time.time;
        timeInterrupted = 0f;
        timeReturned = 0f;
    }

    public virtual void OnExit()
    {
        if (enterAnimation != null)
        {
            if (enterAnimation == "")
                return;

            eState.mEState.enemyAnimator.SetAnimationVariable(enterAnimation, false);
        }

        foreach (ParticleEffectWithTransform effect in effects.effectsOnExit)
        {
            if (effect.transformOverride == null)
                ManagerParticleEffects.instance.Play(effect.particleEffect, eState.mEState.transform);
            else
                ManagerParticleEffects.instance.Play(effect.particleEffect, effect.transformOverride);
        }

        foreach (Sound sound in effects.soundsOnExit)
        {
            ManagerAudio.instance.Play(sound);
        }

        foreach (ParticleSystem effect in durationParticleEffectSystems)
        {
            if (effect != null)
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
            if (effect != null)
                ManagerAudio.instance.FadeSound(effect, effect.volume, 0f, .75f, true);
        }
    }

    public virtual void OnInterrupt()
    {
        timeInterrupted = Time.time;

        foreach (List<ParticleSystem> effect in allPS)
        {
            foreach (ParticleSystem e in effect)
            {
                if (e == null)
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
        if(goToPlayer)
        {
            eState.mEState.navMesh.speed = speed * movementPattern.Evaluate(Time.time + movementPatternOffset);
            eState.mEState.navMesh.SetDestination(eState.mEState.player.position);

        }

        if (alwaysLookAtPlayer)
        {
            eState.transform.rotation = Quaternion.Slerp(
                eState.transform.rotation,
                Quaternion.LookRotation(-eState.transform.position + eState.mEState.player.transform.position, Vector3.up),
                Time.deltaTime * angularSpeed/100f);
        }

        foreach (StateConnection sc in stateConnections)
        {
            if (sc.ConditionMetForGoingToState())
            {
                LeaveState(sc.state);
            }
        }
    }
    public virtual void LeaveState(State newSubState)
    {
        eState.subHSM.ChangeState(newSubState);
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
    
}

[System.Serializable]
public class EnemyAttackSubstate : EnemySubState
{
    [Header("Attack")]
    public BungoAttackInfo attack;

    private List<BungoHitChecker> hitChecker = new List<BungoHitChecker>();
    public override void OnEnter()
    {
        base.OnEnter();
        hitChecker = eState.mEState.hitSpawner.StartAttack(attack);
        //spawn attack
    }

    public override void OnTick()
    {
        base.OnTick();
    }
    public override void OnExit()
    {
        base.OnExit();
        DestroyHitboxes(hitChecker);
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

}