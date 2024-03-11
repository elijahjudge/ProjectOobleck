using Cinemachine;
using System;
using UnityEngine;
public class GS_Intro : GameState
{
    public Animator stateAnimator;
    public InputReader input;
    public float delayEggInput;
    public override void OnStart()
    {
        input.OverrideAllInput(delayEggInput);

        Egg.hatched += Hatched;
        base.OnStart();
    }
    public override void TickGameState()
    {
        base.TickGameState();
    }

    public void Hatched()
    {
        ManagerGame.instance.AdvanceGameState(ManagerGame.GameStates.StartArea);
    }
    public override void OnExit()
    {
        base.OnExit();
        stateAnimator.Play("Default");
    }
}

