using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckPlatform : MonoBehaviour
{
    public BungoCurve heightCurve;

    // Start is called before the first frame update
    void Start()
    {
        CharacterStamina.losingStamina += UpdateHeight;
        CharacterStamina.gainingStamina += UpdateHeight;
        CharacterStamina.staminaReset += ResetHeight;
    }
    public void UpdateHeight(float lerp)
    {
        transform.localPosition = new Vector3(transform.localPosition.x,
            heightCurve.Evaluate(lerp),transform.localPosition.z);
    }

    public void ResetHeight(float lerp)
    {
        transform.localPosition = new Vector3(transform.localPosition.x,
            0f, transform.localPosition.z);
    }
}
