using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressAnyButton : MonoBehaviour
{
    public TextMeshPro tmp;
    public bool showing;
    public void FadeIn()
    {
        if (showing)
            return;

        showing = true;
        LeanTween.value(gameObject, Fade, 0f, 1f, 1f);
        Debug.Log("fading in");
    }

    public void FadeOut()
    {
        if (!showing)
            return;

        showing = false;
        LeanTween.value(gameObject, Fade, 1f, 0f, 1f);
    }


    private void Fade(float alpha)
    {
        tmp.alpha = alpha;
    }
}
