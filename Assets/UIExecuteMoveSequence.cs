using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIExecuteMoveSequence : MonoBehaviour
{
    private readonly List<string> validMoves = new List<string>()
    {
        "U", "D", "L", "R", "F", "B",
        "U2", "D2", "L2", "R2", "F2", "B2",
        "U'", "D'", "L'", "R'", "F'", "B'"
    };

    public TMP_InputField MoveSequenceInputField;
    public TextMeshProUGUI InvalidMoveSequenceErrorText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool ValidateSequence(string[] moves)
    {
        foreach (string move in moves)
        {
            if (!validMoves.Contains(move)) return false;
        }
        return true;
    }
    public void ExecuteMoveSequence()
    {
        if (CubeState.autoRotating) return;

        InvalidMoveSequenceErrorText.text = string.Empty;
        string sequenceString = MoveSequenceInputField.text;
        string[] moves = sequenceString.Split(new char[0]);

        if (ValidateSequence(moves))
        {
            AutomaticMovement.moveList = moves.ToList();
        }
        else
        {
            InvalidMoveSequenceErrorText.text = "ERROR: Invalid Move Sequence";
        }
    }
}
