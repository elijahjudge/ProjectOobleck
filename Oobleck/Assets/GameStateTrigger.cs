using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateTrigger : MonoBehaviour
{
    public ManagerGame.GameStates stateToBeAdvancedTo;
    public Bounds bounds;
    public Transform player;


    private void Start()
    {
        bounds.center = transform.position;
    }
    private void FixedUpdate()
    {
        if (bounds.Contains(player.position))
        {
            ManagerGame.instance.AdvanceGameState(stateToBeAdvancedTo);
        }
    }

    private void OnDrawGizmosSelected()
    {

        bounds.center = transform.position;
        Gizmos.color = Color.black;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
    }

    
}
