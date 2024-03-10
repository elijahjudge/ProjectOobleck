using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInFrontEnableBehind : MonoBehaviour
{
    public Transform mover;
    public List<GameObject> gameObjects;


    private void FixedUpdate()
    {
        if(mover.transform.position.z < transform.position.z)
        {
            foreach(GameObject go in gameObjects)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in gameObjects)
            {
                go.SetActive(true);
            }
        }
    }
}
