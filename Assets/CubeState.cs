using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState : MonoBehaviour
{

    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List <GameObject> right = new List<GameObject>();

    public static bool autoRotating = false;
    public static bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickUp(List<GameObject> cubeSide)
    {
        foreach (GameObject face in cubeSide)
        {
            if (face != cubeSide[4])
            {
                face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }
        }
    }

    public void putDown(List<GameObject> cubes, Transform pivot)
    {
        foreach(GameObject cube in cubes)
        {
            if (cube != cubes[4])
            {
                cube.transform.parent.transform.parent = pivot;
            }
        }
    }

    public string GetSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach(GameObject face in side)
        {
            sideString += face.name[0];
        }
        return sideString;
    }

    public string GetStateString()
    {
        string stateString = "";

        stateString += GetSideString(up);
        stateString += GetSideString(right);
        stateString += GetSideString(front);
        stateString += GetSideString(down);
        stateString += GetSideString(left);
        stateString += GetSideString(back);

        return stateString;
    }
}
