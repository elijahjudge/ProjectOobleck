using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GS_StartArea : GameState
{
    public float pauseAtSecond;
    public float pauseForDuration;
    public float pauseSpeed;
    public GameObject pow;
    private bool paused;
    public override void TickGameState()
    {
        base.TickGameState();

        if (Time.time - timeEnteredState >= pauseAtSecond && !paused) 
        { 
            paused = true;
            StartPauseTimeline();
        }
    }

    private void StartPauseTimeline()
    {
        StartCoroutine(PauseActions());
    }
    IEnumerator PauseActions()
    {
        Time.timeScale = pauseSpeed;
        pow.SetActive(true);
        yield return new WaitForSecondsRealtime(pauseForDuration);
        pow.SetActive(false);
        Time.timeScale = 1f;

    }
}
