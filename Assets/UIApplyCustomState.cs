using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIApplyCustomState : MonoBehaviour
{
    public TMP_InputField CustomStateInputField;
    public TextMeshProUGUI InvalidStateErrorText;

    private CubeSolver cubeSolver;
    // Start is called before the first frame update
    void Start()
    {
        cubeSolver = FindObjectOfType<CubeSolver>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FUBBURBUBRFDRRURRBUFUDFDLDUULFBDBDLRDLLULFRDFLRLLBBDFF
    private bool ValidateCustomState(string stateString)
    {
        if (stateString.Length != 54) return false;

        char[] validChar = new char[] { 'U', 'L', 'F', 'R', 'B', 'D' };

        bool[] centerPieces = new bool[] {
            stateString[04] == 'U',
            stateString[40] == 'L',
            stateString[22] == 'F',
            stateString[13] == 'R',
            stateString[49] == 'B',
            stateString[31] == 'D'
        };

        if (centerPieces.Contains(false)) return false;

        IDictionary<char, int> pieceCount = new Dictionary<char, int> {
            {'U',0}, {'L',0}, {'F',0}, {'R',0}, {'B',0}, {'D',0}
        };
        
        for (int i = 0; i < stateString.Length; i++)
        {
            if (!validChar.Contains(stateString[i])) return false;

            pieceCount[stateString[i]] = pieceCount[stateString[i]] + 1;
            if (pieceCount[stateString[i]] > 9) return false;
        }

        return true;
    }

    public void ApplyCustomState()
    {
        if (CubeState.autoRotating) return;

        InvalidStateErrorText.text = string.Empty;
        string stateString = CustomStateInputField.text;

        if (ValidateCustomState(stateString))
        {
            cubeSolver.SolveWithCustomState(stateString);
        }
        else
        {
            InvalidStateErrorText.text = "ERROR: Invalid State String";
        }
    }
}
