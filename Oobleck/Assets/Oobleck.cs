using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oobleck : MonoBehaviour
{
    [SerializeField] private Transform particlePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Transform GetParticleTransform()
    {
        return particlePosition;
    }
}
