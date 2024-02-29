using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    // Start is called before the first frame update
    void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
        CharacterTarget.targetAdded += AddPlayer;
    }

    public void AddPlayer(Transform transform)
    {
        targetGroup.AddMember(transform, 1, 2f);
    }
}
