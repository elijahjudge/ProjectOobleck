using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OobleckCameraManager : MonoBehaviour
{
    public List<Camera> oobleckRenderCameras;


    public int indexCurrentCamera;

    // so the color of the render cameras can update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f / 60f);

        UseCamera(indexCurrentCamera);
        CheckPoint.checkPointTouched += TouchedCheckPoint;
            
        
    }
    public void UseCamera(int index)
    {
        indexCurrentCamera = index;

        foreach(var cam in oobleckRenderCameras)
        {
                cam.transform.gameObject.SetActive(false);
        }

        oobleckRenderCameras[index].transform.gameObject.SetActive(true);
    }

    public void  TouchedCheckPoint(Vector3 position, int checkPoint)
    {
        UseCamera(checkPoint);
    }
}
