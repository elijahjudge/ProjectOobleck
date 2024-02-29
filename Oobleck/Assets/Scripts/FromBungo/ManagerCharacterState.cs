using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagerCharacterState : MonoBehaviour
{
    public int characterNumber;
    public HierarchicalStateMachine HSM { get; private set; }


    [Header("Grounded")]
    public CharacterState groundMovement;
    public CharacterState jump;
 
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public InputReader input;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GroundRider groundRider;
    [HideInInspector] public CharacterRotation rotater;
    [HideInInspector] public CharacterMovement movement;
    [HideInInspector] public BungoGravity gravity;

    public CharacterVariables CV;
    private CharacterState startingState;

    public delegate void CharacterAdded(ManagerCharacterState character, int index);
    public static CharacterAdded playerAdded;
    public static CharacterAdded npcAdded;

    public delegate void StateChange(GameObject gameObject,State newState);
    public static StateChange stateChanged;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeAllCharacterStates();
        GetAllReferences();
        startingState = groundMovement;
        HSM = new HierarchicalStateMachine(startingState);

        input.AssignInput(GetComponent<PlayerInput>());
        input.myPlayer.currentActionMap = input.myPlayer.actions.FindActionMap("Player");

    }

    private void InitializeAllCharacterStates()
    {
        groundMovement.Initialize(this);
        jump.Initialize(this);
    }

    private void GetAllReferences()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputReader>();
        rotater = GetComponent<CharacterRotation>();
        movement = GetComponent<CharacterMovement>();
        animator = GetComponentInChildren<Animator>();
        groundRider = GetComponent<GroundRider>();
        gravity = GetComponent<BungoGravity>();
    }
    private void Start()
    {
        stateChanged?.Invoke(gameObject, startingState);      
    }
    private void FixedUpdate()
    {
        HSM.currentState.OnTick();
    }

}
