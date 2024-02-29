using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    private InputReader inputReader;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        inputReader = GetComponent<InputReader>();
    }

    public void ApplyBungoCurveForcesToTransformDirection(AmplitudeCurve acceleration, float timeElapsed, int frameDuration)
    {
        Vector3 goalDirection = transform.forward;
        Vector3 force = SetForce(acceleration, timeElapsed, frameDuration, goalDirection);

        rb.AddForce(Vector3.Scale(force, new Vector3(1f, 0f, 1f)), ForceMode.Force);
    }

    public void ApplyBungoCurveForcesToInputDirection(AmplitudeCurve acceleration, float timeElapsed, int frameDuration)
    {
        Vector3 goalDirection = inputReader.CalculateDesiredMovementDirection();
        Vector3 force = SetForce(acceleration, timeElapsed, frameDuration, goalDirection);

        rb.AddForce(Vector3.Scale(force, new Vector3(1f, 0f, 1f)), ForceMode.Force);
    }

    public void ApplyBungoCurveForcesToInputDirectionConservative(AmplitudeCurve acceleration, float timeElapsed, int frameDuration)
    {
        Vector3 goalDirection = inputReader.CalculateDesiredMovementDirection();
        Vector3 force = SetForce(acceleration, timeElapsed, frameDuration, goalDirection);

        if (inputReader.GetJoystickLeft().magnitude != 0)
            rb.AddForce(Vector3.Scale(force, new Vector3(1f, 0f, 1f)), ForceMode.Force);
        else
        {
            Vector3 clampedXZ = Vector3.ClampMagnitude(rb.velocity, acceleration.Evaluate(Mathf.InverseLerp(0f, (float)frameDuration / 60f, timeElapsed)));
            clampedXZ = new Vector3(clampedXZ.x,rb.velocity.y,clampedXZ.z);
            rb.AddForce(Vector3.Scale(force/10f, new Vector3(1f, 0f, 1f)), ForceMode.Force);
        }
    }
    public void ApplyBungoCurveForcesToInputOrTransformDirection(AmplitudeCurve acceleration, float timeElapsed, int frameDuration)
    {
        Vector3 goalDirection = inputReader.CalculateDesiredMovementDirection();

        if (goalDirection == Vector3.zero)
            goalDirection = transform.forward;

        Vector3 force = SetForce(acceleration, timeElapsed, frameDuration, goalDirection);

        rb.AddForce(Vector3.Scale(force, new Vector3(1f, 0f, 1f)), ForceMode.Force);
    }
    public void ApplyBungoCurveForcesToNormalizedInputDirection(AmplitudeCurve acceleration, float timeElapsed, int frameDuration)
    {
        Vector3 goalDirection = inputReader.CalculateDesiredMovementDirection().normalized;
        Vector3 force = SetForce(acceleration, timeElapsed, frameDuration, goalDirection);

        rb.AddForce(Vector3.Scale(force, new Vector3(1f, 0f, 1f)), ForceMode.Force);
    }
    private Vector3 SetForce(AmplitudeCurve acceleration, float timeElapsed, int frameDuration, Vector3 goalDirection)
    {
        Vector3 goalVelocity = goalDirection * acceleration.Evaluate(Mathf.InverseLerp(0f, (float)frameDuration / 60f, timeElapsed));
        Vector3 force = (goalVelocity - rb.velocity) / (.1f + Time.deltaTime);
        return force;
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }

    public void ResetYVelocity()
    {
        rb.velocity = new Vector3(rb.velocity.x,0f, rb.velocity.z);
    }
    public void ResetXZVelocity()
    {
        rb.velocity = new Vector3(0f,rb.velocity.y,0f);
    }
   
}
