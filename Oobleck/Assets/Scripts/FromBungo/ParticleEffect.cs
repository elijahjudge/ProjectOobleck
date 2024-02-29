using Unity.VisualScripting;
using UnityEngine;
using static ParticleEffect;

[CreateAssetMenu(fileName = "PE_", menuName = "ScriptableObjects/ParticleEffect")]
public class ParticleEffect : ScriptableObject
{
    public enum OffsetSpace { Local,World}
    public enum RotationSpace {Local,World}

    [Header("Prefab")]
    public GameObject particleEffect;

    [Header("Variables")]
    public ParticleEffectVariables particleEffectVariables;
}

[System.Serializable]
public class ParticleEffectVariables
{
    [Header("Spawn Variables")]
    [SerializeField] public Lifetime lifeTime;
    public float scaleMultiplier = 1f;

    [Header("When Spawned Relative to Transform")]
    public bool child;
    public Vector3 positionOffset;
    public OffsetSpace offsetSpace;
    public RotationSpace rotationSpace;


    public ParticleEffectVariables(bool child, Lifetime lifeTime, Vector3 position, OffsetSpace offsetSpace, RotationSpace rotationSpace, float scaleMultiplier)
    {
        this.child = child;
        this.lifeTime = lifeTime;
        this.positionOffset = position;
        this.offsetSpace = offsetSpace;
        this.rotationSpace = rotationSpace;
        this.scaleMultiplier = scaleMultiplier;
    }
}

[System.Serializable]
public class Lifetime
{
    public bool destroy = true;
    public float lifetime = 5f;
}

[System.Serializable]

public class ParticleEffectWithTransform
{
    [SerializeField] public ParticleEffect particleEffect;
    public Transform transformOverride;
}