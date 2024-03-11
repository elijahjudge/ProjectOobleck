using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Rigidbody rb;

    public Vector3 startTorque;

    public Vector2 aimErrorRange;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.AddTorque(startTorque, ForceMode.Impulse);
    }
    public void ShootMe(Vector3 destination, float duration)
    {
        Vector3 initialVelocity;
        Vector3 initialPosition = transform.position;
        Vector3 acceleration = Physics.gravity;


        initialVelocity = (initialPosition - destination + (.5f * acceleration * Mathf.Pow(duration, 2f)) )
            / -duration;

        rb.AddForce(initialVelocity,ForceMode.Impulse);

    }

    public void ShootMeAtRigidbodyGoalDestination(Rigidbody rb, Vector3 aimOffset, float duration)
    {
        Vector3 initialVelocity;
        Vector3 initialPosition = transform.position;
        Vector3 acceleration = Physics.gravity;
        Vector3 goalDestination = GetGoalDestination(rb,aimOffset, duration);

        initialVelocity = (initialPosition - goalDestination + (.5f * acceleration * Mathf.Pow(duration, 2f)))
            / -duration;

        this.rb.AddForce(initialVelocity, ForceMode.Impulse);

    }
    public Vector3 GetGoalDestination(Rigidbody rb, Vector3 aimOffset, float afterXXXseconds)
    {
        float aimError = Random.Range(aimErrorRange.x, aimErrorRange.y);
        float aimForDuration = afterXXXseconds + aimError;

        Vector3 acceleration = Vector3.zero;
        Vector3 goalDestination = (rb.transform.position+aimOffset) +
            (Vector3.Scale(rb.velocity, new Vector3(1f,0f,1f)) * aimForDuration) +
            (acceleration * .5f * Mathf.Pow(aimForDuration, 2f));

        return goalDestination;
    }
}
