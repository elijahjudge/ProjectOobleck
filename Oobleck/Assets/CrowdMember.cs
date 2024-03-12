using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    private Animator animator;

    public Vector2 timeBeforeAnimationSwap;
    public List<string> animations;

    private float animationEntered;
    private float waitTime;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        PlayRandomAnimation();
    }

    private void FixedUpdate()
    {
        if((Time.time - animationEntered) > waitTime)
        {
            PlayRandomAnimation();
        }
    }

    private void PlayRandomAnimation()
    {
        animationEntered = Time.time;
        int ran = Random.Range(0, animations.Count);
        animator.Play(animations[ran]);
        waitTime = Random.Range(timeBeforeAnimationSwap.x, timeBeforeAnimationSwap.y);
    }
}
