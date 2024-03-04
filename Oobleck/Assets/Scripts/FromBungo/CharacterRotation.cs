using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterRotation : MonoBehaviour
{
    [Header("Spring Forces")]
    [SerializeField] private float _uprightJointSpringDamper;
    [SerializeField] private float _springStrength;

    private InputReader inputReader;
    private Rigidbody rb;
    private Quaternion _uprightTargetRotation;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputReader = GetComponent<InputReader>();

        _uprightTargetRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void ApplyRotation(float rotateSpeed = 50f)
    {     
        SetTargetRotation();
        UpdateUprightForce(rotateSpeed);     
    }

    public void ApplyRotation(float rotateSpeed, float uprightJointSpringDamper,float springStrength)
    {
        SetTargetRotation();

        Quaternion currentRotation = rb.transform.localRotation;
        Quaternion goalRotation = ShortestRotation(_uprightTargetRotation, currentRotation);
        Vector3 rotationAxis;
        float rotationDegrees;
        goalRotation.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();
        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;


        rb.AddTorque((rotationAxis * (rotationRadians * springStrength * rotateSpeed)) - (rb.angularVelocity * uprightJointSpringDamper * rotateSpeed));
    }

    public void ApplyUprightForces(float uprightForce = 5f)
    {
        
        Quaternion _uprightTargetRotation = Quaternion.LookRotation(Vector3.Scale(rb.transform.forward, new Vector3(1f, 0f, 1f)), Vector3.up);

        Quaternion currentRotation = rb.transform.localRotation;
        Quaternion goalRotation = ShortestRotation(_uprightTargetRotation, currentRotation);
        Vector3 rotationAxis;
        float rotationDegrees;
        goalRotation.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();
        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;


        rb.AddTorque((rotationAxis * (rotationRadians * _springStrength * uprightForce)) - (rb.angularVelocity * _uprightJointSpringDamper * uprightForce));
    }
    private void SetTargetRotation()
    {

        Vector2 inputDirection = inputReader.GetJoystickLeft();
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 velocityDirection = new Vector3(inputDirection.x,0f,inputDirection.y);
        Vector3 directionInTermsOfCameraRotation = Quaternion.AngleAxis(cameraRotation.eulerAngles.y, Vector3.up) * velocityDirection;
        Vector3 goalLookRotation = Vector3.Scale(directionInTermsOfCameraRotation, new Vector3(1f,0f,1f));
        Quaternion lookRotation;

        if (inputDirection == Vector2.zero)
            lookRotation = Quaternion.LookRotation(Vector3.Scale(rb.transform.forward, new Vector3(1f, 0f, 1f)), Vector3.up);
        else       
            lookRotation = Quaternion.LookRotation(goalLookRotation, Vector3.up);
        

        _uprightTargetRotation = lookRotation;

    }

    private void UpdateUprightForce(float rotateSpeed)
    { 
        Quaternion currentRotation = rb.transform.localRotation;
        Quaternion goalRotation = ShortestRotation(_uprightTargetRotation, currentRotation);
        Vector3 rotationAxis;
        float rotationDegrees;
        goalRotation.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();
        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        rb.AddTorque((rotationAxis * (rotationRadians * _springStrength * rotateSpeed)) - (rb.angularVelocity * _uprightJointSpringDamper * rotateSpeed));
    }

    public static Quaternion ShortestRotation(Quaternion to, Quaternion from)
    {
        if (Quaternion.Dot(to, from) < 0)
        {
            return to * Quaternion.Inverse(Multiply(from, -1));
        }

        else return to * Quaternion.Inverse(from);
    }

    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public void RotateTowardsInputDirection()
    {
        Vector3 direction = inputReader.CalculateDesiredMovementDirection();

        if (direction == Vector3.zero)
            return;

        Quaternion lookDirection = Quaternion.LookRotation(direction, Vector3.up);

        transform.localRotation = lookDirection;
    }

    public void RotateLimitedAngleTowardsInputDirection(float angleAllowed)
    {
        Vector3 direction = inputReader.CalculateDesiredMovementDirection();

        if (direction == Vector3.zero)
            return;

        Quaternion lookDirection = Quaternion.LookRotation(direction, Vector3.up);

        transform.localRotation = Slerp(transform.localRotation, lookDirection, angleAllowed);
    }

    Quaternion Slerp(Quaternion from, Quaternion to, float t)
    {
        // Ensure both quaternions are normalized
        from.Normalize();
        to.Normalize();

        // Perform spherical linear interpolation (slerp)
        return Quaternion.Slerp(from, to, t);
    }


    public void ResetAngularVelocity()
    {
        rb.angularVelocity = Vector3.zero;
    }
}
