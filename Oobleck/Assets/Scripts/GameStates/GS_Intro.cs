using Cinemachine;
using System;
using UnityEngine;
public class GS_Intro : GameState
{
    public CinemachineVirtualCamera VirtualCamera;
    public float introDuration;

    public override void OnStart()
    {
        base.OnStart();
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
    }
}

