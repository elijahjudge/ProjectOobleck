using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    public GameObject canvas;
    public GameObject startCamera;

    private void Awake()
    {
        SceneManager.sceneLoaded += HideEverything;
    }
    void Start()
    {
        SceneManager.LoadSceneAsync("SampleScene",LoadSceneMode.Additive);
    }

    public void HideEverything(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            Destroy(canvas);
            Destroy(startCamera);
        }
    }
}
