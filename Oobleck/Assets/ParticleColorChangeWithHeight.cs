using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorChangeWithHeight : MonoBehaviour
{
    public GroundRider groundRider;

    public AnimationCurve heightDifferenceToColor;

    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HeightDifferenceInPositionToColor();
    }

    private void HeightDifferenceInPositionToColor()
    {
        float heightDifference = ((groundRider.latestGroundHeight - groundRider.transform.position.y) - 1.35f);

        float newColor = heightDifferenceToColor.Evaluate(heightDifference);

        sr.color = new Color(newColor, newColor, newColor);
    }
}
