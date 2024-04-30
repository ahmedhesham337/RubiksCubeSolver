using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStateStringUpdater : MonoBehaviour
{
    public TextMeshProUGUI stateStringText;

    private CubeState cubeState;
    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        stateStringText.text = cubeState.GetStateString();
    }

    public void CopyStateToClipBoard()
    {
        if (CubeState.autoRotating) return;
        GUIUtility.systemCopyBuffer = stateStringText.text;
    }
}
