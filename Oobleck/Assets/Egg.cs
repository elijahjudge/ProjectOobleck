using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Egg : MonoBehaviour
{
    public InputReader input;
    public Animator animator;

    public ParticleEffect hatchEffect;
    public ParticleEffect shakeEffect;
    public Sound shakeSound;
    public Sound hatchSound;

    public float timeBetweenShakes;
    public int shakesBeforeHatch;

    private int shakeCount;
    public delegate void EggEvent();
    public static EggEvent hatched;
    public static EggEvent shaked;

    public List<string> shakeAnimations;
 
    // Start is called before the first frame update
    void Start()
    {
    }


    private float lastTimeShook;
    private void FixedUpdate()
    {
        if(input.FrameAllowanceSouthButton(5,true) ||
            input.FrameAllowanceEastButton(5, true) ||
        input.FrameAllowanceNorthButton(5, true) ||
        input.FrameAllowanceWestButton(5, true))
        {
            TryShaking();
        }
    }


    private void TryShaking()
    {
        if(Time.time - lastTimeShook > timeBetweenShakes )
        {
            lastTimeShook = Time.time;
            int ranShakeIndex = Random.Range(0, shakeAnimations.Count);
            shaked?.Invoke();
            shakeCount++;
            // camera shake
            // sound;
            ManagerParticleEffects.instance.Play(shakeEffect, transform); // pe
            animator.Play(shakeAnimations[ranShakeIndex], 0, 0f); // animation

            CheckHatch();
        }
    }

    private void CheckHatch()
    {
        if(shakeCount >= shakesBeforeHatch)
        {
            hatched?.Invoke();
            ManagerParticleEffects.instance.Play(hatchEffect, transform);

            Destroy(gameObject);
        }
    }

}
