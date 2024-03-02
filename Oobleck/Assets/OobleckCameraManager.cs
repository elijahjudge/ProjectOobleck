using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckCameraManager : MonoBehaviour
{
    public List<Camera> oobleckRenderCameras;


    public int indexCurrentCamera;

    private void Start()
    {
        UseCamera(indexCurrentCamera);
    }
    public void UseCamera(int index)
    {
        foreach(var cam in oobleckRenderCameras)
        {
                cam.transform.root.gameObject.SetActive(false);
        }

        oobleckRenderCameras[index].transform.root.gameObject.SetActive(true);
    }
}
