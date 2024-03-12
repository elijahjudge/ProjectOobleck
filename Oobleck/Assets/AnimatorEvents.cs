using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour
{
    public delegate void AnimatorEvent();

    public static AnimatorEvent playFootstep;
 


    public void PlayFootstep() => playFootstep?.Invoke();

}
