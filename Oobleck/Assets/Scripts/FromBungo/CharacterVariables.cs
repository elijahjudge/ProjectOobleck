using UnityEngine;


[CreateAssetMenu(fileName = "_Variables", menuName = "ScriptableObjects/CharacterVariables")]
[System.Serializable]
public class CharacterVariables : ScriptableObject
{
    [Header("Jump")]
    public float jumpHeight;
    public float velocityForHoverWindow;
    public float riseDuration;
    public float hoverDuration;
    public float fallDuration;

}