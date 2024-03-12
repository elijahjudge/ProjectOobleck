using UnityEngine;

public class GS_FinalCutScene : GameState
{
    public Animator cutsceneAnimator;
    public Animator stateDriverCamera;

    public override void OnStart()
    {
        base.OnStart();

        stateDriverCamera.Play("EndCutscene");
        cutsceneAnimator.Play("EndCutSceneDollyIn");
        mCState.input.OverrideAllInput(true);

    }
}