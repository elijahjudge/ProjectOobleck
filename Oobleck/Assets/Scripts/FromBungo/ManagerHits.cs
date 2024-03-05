using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Net.Sockets;
using System.Collections;

public class ManagerHits : MonoBehaviour
{
    // this script is set to execute after all other scripts

    [Header("Layer Masks")]
    public int hitboxLayer;
    public LayerMask hitboxLayerMask;

    [Header("Debug")]
    public List<HitTicket> validHitTickets = new List<HitTicket>();
    public List<HitTicket> hitTickets = new List<HitTicket>();
    public static ManagerHits instance;
    [HideInInspector] public GameObject hitBoxHolder;

    public delegate void HitDelegate(GameObject gameObject, HitTicket ticket);
    public static HitDelegate objectHit;
    public static HitDelegate objectHitter;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            hitBoxHolder = new GameObject();
            hitBoxHolder.name = "ManagerHits: HitBox Holder";
            DontDestroyOnLoad(instance);
            DontDestroyOnLoad(hitBoxHolder);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void FixedUpdate() // this is a late fixed update: see Script Execution Order in project settings
    {
        StartCoroutine(ValidateTickets());
    }

    IEnumerator ValidateTickets()
    {
        //Debug.Log("VT");

        if (hitTickets.Count == 0)
            yield break;

        //sort tickets

        List<HitTicket> clashTickets = new List<HitTicket>(); // clash tickets have priority
        

        foreach (HitTicket ticket in hitTickets)
        {
            if (ticket.hitResponder.type == HitResponder.HurtBoxType.Clash)
            {
                if(ValidTicket(ticket)) validHitTickets.Add(ticket);
            }
        }

        CompleteAllTickets();

        yield return new WaitForSeconds(1f / 60f);


        foreach (HitTicket ticket in hitTickets)
        {
            if (!clashTickets.Contains(ticket))
            {
                if (ValidTicket(ticket)) validHitTickets.Add(ticket);
            }
        }



        hitTickets.Clear();


        CompleteAllTickets();

    }

    private void CompleteAllTickets()
    {
        if (validHitTickets.Count == 0)
            return;

        foreach (HitTicket ticket in validHitTickets) // uses temporary list so we can clear the tickets without messing up the for each loop
        {
            ticket.CompleteTicket();
        }

        validHitTickets.Clear();
    }

    IEnumerator CompleteAllTicketsWithDelay()
    {
        if (validHitTickets.Count == 0)
            yield break;

        yield return new WaitForSeconds(1f / 60f);

        foreach (HitTicket ticket in validHitTickets) // uses temporary list so we can clear the tickets without messing up the for each loop
        {
            ticket.CompleteTicket();
        }

        validHitTickets.Clear();
    }
    public void AddHitTicket(HitTicket ticket)
    {
        bool duplicate = false;

        foreach (HitTicket t in hitTickets) // check duplicates
        {
            if (ticket.hitResponder == t.hitResponder && ticket.hitSpawnerResponder == t.hitSpawnerResponder)
            {
                duplicate = true;
            }
        }

        if (!duplicate) hitTickets.Add(ticket);
    }


    public bool ValidTicket(HitTicket ticket)
    {
        if (ticket.hitResponder == null || ticket.hitSpawnerResponder == null)
            return false;

        bool hitboxDoesntHitSelf = ticket.hitResponder.gameObject.GetRootGameObject() != ticket.hitSpawnerResponder.gameObject.GetRootGameObject();

        return hitboxDoesntHitSelf && ticket.hitResponder.AllowHit(ticket.attack);
    }


    /*
    private bool CheckCapsuleIntersection(BCapsule a, BCapsule b)
    {
        return a.radius + b.radius >= FindShortestDistance(a, b);
    }
    // deprecated, keeping here for future reference useful line function
    private float FindShortestDistance(BCapsule leftCapsule, BCapsule rightCapsule)
    {

        Vector3 p1, p2, p3, p4, d1, d2;

        p1 = leftCapsule.position;
        p2 = leftCapsule.position + (leftCapsule.direction * leftCapsule.distance);

        p3 = rightCapsule.position;
        p4 = rightCapsule.position + (rightCapsule.direction * rightCapsule.distance);

        d1 = leftCapsule.direction; // direction Line 1
        d2 = rightCapsule.direction; // direction Line 2

        // start point Shortest Line is i1 = p1 + m*d1
        // end point Shortest Line is i2 = p3 + n*d2

        //d1x*vx + d1y*vy + d1z*vz = 0 because the line is perpendicular the dot is 0
        //d2x*vx + d2y*vy + d2z*vz = 0 because the line is perpendicular the dot is 0

        // vx = i2x - i1x = (p3x + n*d2x) - (p1x + m*d1x)
        // vy = i2y - i1y = (p3y + n*d2y) - (p1y + m*d1y)
        // vz = i2z - i1z = (p3z + n*d2z) - (p1x + m*d1z)

        float H = Mathf.Pow(d2.x, 2) + Mathf.Pow(d2.y, 2) + Mathf.Pow(d2.z, 2);
        float I = (d1.x * d2.x) + (d1.y * d2.y) + (d1.z * d2.z);
        float J = (d2.x * p3.x) - (d2.x * p1.x) + (d2.y * p3.y) - (d2.y * p1.y) + (d2.z * p3.z) - (d2.z * p1.z);
        float K = Mathf.Pow(d1.x, 2) + Mathf.Pow(d1.y, 2) + Mathf.Pow(d1.z, 2);
        float L = (d1.x * p3.x) - (d1.x * p1.x) + (d1.y * p3.y) - (d1.y * p1.y) + (d1.z * p3.z) - (d1.z * p1.z);

        float m = (((-J * I) / (H * K)) + (L / K)) / (1 - (Mathf.Pow(I, 2) / (H * K)));
        float n = ((-m * I) + J) / -H; // solve for n

        if ((1 - (Mathf.Pow(I, 2) / (H * K))) == 0) // lines are parallel edge case
        {
            return Mathf.Min(Vector3.Distance(p1, p3), Vector3.Distance(p1, p4),
                Vector3.Distance(p2, p3), Vector3.Distance(p2, p4));
        }

        m = Mathf.Clamp(m, 0, leftCapsule.distance);
        n = Mathf.Clamp(n, 0, rightCapsule.distance); // make sure the points are on the line

        Vector3 I1 = p1 + (m * d1);
        Vector3 I2 = p3 + (n * d2);


        Debug.DrawLine(I1, I2, Color.red, 1f);

        return Vector3.Distance(I1, I2);

    }
    */


    [Header("Debug Gizmos")]
    public List<CapsuleHitboxGizmo> toBeDrawn = new List<CapsuleHitboxGizmo>();


    public void AddToBeDrawn(CapsuleHitboxGizmo hitbox)
    {
        if (toBeDrawn.Contains(hitbox))
            return;

        toBeDrawn.Add(hitbox);
    }

    public void RemoveToBeDrawn(CapsuleHitboxGizmo hitbox)
    {
        if (!toBeDrawn.Contains(hitbox))
        {
            return;
        }

        toBeDrawn.Remove(hitbox);
    }
    // needs to be drawn in the Managerhits object so all hitboxes can be visualized together
    private void OnDrawGizmos()
    {
        foreach(CapsuleHitboxGizmo hitbox in toBeDrawn)
        {
            DrawCapsule(hitbox);
        }
    }


    private void DrawCapsule(CapsuleHitboxGizmo hitBox)
    {
        Gizmos.color = hitBox.color;

        //Vector3 offset =  hitBox.offset;
        //Vector3 direction =  hitBox.direction;

        Vector3 offset = hitBox.transform.localRotation * hitBox.offset;
        Vector3 direction = hitBox.transform.localRotation * hitBox.direction;
        Vector3 position = hitBox.transform.position;

        Gizmos.DrawSphere(position + offset, hitBox.radius);
        Gizmos.DrawSphere(position + offset + (direction * hitBox.distance), hitBox.radius);

        for (float i = 0 + hitBox.drawStep; i < hitBox.distance; i += hitBox.drawStep)
        {
            Gizmos.DrawSphere(position + offset + (direction * i), hitBox.radius);
        }
    }
}
[System.Serializable]
public class CapsuleHitboxGizmo
{
    public Transform transform;
    public Vector3 offset;
    public Vector3 direction;
    public float distance;
    public float radius;
    public float drawStep;
    public Color color;

    public CapsuleHitboxGizmo(Transform transform, Vector3 offset, Vector3 direction, float distance, float radius, float drawStep, Color color)
    {
        this.transform = transform;
        this.offset = offset;
        this.direction = direction;
        this.distance = distance;
        this.radius = radius;
        this.drawStep = drawStep;
        this.color = color;
    }
}

[System.Serializable]
public class HitTicket
{
    // responders should be attached to the same object for this system to work

    public HitResponder hitResponder;
    public BungoHitbox_Spawner hitSpawnerResponder;
    public BungoHitBox attack;
    public Vector3 hitPosition;
    public HitTicket(HitResponder hitResponder, BungoHitbox_Spawner hitSpawnerResponder, BungoHitBox attack, Vector3 hitPosition)
    {
        this.hitResponder = hitResponder;
        this.hitSpawnerResponder = hitSpawnerResponder;
        this.attack = attack;
        this.hitPosition = hitPosition;
    }

    public void CompleteTicket()
    {
        hitResponder.Respond(attack, hitSpawnerResponder,  hitPosition);
        hitSpawnerResponder.HitResponse(attack, hitResponder, hitPosition);
    }

    
}


