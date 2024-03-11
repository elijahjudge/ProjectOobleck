using System;
using UnityEngine;
public class GS_Intro : GameState
{
    public float introDuration;

    public override void OnStart()
    {
        base.OnStart();

        mCState.input.OverrideAllInput(true);
    }
    public override void TickGameState()
    {
        base.TickGameState();
        

        if(Time.time - timeEnteredState > introDuration)
        {
            ManagerGame.instance.AdvanceGameState(ManagerGame.GameStates.StartArea);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        mCState.input.OverrideAllInput(false);

    }
}

