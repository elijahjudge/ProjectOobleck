using Cinemachine;
using System;
using UnityEngine;
public class GS_Intro : GameState
{
    public Animator stateAnimator;
    public InputReader input;
    public float delayEggInput;

    private bool inputUsed;
    public PressAnyButton pab;
    public override void OnStart()
    {
        input.OverrideAllInput(delayEggInput);

        Egg.hatched += Hatched;
        base.OnStart();
    }

   
    public override void TickGameState()
    {
        base.TickGameState();

        if (input.FrameAllowanceSouthButton(5, true) ||
            input.FrameAllowanceEastButton(5, true) ||
        input.FrameAllowanceNorthButton(5, true) ||
        input.FrameAllowanceWestButton(5, true) ||
        input.FlickedLeftJoystick())
        {
            inputUsed = true;
            pab.FadeOut();
        }

        if (Time.time - timeEnteredState > 5  && !inputUsed)
        {
            pab.FadeIn();
        }
    }

    public void Hatched()
    {
        ManagerGame.instance.AdvanceGameState(ManagerGame.GameStates.StartArea);
    }
    public override void OnExit()
    {
        base.OnExit();
        stateAnimator.Play("Default");
        pab.FadeOut();

    }
}

