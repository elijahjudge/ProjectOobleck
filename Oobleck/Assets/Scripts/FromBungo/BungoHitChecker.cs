using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BungoHitChecker : MonoBehaviour
{

    private BungoHitBox bungoHitBox;
    private BungoHitbox_Spawner hitSpawner;
    private CapsuleCollider myHurtBox;
    private LayerMask hitboxLayerMask;

    // debug gizmos
    private float gizmoSphereDrawStep = .5f;
    private CapsuleHitboxGizmo hitboxGizmo;

    private float timeSinceEntered;
    private bool gizmoAddedToBeDrawn;

    [SerializeField] private List<HitResponder> hitList = new List<HitResponder>();
    public void InitializeMe(BungoHitBox bungoHitBox, BungoHitbox_Spawner hitSpawner,CapsuleCollider myHurtBox)
    {
        this.bungoHitBox = bungoHitBox;
        this.hitSpawner = hitSpawner;
        this.myHurtBox = myHurtBox;
        hitboxLayerMask = ManagerHits.instance.hitboxLayerMask;

        hitboxGizmo = new CapsuleHitboxGizmo(hitSpawner.transform, bungoHitBox.hitbox.positionOffset, bungoHitBox.hitbox.direction, bungoHitBox.hitbox.distance, bungoHitBox.hitbox.radius, gizmoSphereDrawStep, Color.green);

        
    }

    public void FixedUpdate() // characters override this and tick hitbox when it should be ticked
    {
        TickHitbox();
    }
    // needs to be called this way instead of fixed update so it can be interuptted by hit stop
    public void TickHitbox() // check for hits only during frame window
    {
        CopyHitboxPosition();

        if (GetTimeHitboxHasBeenOut() > (Mathf.Max(bungoHitBox.frameStart - 1,0)).ConvertFramesToSeconds()) // 1 frame early hurtbox window
        {
            if (myHurtBox != null)
            {
                if(!myHurtBox.enabled)
                    myHurtBox.enabled = true;
            }
        }

        if (hitSpawner.CheckHitbox(bungoHitBox,GetTimeHitboxHasBeenOut()))
        {
            CheckHitbox(bungoHitBox, hitSpawner);


            if (!gizmoAddedToBeDrawn)
            {
                gizmoAddedToBeDrawn = true;
                ManagerHits.instance.AddToBeDrawn(hitboxGizmo);
            }
        }
        else if (GetTimeHitboxHasBeenOut() > bungoHitBox.GetEndFrame().ConvertFramesToSeconds())
        {
            DestroyMe();
        }

        IncrementTime(); // as the states tick this, the timing will match if they are interrupted and returned to
    }

    public void DestroyMe()
    {
        ManagerHits.instance.RemoveToBeDrawn(hitboxGizmo);
        hitSpawner.activeHitBoxes.Remove(this);

        if (gameObject != null)
        {
            Destroy(gameObject);
        }

        /*if (destroying == null)
            destroying = StartCoroutine(DestroyWithDelay());  
        */
    }

    public void Interrupt()
    {
        ManagerHits.instance.RemoveToBeDrawn(hitboxGizmo);

    }

    public void Return()
    {
        if (hitSpawner.CheckHitbox(bungoHitBox, GetTimeHitboxHasBeenOut()))
            ManagerHits.instance.AddToBeDrawn(hitboxGizmo);

    }
    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(3f / 60f);

        
    }
    private float GetTimeHitboxHasBeenOut()
    {
        return timeSinceEntered;
    }

    private void IncrementTime()
    {
        timeSinceEntered += Time.deltaTime;
    }
    private void CopyHitboxPosition()
    {
        Vector3 offset = hitSpawner.transform.localRotation * bungoHitBox.hitbox.positionOffset;
        Vector3 direction = hitSpawner.transform.localRotation * bungoHitBox.hitbox.direction;
        Vector3 position = hitSpawner.transform.position;
        transform.localPosition = position + (offset + (direction * bungoHitBox.hitbox.distance / 2f)); // distance/two because its the midpoint

        if (direction != Vector3.zero)
        {
            transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    public void CheckHitbox(BungoHitBox bungoHitBox, BungoHitbox_Spawner hitSpawner)
    {
        Vector3 offset = hitSpawner.transform.localRotation * bungoHitBox.hitbox.positionOffset;
        Vector3 direction = hitSpawner.transform.localRotation * bungoHitBox.hitbox.direction;
        Vector3 position = hitSpawner.transform.position;

        Collider[] collidersHit = Physics.OverlapCapsule(position + offset, position + offset + (direction * bungoHitBox.hitbox.distance), bungoHitBox.hitbox.radius, hitboxLayerMask);

        
        foreach (Collider collider in collidersHit)
        {
            HitResponder hitResponder = collider.GetComponent<HitResponder>();

            if (hitResponder == null)
            {
                Debug.LogWarning("Why doesnt " + collider.name + " Have Hit Responder if its in layer mask " + hitboxLayerMask);
                continue;
            }

            if (hitResponder.owner == hitSpawner.owner) // for when spawning multiple overlapping colliders in one move
                continue;

            if (hitResponder.gameObject.GetRootGameObject("ManagerHits: HitBox Holder") == gameObject.GetRootGameObject("ManagerHits: HitBox Holder")) // for the case where the checker has its own hitbox ie the player attacks
                continue;

            if (hitList.Contains(hitResponder)) // attack specifies the minimum time it takes to hit the same responder again
                continue;


            //Debug.Log("HitResponder Owner: " + hitResponder.owner + "  HitSpawner Owner: " + hitSpawner.owner);

            /*LineSegment hurtCapsuleLine = new LineSegment(collider.transform.position - (Vector3.down * collider.bounds.extents.y), Vector3.up, collider.bounds.extents.y * 2f);
            LineSegment hitterCapsuleLine = new LineSegment(position + offset, direction, bungoHitBox.hitbox.distance);
            LineSegment shortestLineBetweenThem = FindShortestLineBetweenSkewLines(hurtCapsuleLine, hitterCapsuleLine);*/

            Vector3 directionBetweenResponders = (transform.position - collider.transform.position).normalized;

            //Vector3 hitPosition = shortestLineBetweenThem.position + (shortestLineBetweenThem.direction * (collider.bounds.extents.x + 1f));
            Vector3 hitPosition = collider.transform.position + (directionBetweenResponders * (collider.bounds.extents.x + 1.5f));

            hitList.Add(hitResponder);
            StartCoroutine(RemoveHitResponderFromHitList(hitResponder));
            HitTicket ticket = new HitTicket(hitResponder, hitSpawner, bungoHitBox, position + offset);
            ManagerHits.instance.AddHitTicket(ticket);

        }
    }

    IEnumerator RemoveHitResponderFromHitList(HitResponder hit)
    {
        yield return new WaitForSeconds(bungoHitBox.minimumFramesInbetweenHit/60f);

        hitList.Remove(hit);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private LineSegment FindShortestLineBetweenSkewLines(LineSegment a, LineSegment b)
    {

        Vector3 p1, p2, p3, p4, d1, d2;

        p1 = a.position;
        p2 = a.position + (a.direction * a.distance);

        p3 = b.position;
        p4 = b.position + (b.direction * b.distance);

        d1 = a.direction; // direction Line 1
        d2 = b.direction; // direction Line 2

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

        // lines are parallel edge case
        if ((1 - (Mathf.Pow(I, 2) / (H * K))) == 0) 
        {
            LineSegment p1p3 = new LineSegment(p1, (p1 - p3).normalized, Vector3.Distance(p1, p3));
            LineSegment p1p4 = new LineSegment(p1, (p1 - p4).normalized, Vector3.Distance(p1, p4));
            LineSegment p2p3 = new LineSegment(p2, (p2 - p3).normalized, Vector3.Distance(p2, p3));
            LineSegment p2p4 = new LineSegment(p2, (p2 - p4).normalized, Vector3.Distance(p2, p4));

            List<LineSegment> lines = new List<LineSegment>() { p1p3, p1p4, p2p3,p2p4};


            int indexShortest = 0;

            for (int i = 1; i < lines.Count; i ++)
            {
                if (lines[i].distance < lines[indexShortest].distance)
                {
                    indexShortest = i;
                }
            }

            return lines[indexShortest];
        }

        m = Mathf.Clamp(m, 0, a.distance);
        n = Mathf.Clamp(n, 0, b.distance); // make sure the points are on the line

        Vector3 I1 = p1 + (m * d1);
        Vector3 I2 = p3 + (n * d2);


        Debug.DrawLine(I1, I2, Color.red, 1f);

        LineSegment shortestLine = new LineSegment(I1, (I2-I1).normalized, Vector3.Distance(I1,I2));

        return shortestLine;

    }
}

public class LineSegment
{
    public Vector3 position;
    public Vector3 direction;
    public float distance;

    public LineSegment(Vector3 position, Vector3 direction, float distance)
    {
        this.position = position;
        this.direction = direction;
        this.distance = distance;
    }

    public Vector3 GetMidwayPoint()
    {
        return position + (direction * (distance * .5f));
    }

    public Vector3 GetEndPoint()
    {
        return position + (direction * (distance));
    }
}

public class BungoHitCheckerCharacter : BungoHitChecker
{
    public new void FixedUpdate()
    {
        // only tick when character specifies
    }
}