using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRespawnPosition : MonoBehaviour
{
    public Vector3 spawnPosition;

    private void Start()
    {
        CheckPoint.checkPointTouched += UpdateRespawnPosition;
    }
    public void UpdateRespawnPosition(Vector3 newPosition, int checkPoint)
    {
        spawnPosition = newPosition;
    }
}
