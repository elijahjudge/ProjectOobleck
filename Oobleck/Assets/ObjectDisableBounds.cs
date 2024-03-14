using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisableBounds : MonoBehaviour
{
    public Bounds bounds;
    public Transform player;

    public List<GameObject> gameObjectsEnabledOnEnter = new List<GameObject>();
    public List<GameObject> gameObjectsDisabledOnEnter = new List<GameObject>();

    private bool triggered;
    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (bounds.Contains(player.position) && !triggered)
        {
            triggered = true;

            foreach (GameObject obj in gameObjectsEnabledOnEnter)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in gameObjectsDisabledOnEnter)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        bounds.center = transform.position;
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }


}
