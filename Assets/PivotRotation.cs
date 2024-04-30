using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private Vector3 mouseRef;
    private bool dragging = false;

    private bool autoRotating = false;

    private Quaternion targetQuaternion;

    private float sensitivity = 0.4f;
    private float speed = 300f;
    private Vector3 rotation;

    private ReadCube readCube;
    private CubeState cubeState;
    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindAnyObjectByType<CubeState>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (dragging && !autoRotating)
        {
            spinSide(activeSide);
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                rotateToRightAngle();
            }
        }
        if (autoRotating) autoRotate();
    }

    private void spinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;

        Vector3 mouseOffset = (Input.mousePosition - mouseRef);

        if (side == cubeState.front) rotation.x = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        if (side == cubeState.back)  rotation.x = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        if (side == cubeState.up)    rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        if (side == cubeState.down)  rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        if (side == cubeState.left)  rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * 1;
        if (side == cubeState.right) rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;

        transform.Rotate(rotation, Space.Self);
        mouseRef = Input.mousePosition;
    }

    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Input.mousePosition;
        dragging = true;

        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition; 
    }

    public void startAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.pickUp(side);

        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;
    }

    public void rotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;

        autoRotating = true;
    }

    private void autoRotate()
    {
        dragging = false;
        var step = speed * Time.deltaTime;

        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);
        
        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;
            cubeState.putDown(activeSide, transform.parent);
            readCube.ReadState();
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }
}
