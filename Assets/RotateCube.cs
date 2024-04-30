using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{

    enum swipeDirections
    {
        Left, Right, UpLeft, UpRight, DownLeft, DownRight
    }

    IDictionary<swipeDirections, Action> swipeActionMapping;

    Vector2 pressPos = Vector2.zero;
    Vector2 releasePos = Vector2.zero;

    Vector3 previousMousePos = Vector3.zero;

    float rotationSpeed = 200f;

    public GameObject rotationTarget;

    // Start is called before the first frame update
    void Start()
    {
        swipeActionMapping = new Dictionary<swipeDirections, Action>
        {
            {swipeDirections.Left, () => { RotateLeft(); } },
            {swipeDirections.Right, () => { RotateRight(); } },
            {swipeDirections.UpLeft, () => { RotateUpLeft(); } },
            {swipeDirections.UpRight, () => { RotateUpRight(); } },
            {swipeDirections.DownLeft, () => { RotateDownLeft(); } },
            {swipeDirections.DownRight, () => { RotateDownRight(); } },
        };
    }

    // Update is called once per frame
    void Update()
    {
        GetSwipe();
        DragCube();
    }

    void DragCube()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - previousMousePos;
            mouseDelta *= 0.1f;
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
        }
        else
        {
            UpdateCubePosition();
        }

        previousMousePos = Input.mousePosition;
    }

    void UpdateCubePosition()
    {
        if (transform.rotation != rotationTarget.transform.rotation)
        {
            var step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationTarget.transform.rotation, step);
        }
    }

    // Receive and apply swipe to dummy rotation object
    void GetSwipe()
    {   
        // Mouse is pressed
        // Get initial mouse position
        if (Input.GetMouseButtonDown(1))
        {
            pressPos.Set(Input.mousePosition.x, Input.mousePosition.y);
            return;
        }

        // Mouse is released
        // Get final mouse position
        if (Input.GetMouseButtonUp(1))
        {
            releasePos.Set(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 swipe = CalcSwipe(pressPos, releasePos);
            DoRotate(swipe);
            return;
        }

    }

    // Calculate swipe vector
    Vector2 CalcSwipe(Vector2 initialPos, Vector2 finalPos)
    {
        Vector2 swipe = new Vector2(finalPos.x - initialPos.x, finalPos.y - initialPos.y);
        swipe.Normalize();
        return swipe;
    }

    // Perform rotation
    void DoRotate(Vector2 swipe)
    {
        swipeDirections swipeDirection = GetSwipeDirection(swipe);
        swipeActionMapping[swipeDirection]();
    }

    // Determine Swipe Direction
    swipeDirections GetSwipeDirection(Vector2 swipe)
    {
        // Left and Right swipes
        if (swipe.y > -0.5f && swipe.y < 0.5f)
        {
            return swipe.x < 0 ? swipeDirections.Left : swipeDirections.Right;
        }

        // UpLeft and UpRight swipes
        if (swipe.y > 0)
        {
            return swipe.x < 0f ? swipeDirections.UpLeft : swipeDirections.UpRight;
        }

        // DownLeft and DownRight swipes
        return swipe.x < 0f ? swipeDirections.DownLeft : swipeDirections.DownRight;
    }

    // Rotation Functions
    void RotateLeft()
    {
        rotationTarget.transform.Rotate(0, 90, 0, Space.World);
    }

    void RotateRight()
    {
        rotationTarget.transform.Rotate(0, -90, 0, Space.World);
    }

    void RotateUpLeft()
    {
        rotationTarget.transform.Rotate(90, 0, 0, Space.World);
    }

    void RotateUpRight()
    {
        rotationTarget.transform.Rotate(0, 0, -90, Space.World);
    }

    void RotateDownLeft()
    {
        rotationTarget.transform.Rotate(0, 0, 90, Space.World);
    }

    void RotateDownRight()
    {
        rotationTarget.transform.Rotate(-90, 0, 0, Space.World);
    }
}
