using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject canvasPauseMenu;

    public InputReader playerInput;

    private void OnEnable()
    {
        playerInput.myPlayer.actions.FindAction("ButtonSouth").performed += pick;

        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        playerInput.myPlayer.actions.FindAction("ButtonSouth").performed -= pick;
    }


    public void pick(InputAction.CallbackContext ctx)
    {
        canvasPauseMenu.SetActive(false);
    }

}
