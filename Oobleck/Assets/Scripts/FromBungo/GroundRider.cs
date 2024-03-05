using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundRider : MonoBehaviour
{
    private int index;

    [Header("Raycast Setup")]
    public float rayLength;
    public LayerMask _groundMask;
    public int oobleckLayer;

    [Header("Ride Setup")]
    public float _rideHeight;
    public float _rideSpringStrength;
    public float _rideSpringDamper;
    [SerializeField] private Collider _collider;

    [Header("Slow Falling Player Force")]
    [SerializeField] private float _maxRayDistanceGroundApplySlowForce;
    [SerializeField] private float desiredFallingSpeed;

    [Header("Affect On Other Objects")]
    [SerializeField] private float _forceMultiplier;

    [HideInInspector] public Vector3 latestSlopeFoward = Vector3.zero;
    private  Vector3 inset = new Vector3(0f, 1f, 0f);
    private Vector3 _rayOffset;
    private float _scaledHeight;
    private float _scaledLength;
    private float _scaledLengthSlowForce;
    private Vector3 startCastPosition;

    private Rigidbody rb;

    [Header("Debug")]
    [SerializeField] private bool debugOn;
    public bool checkForGround = true;
    public bool grounded;
    public bool touchingOobleck;

    public delegate void GroundRelatedDelegate();
    public static GroundRelatedDelegate characterTouchedGround;
    public static GroundRelatedDelegate characterLeftGround;
    public static GroundRelatedDelegate characterTouchedOobleck;
    public static GroundRelatedDelegate characterLeftOobleck;
    public Transform oobleckParticlePosition;

    public float latestGroundHeight;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        characterTouchedGround += UpdateGroundHeight;
    }


    private void FixedUpdate()
    {      
        UpdateRideInfoBasedOnScale();
        AssignSlopeFoward();

        if(CheckIfGrounded())
        {
            if (!grounded)
            {
                if (touchingOobleck)
                    characterTouchedOobleck?.Invoke();
                else
                    characterTouchedGround?.Invoke();


                if (touchingOobleck)
                {

                }
            }

            grounded = true;
        }
        else
        {
            if (touchingOobleck)
                characterLeftOobleck?.Invoke();
            else
                characterLeftGround?.Invoke();

            grounded = false;
        }

    }

    private void UpdateGroundHeight()
    {
        latestGroundHeight = transform.position.y;
    }
    public float GetHeightDifferenceFromLatestGround()
    {
        return (latestGroundHeight - transform.position.y) - 1.35f;
    }
    public void UpdateRideInfoBasedOnScale()
    {
        startCastPosition = _collider.bounds.center + (Vector3.down * _collider.bounds.extents.y) + (Vector3.up * 1.1f);
        //_rayOffset = (Vector3.down * (_collider.bounds.size.y / 2f) + (inset * _collider.bounds.size.y));
        _scaledLength = transform.localScale.x * rayLength;
        _scaledLengthSlowForce = (_maxRayDistanceGroundApplySlowForce * _collider.bounds.size.y) + (inset * _collider.bounds.size.y).y;
        //_scaledHeight = _rideHeight * _collider.bounds.size.y + (inset * _collider.bounds.size.y).y;
        _scaledHeight = _rideHeight;

        spherePositionDebug = startCastPosition;
    }

    public void GroundRide()
    {
        RaycastHit rayHit;
  
        float sphereRadius = .5f;
        sphereScaleDebug = sphereRadius;
        //bool rayDidHit = Physics.Raycast(transform.position + _rayOffset, Vector3.down, out rayHit, _rayLength, _groundMask);
        //vector up * .5f compensates for the radius of the sphere
        bool rayDidHit = Physics.SphereCast(startCastPosition , sphereRadius, Vector3.down, out rayHit, _scaledLength, _groundMask);

        if (rayDidHit)
        {

            if (Vector3.Dot(rayHit.normal, Vector3.up) < .5f)
            {
                Vector3 getMeOffLedgeDirection = JustXZ(rayHit.normal).normalized;
                rb.AddForce(getMeOffLedgeDirection * 200f);
                return;
            }

            Vector3 velocity = rb.velocity;
            Vector3 rayDirection = Vector3.down;// transform.TransformDirection(Vector3.down);

            Rigidbody hitRigid = rayHit.rigidbody;
            Vector3 hitRigidVelocity = Vector3.zero;
            if (hitRigid != null)
            {
                hitRigidVelocity = hitRigid.velocity * _forceMultiplier;
            }


            float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
            float hitRigidDirectionVelocity = Vector3.Dot(rayDirection, hitRigidVelocity);

            float relativeVelocity = rayDirectionVelocity - hitRigidDirectionVelocity;

            float x = rayHit.distance - _scaledHeight;

            float springForce = (x * _rideSpringStrength * rb.mass) - (relativeVelocity * _rideSpringDamper);

            rb.AddForce(rayDirection * springForce);

            if (hitRigid != null)
            {
                hitRigid.AddForceAtPosition(rayDirection * -springForce, rayHit.point);
            }
        }


    }

    private Vector3 JustXZ(Vector3 vect)
    {
        return Vector3.Scale(vect, new Vector3(1f, 0f, 1f));
            
    }

    private void ApplySlowForce()
    {
        if (grounded)
            return;

        if (rb.velocity.y >= desiredFallingSpeed)
            return;

        RaycastHit hit;

        //Debug.DrawRay(transform.position + _rayOffset + (Vector3.right * 3f), Vector3.down * _scaledLengthSlowForce, Color.cyan);
        if (Physics.SphereCast(transform.position + _rayOffset, 1f, Vector3.down,
            out hit, _scaledLengthSlowForce, _groundMask))
        {
            float springForce = (desiredFallingSpeed) - (rb.velocity.y);

            rb.AddForce(0f,springForce,0f,ForceMode.VelocityChange);

        }
    }

    private Vector3 spherePositionDebug;
    private Vector3 spherePositionEndDebug;

    private float sphereScaleDebug;
    private Color rayColorDebug = Color.red;
    public bool CheckIfGrounded()
    {
        bool grounded = false;

        RaycastHit hit;
        float sphereRadius = 1f;

        float checkDistance = _scaledLength + ((rb.velocity.y < 0)?(Mathf.Abs(rb.velocity.y) / 50f):0f);

        if (Physics.SphereCast(startCastPosition, sphereRadius, Vector3.down, out hit, checkDistance, _groundMask))
        {
            float dot = Vector3.Dot(Vector3.up, hit.normal);

            if (dot > .75f)
            {
                rayColorDebug = Color.green;
                grounded = true;

                if (hit.transform.gameObject.layer == oobleckLayer)
                {
                    touchingOobleck = true;

                    Oobleck oob = hit.transform.gameObject.GetComponentInParent<Oobleck>();

                    if (oob != null)
                    {
                        oobleckParticlePosition = oob.GetParticleTransform();
                    }
                    else
                    {
                        oobleckParticlePosition = transform;
                    }
                }
                else
                {
                    touchingOobleck = false;
                }
            }
            
        }
        else
        {
            rayColorDebug = Color.red;
        }

        //DebugRay(sphereRadius, startPosition);

        return grounded;
    }

    public bool CheckIfGrounded(float castLength)
    {
        bool grounded = false;

        RaycastHit hit;
        float sphereRadius = 1.5f;
        Vector3 startPosition = transform.position + _rayOffset + (Vector3.up * sphereRadius / 2f);

        if (Physics.SphereCast(startPosition, sphereRadius, Vector3.down,
            out hit, castLength, _groundMask))
        {
            float dot = Vector3.Dot(Vector3.up, hit.normal);

            if (dot > .75f)
            {
                rayColorDebug = Color.green;
                grounded = true;
            }

        }
       
        return grounded;
    }

    /*private void DebugRay(float sphereRadius, Vector3 startPosition)
    {
        spherePositionDebug = startPosition;
        spherePositionEndDebug = spherePositionDebug + (Vector3.down * _scaledLength);
        sphereScaleDebug = sphereRadius;
        //if (debugOn)
        //    Debug.DrawRay(spherePositionDebug, Vector3.down * _scaledLength, rayColorDebug);
    }*/

    private void AssignSlopeFoward()
    {
        RaycastHit rayHit;
        bool rayDidHit = Physics.Raycast(transform.position, Vector3.down, out rayHit, _scaledLength * 2f, _groundMask);
  
        if(rayDidHit)
        {
            latestSlopeFoward = Vector3.ProjectOnPlane(transform.forward, rayHit.normal);
        }
        else
        {
            latestSlopeFoward = transform.forward;
        }
        /*else if(!assignedDiagonal)
        {
            latestSlopeFoward = latestSlopeFoward + Vector3.down;
            assignedDiagonal = true;        
        }*/
    }

    private void OnDrawGizmos()
    {
        if (debugOn)
        {
            Gizmos.color = rayColorDebug;
            Gizmos.DrawSphere(spherePositionDebug, sphereScaleDebug);
            Debug.DrawLine(startCastPosition, startCastPosition + (Vector3.down * _scaledLength));
            Gizmos.DrawSphere(spherePositionDebug + (Vector3.down * _scaledLength), sphereScaleDebug);
        }

    }
}
