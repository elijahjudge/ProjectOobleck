using UnityEngine;

public class BungoGravity : MonoBehaviour
{
    
    [Header("MinFallSpeed")]
    public float minFallSpeed;

    private Rigidbody rb;
    private float gravity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetGravity()
    {

        gravity = 13 / (2f * Mathf.Pow(.15f, 2f));

    }

    public float GetGravity() => gravity;
    public void SetGravity(float gravity)
    {
        this.gravity = gravity;
    }
    public void SimulateGravity()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Force);
        ClampYVelocity();
    }

    private void ClampYVelocity()
    {
        float clampedYSpeed = Mathf.Max(minFallSpeed, rb.velocity.y);
        rb.velocity = new Vector3(rb.velocity.x, clampedYSpeed, rb.velocity.z);
    }

    public void ChangeGravityBasedOnYVelocity(float desiredHeight,Vector3 durations, float hoverWindow)
    {
        float gravity;

        if (rb.velocity.y < hoverWindow && rb.velocity.y > -hoverWindow)
        {
            gravity = desiredHeight / (2f * Mathf.Pow(durations.y, 2f));
        }
        else if (rb.velocity.y < -hoverWindow)
        {
            gravity = desiredHeight / (2f * Mathf.Pow(durations.z, 2f));
        }
        else
        {
            gravity = desiredHeight / (2f * Mathf.Pow(durations.x, 2f));
        }

        this.gravity = gravity;
    }

    /*public float CalculateGravityFromDesiredJumpHeight()
    {
        float gravity;

        if (rb.velocity.y < 6f && rb.velocity.y > -6f)
        {
            gravity = jumpHeight / (2f * Mathf.Pow(hoverDuration, 2f));
        }
        else if (hsm._rigidbody.velocity.y < -6f)
        {
            gravity = jumpHeight / (2f * Mathf.Pow(fallDuration, 2f));
        }
        else
        {
            gravity = jumpHeight / (2f * Mathf.Pow(riseDuration, 2f));
        }

        return gravity;
    }*/
}

