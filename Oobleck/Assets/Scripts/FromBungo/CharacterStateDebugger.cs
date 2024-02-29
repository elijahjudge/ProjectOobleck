using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterStateDebugger : MonoBehaviour
{
    public string currentState;

    [Header("Debug Overhead")]
    public bool showStateAsText;
    public TextMeshPro tmp;
    public Vector3 transformOffset;

    private TextMeshPro overheadText;
    // Start is called before the first frame update
    void Awake()
    {
        ManagerCharacterState.stateChanged += StateChanged;

        if (showStateAsText)
            overheadText = Instantiate(tmp, transform.position + transformOffset, Quaternion.Euler(0f, 0f, 0f));

    }

    private void FixedUpdate()
    {
        if (overheadText == null || !showStateAsText)
            return;

        overheadText.gameObject.transform.position = transform.position + transformOffset;
    }
    public void StateChanged(GameObject gameObject, State newState)
    {
        if (this.gameObject != gameObject)
            return;

        currentState = newState.ToString();

        if(overheadText != null)
            overheadText.text = currentState;
    }

    private void OnDisable()
    {
        ManagerCharacterState.stateChanged -= StateChanged;

        if (overheadText != null)
            Destroy(overheadText.gameObject);
    }

}


