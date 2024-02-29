using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class InputReader : MonoBehaviour
{
    public enum overridable
    {
        buttonSouth,
        buttonNorth,
        buttonWest,
        triggerLeft,
        triggerRight,
        joystickLeft,
        joystickRight,
        buttonEast
    }

    public PlayerInput myPlayer;
    private Animator myAnimator;

    //public InputActionMap myActionMap;

    private Vector2 joystickLeft;
    private Vector2 joystickLeftAveraged;
    private Vector3 joystickLeftDirectionBasedOnGround;
    private Vector2 joystickRight;

    [HideInInspector] public delegate void Button();

    public event Button OnButtonSouthPressed = null;
    public event Button OnButtonSouthReleased = null;
    public event Button OnButtonNorthPressed = null;
    public event Button OnButtonNorthReleased = null;
    public event Button OnButtonWestPressed = null;
    public event Button OnButtonWestReleased = null;

    private bool buttonSouth;
    private bool buttonNorth;
    private bool buttonWest;
    private bool buttonEast;
    private bool triggerLeft;
    private bool triggerRight;

    [HideInInspector] public bool overrideJoystickLeft = false;
    [HideInInspector] public bool overrideJoystickRight = false;
    [HideInInspector] public bool overrideButtonSouth = false;
    [HideInInspector] public bool overrideButtonNorth = false;
    [HideInInspector] public bool overrideButtonWest = false;
    [HideInInspector] public bool overrideButtonEast = false;
    [HideInInspector] public bool overrideTriggerLeft = false;
    [HideInInspector] public bool overrideTriggerRight = false;

    private void Awake()
    {
        //prevent instant inputs
        timeSinceLastButtonWest = 999f;
        timeSinceLastButtonEast = 999f;
        timeSinceLastButtonNorth = 999f;
        timeSinceLastButtonSouth = 999f;
        timeSinceLastTriggerLeft = 999f;
        timeSinceLastTriggerRight = 999f;
    }

    private bool leftJoystickActive;
    private float ljiTime;
    public virtual void FixedUpdate()
    {
        IncrementTimesSinceButtonPresses();


        if (GetJoystickLeft().magnitude == 0 && leftJoystickActive)
        {
            ljiTime = Time.time;
            leftJoystickActive = false;
        }
        else if (GetJoystickLeft().magnitude != 0)
        {
            leftJoystickActive = true;
        }
    }

    public virtual bool GetButtonSouth() => overrideButtonSouth? false : buttonSouth;
    public virtual bool GetButtonNorth() => overrideButtonNorth ? false : buttonNorth;
    public virtual bool GetButtonWest(bool ignoreOverride = false)
    {
        if (ignoreOverride)
        {
            return buttonWest;
        }
        else
        {      
            return overrideButtonWest ? false : buttonWest;
        }
    }
    public virtual bool GetButtonEast() => overrideButtonEast ? false : buttonEast;

    public virtual Vector2 GetJoystickLeft() => overrideJoystickLeft ? Vector2.zero : joystickLeft;
    public virtual Vector2 GetJoystickLeftAveraged() => overrideJoystickLeft ? Vector2.zero : joystickLeftAveraged;
    public virtual Vector2 GetJoystickRight() => overrideJoystickRight ? Vector2.zero : joystickRight;
    public virtual bool GetTriggerLeft(bool ignoreOverride = false)
    {
        if (ignoreOverride)
        {
            return triggerLeft;
        }
        else
        {
            return overrideTriggerLeft ? false : triggerLeft;
        }
    }
    public virtual bool GetTriggerRight() => overrideTriggerRight ? false : triggerRight;

    public virtual void AssignInput(PlayerInput playerInput)
    {
        myPlayer = playerInput;
        myPlayer.actionEvents[0].AddListener(ReadButtonSouth);
        myPlayer.actionEvents[1].AddListener(ReadButtonNorth);
        myPlayer.actionEvents[2].AddListener(ReadButtonEast);
        myPlayer.actionEvents[3].AddListener(ReadButtonWest);
        myPlayer.actionEvents[4].AddListener(ReadJoystickLeft);
        myPlayer.actionEvents[5].AddListener(ReadJoystickRight);
        myPlayer.actionEvents[6].AddListener(ReadTriggerLeft);
        myPlayer.actionEvents[7].AddListener(ReadTriggerRight);
    }

    
    protected void InvokeOnButtonWestReleased()
    {
        OnButtonWestReleased?.Invoke();
    }


    private Vector2 previousJoystickValue;
    private float flickThreshold = 0.25f;
    private float flickDurationThreshold = 0.25f;
    private float joystickLeftTimeFlicked;
    public void ReadJoystickLeft(InputAction.CallbackContext ctx)
    {
        
        joystickLeft = Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1f);
       
        // Check if the joystick value has changed quickly
        if ((joystickLeft - previousJoystickValue).sqrMagnitude >= flickThreshold)
        {
            // Calculate the duration of the flick
            float flickDuration = (float)ctx.time - (float)ctx.startTime;
            // Check if the flick duration is within the threshold
            if (flickDuration <= flickDurationThreshold)
            {
                joystickLeftTimeFlicked = Time.time;
            }
        }

        previousJoystickValue = joystickLeft;    
      
        if(ctx.canceled)
        {
            timeJoystickLeftCanceled = Time.time;
        }
  
    }

    /*private bool leftSmash;
    private float timeSinceLastLeftSmash = 0f;
    public void ReadLeftJoystickSmash(InputAction.CallbackContext ctx)
    {
        ctx.ReadValue<Vector2>();
            Debug.Log("READSMASHLEFT HAS STARTED");

        if(ctx.started)
        {
            leftSmash = true;
            timeSinceLastLeftSmash = 0f;
            Debug.Log("READSMASHLEFT HAS STARTvklahjnergiunqeoirgnoiqenrgED");

        }
        else
        {
            leftSmash = false;
        }
    }

    public bool FrameAllowanceLeftSmash(int frames)
    {
        return timeSinceLastLeftSmash < frames.ConvertFramesToSeconds(); 
    }*/
    public void ReadJoystickRight(InputAction.CallbackContext ctx)
    {
        joystickRight = ctx.ReadValue<Vector2>();
    }
    public void ReadButtonSouth(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            if(OnButtonSouthPressed != null)
                OnButtonSouthPressed.Invoke();

            buttonSouth = true;
            timeSinceLastButtonSouth = 0f;

        }
        else if (ctx.canceled)
        {
            if (OnButtonSouthReleased != null)
                OnButtonSouthReleased.Invoke();

            buttonSouth = false;
        }
    }

    public void ReadButtonNorth(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (OnButtonNorthPressed != null)
                OnButtonNorthPressed.Invoke();

            buttonNorth = true;
            timeSinceLastButtonNorth = 0f;

        }
        else if (ctx.canceled)
        {
            if (OnButtonNorthReleased != null)
                OnButtonNorthReleased.Invoke();

            buttonNorth = false;
        }
    }

    public void ReadButtonWest(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (OnButtonWestPressed != null)
                OnButtonWestPressed.Invoke();

            buttonWest = true;
            timeSinceLastButtonWest = 0f;
        }
        else if (ctx.canceled)
        {
            if (OnButtonWestReleased != null)
                OnButtonWestReleased.Invoke();

            buttonWest = false;
        }
    }

    public void ReadButtonEast(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            buttonEast = true;
            timeSinceLastButtonEast = 0f;

        }
        else if (ctx.canceled)
        {
            buttonEast = false;
        }
    }
    public void ReadTriggerLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            triggerLeft = true;
            timeSinceLastTriggerLeft = 0f;
        }
        else if (ctx.canceled)
        {
            triggerLeft = false;
        }
    }

    public void ReadTriggerRight(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            triggerRight = true;
            timeSinceLastTriggerRight = 0f;
        }
        else if (ctx.canceled)
        {
            triggerRight = false;
        }
    }

    public bool GetJoystickLeftWindowForSmash(int frameWindow)
    {
        return (Time.time - joystickLeftTimeFlicked) < frameWindow.ConvertFramesToSeconds();
    }

    /*private void UpdateInputQueue()
    {
        if (inputsOverTime.Count == capacity)
        {
            inputsOverTime.Dequeue();
        }

        inputsOverTime.Enqueue(GetJoystickLeft());
    }

    private void UpdateAveragedInput()
    {
        Vector2 sumInput = Vector2.zero;

        foreach (Vector2 input in inputsOverTime)
        {
            sumInput += input;
        }

        Vector2 averagedInput = sumInput / capacity;

        joystickLeftAveraged = averagedInput;

    }*/

    private Dictionary<overridable, Coroutine> currentInputOverrides = new Dictionary<overridable, Coroutine>();

    public void OverrideInput(overridable input, float duration)
    {
        if(currentInputOverrides.ContainsKey(input))
        {
            StopCoroutine(currentInputOverrides[input]);
            currentInputOverrides.Remove(input);
        }

        Coroutine routine = StartCoroutine(OverrideInputRoutine(input, duration));
        currentInputOverrides.Add(input, routine);
    }

    public void OverrideAllInput(float duration)
    {
        StartCoroutine(OverrideInputRoutine(overridable.buttonSouth,duration));
        StartCoroutine(OverrideInputRoutine(overridable.buttonNorth, duration));
        StartCoroutine(OverrideInputRoutine(overridable.buttonWest, duration));
        StartCoroutine(OverrideInputRoutine(overridable.buttonSouth, duration));

        StartCoroutine(OverrideInputRoutine(overridable.joystickLeft, duration));
        StartCoroutine(OverrideInputRoutine(overridable.joystickRight, duration));
        StartCoroutine(OverrideInputRoutine(overridable.triggerLeft, duration));
        StartCoroutine(OverrideInputRoutine(overridable.triggerRight, duration));
    }

    public void OverrideAllInput(bool value)
    {
        overrideButtonSouth = value;
        overrideButtonNorth = value;
        overrideButtonWest = value;
        overrideButtonEast = value;
        overrideJoystickLeft = value;
        overrideJoystickRight = value;
        overrideTriggerLeft = value;
        overrideTriggerRight = value;
    }
    IEnumerator OverrideInputRoutine(overridable input, float duration)
    {
        OverrideSwitch(input, true);
        yield return new WaitForSeconds(duration);
        OverrideSwitch(input, false);
    }
    private void OverrideSwitch(overridable input,bool value)
    {
        switch (input)
        {
            case overridable.buttonSouth:
                overrideButtonSouth = value;
                break;
            case overridable.buttonNorth:
                overrideButtonNorth = value;
                break;
            case overridable.buttonWest:
                overrideButtonWest = value;
               break;
            case overridable.triggerLeft:
                overrideTriggerLeft = value;
                break;
            case overridable.triggerRight:
                overrideTriggerRight = value;
                break;
            case overridable.joystickLeft:
                overrideJoystickLeft = value;
                break;
            case overridable.joystickRight:
                overrideJoystickRight = value;
                break;
            case overridable.buttonEast:
                overrideButtonEast = value;
                break;
        }
    }

    public Vector3 CalculateDesiredMovementDirection()
    {
        Vector2 input = GetJoystickLeft();
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 directionInTermsOfCameraRotation = Quaternion.AngleAxis(cameraRotation.eulerAngles.y, Vector3.up) * direction;

        return directionInTermsOfCameraRotation;
    }

    private float timeSinceLastButtonWest = 0f;
    private float timeSinceLastButtonEast = 0f;
    private float timeSinceLastButtonNorth = 0f;
    private float timeSinceLastButtonSouth = 0f;
    private float timeSinceLastTriggerLeft = 0f;
    private float timeSinceLastTriggerRight = 0f;

    private void IncrementTimesSinceButtonPresses()
    {
        timeSinceLastButtonWest += Time.deltaTime;
        timeSinceLastButtonEast += Time.deltaTime;
        timeSinceLastButtonNorth += Time.deltaTime;
        timeSinceLastButtonSouth += Time.deltaTime;

        timeSinceLastTriggerLeft += Time.deltaTime;
        timeSinceLastTriggerRight += Time.deltaTime;
    }

    private float timeJoystickLeftCanceled = 0f;

    private float minimumTimeJoystickToDetectCancel = 1f/60f;

    public bool JoyStickLeftCanceled()
    {
        if ((Time.time - timeJoystickLeftCanceled) > minimumTimeJoystickToDetectCancel)
        {
            return joystickLeft == Vector2.zero;
        }
        else
            return false;
    }


    public virtual bool FrameAllowanceWestButton(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideButtonWest)
            return false;

        return timeSinceLastButtonWest < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetButtonWest());
    }
    public virtual bool FrameAllowanceEastButton(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideButtonEast)
            return false;

        return timeSinceLastButtonEast < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetButtonEast());
    }
    public virtual bool FrameAllowanceNorthButton(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideButtonNorth)
            return false;

        return timeSinceLastButtonNorth < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetButtonNorth());
    }
    public virtual bool FrameAllowanceSouthButton(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideButtonSouth)
            return false;

        return timeSinceLastButtonSouth < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetButtonSouth());
    }

    public virtual bool FrameAllowanceLeftTrigger(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideTriggerLeft)
            return false;

        return timeSinceLastTriggerLeft < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetTriggerLeft());
    }
    public virtual bool FrameAllowanceRightTrigger(int frameAllowance, bool hasToBeNewInput = false)
    {
        if (overrideTriggerRight)
            return false;

        return timeSinceLastTriggerRight < (float)frameAllowance / 60f || (hasToBeNewInput ? false : GetTriggerRight());
    }

}
