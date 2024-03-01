using UnityEngine;

public class ClampWithinBounds : MonoBehaviour
{
    [SerializeField] Bounds bounds;
    public bool storeBoundCenterOffsetToPosition;

    private void Start()
    {
        if(storeBoundCenterOffsetToPosition)
        {
            bounds.center = bounds.center + transform.position;
        }
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -bounds.extents.x +bounds.center.x, bounds.extents.x + bounds.center.x), Mathf.Clamp(transform.position.y, -bounds.extents.y + bounds.center.y, bounds.extents.y + bounds.center.y),
            Mathf.Clamp(transform.position.z, -bounds.extents.z + bounds.center.z, bounds.extents.z + bounds.center.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}