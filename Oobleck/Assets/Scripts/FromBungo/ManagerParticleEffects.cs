using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerParticleEffects : MonoBehaviour
{
    [Header("Testing Purposes")]
    [SerializeField] private List<ParticleEffect> testParticleEffects = new List<ParticleEffect>();
    [SerializeField] private float testRate = 1f;

    public static ManagerParticleEffects instance = null;
    private GameObject particleEffectHolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            particleEffectHolder = new GameObject();
            particleEffectHolder.name = "Holder Particle Effects";

            DontDestroyOnLoad(instance);
            DontDestroyOnLoad(particleEffectHolder);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        // Easy way to test particle effects
        if (testParticleEffects.Count > 0)
            InvokeRepeating("Test", 0f, testRate);
    }

    public GameObject Play(ParticleEffect particleEffect, Transform transform, out List<ParticleSystem> particleSystems)
    {
        GameObject go = SpawnParticleEffect(particleEffect, transform);
        particleSystems = go.GetComponentsInChildren<ParticleSystem>().ToList();
        return go;
    }
    public GameObject Play(ParticleEffect particleEffect, Transform transform)
    {
        return SpawnParticleEffect(particleEffect, transform);
    }
    public GameObject Play(ParticleEffect particleEffect, Vector3 position)
    {
        GameObject pe = SpawnParticleEffect(particleEffect, position);
        return pe;
    }
    public GameObject Play(ParticleEffect particleEffect, Vector3 position, out List<ParticleSystem> particleSystems)
    {
        GameObject pe = SpawnParticleEffect(particleEffect, position);
        particleSystems = pe.GetComponentsInChildren<ParticleSystem>().ToList();
        return pe;
    }

    private GameObject SpawnParticleEffect(ParticleEffect particleEffect, Vector3 position)
    {
        GameObject pe = Instantiate(particleEffect.particleEffect);

        pe.name = particleEffect.name;
        pe.transform.parent = particleEffectHolder.transform;
        pe.transform.position = position;
        ApplyScaleModifier(particleEffect, pe);
        if(particleEffect.particleEffectVariables.lifeTime.destroy)
            Destroy(pe, particleEffect.particleEffectVariables.lifeTime.lifetime);

        return pe;
    }


    private GameObject SpawnParticleEffect(ParticleEffect particleEffect, Transform spawnerTransform)
    {
        if (spawnerTransform == null)
        {
            Debug.LogWarning(particleEffect.name + " trying to spawn particle effect with null transform ");
            return null;
        }

        GameObject pe = Instantiate(particleEffect.particleEffect);

        pe.name = particleEffect.name;
        SetParent(particleEffect, spawnerTransform, pe);       
        SetPositionParticleEffect(particleEffect, spawnerTransform, pe);
        ApplyScaleModifier(particleEffect, pe);

        if (particleEffect.particleEffectVariables.lifeTime.destroy)
            Destroy(pe, particleEffect.particleEffectVariables.lifeTime.lifetime);

        return pe;
    }

    private void SetParent(ParticleEffect particleEffect, Transform spawnerTransform, GameObject pe)
    {
        if (particleEffect.particleEffectVariables.child)
            pe.transform.parent = spawnerTransform;       
        else
            pe.transform.parent = particleEffectHolder.transform;
    }
    private void SetPositionParticleEffect(ParticleEffect particleEffect, Transform spawnerTransform, GameObject pe)
    {
        switch (particleEffect.particleEffectVariables.offsetSpace)
        {
            case ParticleEffect.OffsetSpace.Local:
                Vector3 localOffset = spawnerTransform.localRotation * particleEffect.particleEffectVariables.positionOffset;
                pe.transform.localPosition = particleEffect.particleEffectVariables.child ? particleEffect.particleEffectVariables.positionOffset : spawnerTransform.position + localOffset;
                break;
            case ParticleEffect.OffsetSpace.World:
                pe.transform.position = spawnerTransform.position + particleEffect.particleEffectVariables.positionOffset;
                break;
        }

        switch (particleEffect.particleEffectVariables.rotationSpace)
        {
            case ParticleEffect.RotationSpace.Local:
                pe.transform.rotation = spawnerTransform.rotation;
                break;
        }
    }
    private void ApplyScaleModifier(ParticleEffect particleEffect, GameObject pe)
    {
        if (particleEffect.particleEffectVariables.scaleMultiplier == 1f)
            return;

        ParticleSystem[] allParticleSystems = pe.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < allParticleSystems.Length; i++)
        {
            allParticleSystems[i].transform.localScale = allParticleSystems[i].transform.localScale * particleEffect.particleEffectVariables.scaleMultiplier;
        }
    }

    private void Test()
    {
        foreach (ParticleEffect particleEffect in testParticleEffects)
        {
            Debug.Log("Testing particle effect " + particleEffect.name);
            Play(particleEffect, transform);
        }
    }
}

