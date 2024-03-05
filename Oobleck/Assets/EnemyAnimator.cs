using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    
    private Vector3 oldPosition;

    private void Start()
    {
        oldPosition = transform.position;

        InvokeRepeating("UpdatePosition", .2f, .2f);
    }
    public void SetAnimationVariable(string variable, bool value)
    {
        animator.SetBool(variable, value);
    }

    public void PlayAnimation(string anim)
    {
        animator.Play(anim, 0, 0f);
    }
    private void FixedUpdate()
    {
        float distanceTraveled = Vector3.Distance(oldPosition, transform.position);

        if(navMeshAgent.isOnNavMesh)
            animator.SetBool("Moving", navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance);
    }

    private void UpdatePosition()
    {
        oldPosition = transform.position;
    }
}
