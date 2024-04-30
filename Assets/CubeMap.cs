using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeMap : MonoBehaviour
{
    CubeState cubeState;

    public Transform up;
    public Transform down;
    public Transform left;
    public Transform right;
    public Transform front;
    public Transform back;

    private IDictionary<string, Color> faceColorMap = new Dictionary<string, Color> {
        { "F", new Color(1, 0.5f, 0, 1) },
        { "B", Color.red },
        { "U", Color.yellow },
        { "D", Color.white },
        { "L", Color.green },
        { "R", Color.blue },
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set()
    {
        cubeState = FindObjectOfType<CubeState>();

        updateMap(cubeState.front, front);
        updateMap(cubeState.back, back);
        updateMap(cubeState.left, left);
        updateMap(cubeState.right, right);
        updateMap(cubeState.up, up);
        updateMap(cubeState.down, down);
    }

    void updateMap(List<GameObject> face, Transform side) {
        int i = 0;
        foreach (Transform map in side)
        {
            map.GetComponent<Image>().color = faceColorMap[face[i].name[0].ToString()];
            i++;
        }
    }
}
