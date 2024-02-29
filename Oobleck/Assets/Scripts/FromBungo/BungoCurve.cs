using UnityEngine;

[System.Serializable]
public class BungoCurve
{ 
    public AnimationCurve curve;
    public Vector2 valueRange = new Vector2(0,1);
    public Vector2 timeRange = new Vector2(0, 1);
}


[System.Serializable]
public class AmplitudeCurve
{
    public AnimationCurve curve;
    public Vector2 valueRange = new Vector2(0, 1);
}





