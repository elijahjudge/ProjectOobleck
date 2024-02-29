using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private InputReader inputReader;

    public delegate void AnimatorDelayedDelegate(GameObject character,string trigger,float waitDuration = 0f);
    public delegate void AnimatorDelegate(GameObject character, string trigger);

    public static AnimatorDelayedDelegate setTrigger;
    public static AnimatorDelegate resetTrigger;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        inputReader = GetComponent<InputReader>();

        GroundRider.characterTouchedGround += SetGrounded;
        GroundRider.characterLeftGround += LeftGrounded;
        ManagerCharacterState.stateChanged += ResetStateWait;
        Ooble_Jump.playerJumped += PlayerJumped;

        setTrigger += SetTrigger;
        resetTrigger += ResetTrigger;
        

    }
    private void Update()
    {
        //SetAnimatorTilt();
        animator.SetFloat("VelocityMagnitude", rb.velocity.magnitude);
        animator.SetFloat("VelocityY", rb.velocity.y);
        //animator.SetFloat("StateWait", animator.GetFloat("StateWait") + Time.deltaTime);
        animator.SetFloat("TimeSinceJump", animator.GetFloat("TimeSinceJump") + Time.deltaTime);
    }
    public void SetGrounded(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        animator.SetBool("Grounded", true);
        animator.SetTrigger("LandedGround");
        animator.ResetTrigger("LeftGround");

        animator.Play("GroundLanding", 1, 0f);
        //ManagerAudio.instance.Play(landingSound);
    }

    public void LeftGrounded(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        animator.SetTrigger("LeftGround");
        animator.ResetTrigger("LandedGround");
        animator.SetBool("Grounded", false);
    }

    public void ResetStateWait(GameObject gameObject, State newState)
    {
        if (this.gameObject != gameObject)
            return;

        //animator.SetFloat("StateWait", 0f);
    }
    private void SetAnimatorTilt()
    {
        Vector3 facingDirectionPerp = transform.right;
        Vector3 inputDirection = inputReader.CalculateDesiredMovementDirection();
        float tilt = Vector3.Dot(facingDirectionPerp, inputDirection);

        animator.SetFloat("MovementTilt", tilt);
    }

    public void PlayerJumped(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        animator.SetFloat("TimeSinceJump", 0f);

    }
    public void SetTrigger(GameObject gameObject, string trigger, float waitDuration)
    {
        if (this.gameObject != gameObject)
            return;

        StartCoroutine(TriggerDelay(trigger, waitDuration));
    }

    public void ResetTrigger(GameObject gameObject, string trigger)
    {
        if(this.gameObject != gameObject)
            return;

        Debug.Log("reset: " + trigger);

        animator.ResetTrigger(trigger);
    }
    IEnumerator TriggerDelay(string trigger,float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        animator.SetTrigger(trigger);
    }

}
