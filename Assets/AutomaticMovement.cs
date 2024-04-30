using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AutomaticMovement : MonoBehaviour
{

    public static List<string> moveList = new List<string>() { };

    private readonly List<string> validMoves = new List<string>()
    {
        "U", "D", "L", "R", "F", "B",
        "U2", "D2", "L2", "R2", "F2", "B2",
        "U'", "D'", "L'", "R'", "F'", "B'"
    };
    IDictionary<string, Action> MoveActionMapping;

    private CubeState cubeState;
    private ReadCube readCube;

    // Start is called before the first frame update
    void Start()
    {
        MoveActionMapping = new Dictionary<string, Action>
        {
            {
                "U", () => { 
                    RotateSide(cubeState.up, -90); 
                } 
            },
            {
                "U'", () => { 
                    RotateSide(cubeState.up, 90); 
                } 
            },
            {
                "U2", () => { 
                    RotateSide(cubeState.up, -180); 
                } 
            },

            {
                "D", () => { 
                    RotateSide(cubeState.down, -90); 
                } 
            },
            {
                "D'", () => { 
                    RotateSide(cubeState.down, 90); 
                } 
            },
            {
                "D2", () => { 
                    RotateSide(cubeState.down, -180); 
                } 
            },

            {
                "L", () => { 
                    RotateSide(cubeState.left, -90); 
                } 
            },
            {
                "L'", () => { 
                    RotateSide(cubeState.left, 90); 
                } 
            },
            {
                "L2", () => { 
                    RotateSide(cubeState.left, -180); 
                } 
            },

            {
                "R", () => { 
                    RotateSide(cubeState.right, -90); 
                } 
            },
            {
                "R'", () => { 
                    RotateSide(cubeState.right, 90); 
                } 
            },
            {
                "R2", () => { 
                    RotateSide(cubeState.right, -180); 
                } 
            },

            {
                "F", () => { 
                    RotateSide(cubeState.front, -90); 
                } 
            },
            {
                "F'", () => { 
                    RotateSide(cubeState.front, 90); 
                } 
            },
            {
                "F2", () => { 
                    RotateSide(cubeState.front, -180); 
                } 
            },

            {
                "B", () => { 
                    RotateSide(cubeState.back, -90); 
                } 
            },
            {
                "B'", () => { 
                    RotateSide(cubeState.back, 90); 
                } 
            },
            {
                "B2", () => { 
                    RotateSide(cubeState.back, -180); 
                } 
            }
        };

        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveList.Count > 0 && !CubeState.autoRotating && CubeState.initialized)
        {
            DoMove(moveList[0]);
            moveList.Remove(moveList[0]);
        }
        
    }

    public void shuffle()
    {
        if (CubeState.autoRotating) return;

        List<string> moves = new List<string>();
        int numMoves = UnityEngine.Random.Range(15, 35);
        for (int i = 0; i < numMoves; i++)
        {
            int randomMove = UnityEngine.Random.Range(0, validMoves.Count);
            moves.Add(validMoves[randomMove]);
        }
        moveList = moves;
    }
    void DoMove(string move)
    {
        readCube.ReadState();
        CubeState.autoRotating = true;
        MoveActionMapping[move]();
    }

    void RotateSide(List<GameObject> side, float angle)
    {
        PivotRotation pr = side[4].transform.parent.GetComponent<PivotRotation>();
        pr.startAutoRotate(side, angle);
    }
}
