using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TetrominoI : Tetromino
{
    public void ClockWise()
    {
        int[] testInputs = new int[10];
        switch (currentRotation)
        {
            case RotateState.Zero:
                desiredRotation = RotateState.Right;
                testInputs = new int[] { 0, 0,  -2, 0,  1, 0,  -2, -1,  1, 2 };
                break;
            case RotateState.Right:
                desiredRotation = RotateState.Two;
                testInputs = new int[] { 0, 0,  -1, 0,  2, 0,  -1, 2,  2, -1 };
                break;
            case RotateState.Two:
                desiredRotation = RotateState.Left;
                testInputs = new int[] { 0, 0,  2, 0,  -1, 0,  2, 1,  -1, -2 };
                break;
            case RotateState.Left:
                desiredRotation = RotateState.Zero;
                testInputs = new int[] { 0, 0,  1, 0,  -2, 0,  1, -2, -2, 1 };
                break;
        }
        Vector3 pos = transform.GetChild(4).position;
        transform.RotateAround(pos, Vector3.forward, -90.0f);
        //transform.Rotate(0,0,-90);
        if (TestPositions(testInputs))
        {
            currentRotation = desiredRotation;
        }
        else
        {
            transform.RotateAround(pos, Vector3.forward, 90.0f);
        }
    }

    public void AntiClockWise()
    {
        int[] testInputs = new int[10];
        switch (currentRotation)
        {
            case RotateState.Zero:
                desiredRotation = RotateState.Left;
                testInputs = new int[]{ 0, 0,  -1, 0,  2, 0,  -1, 2,  2, -1 };
                break;
            case RotateState.Right:
                desiredRotation = RotateState.Zero;
                testInputs = new int[] { 0, 0,  2, 0,  -1, 0,  2, 1,  -1, -2 };
                break;
            case RotateState.Two:
                desiredRotation = RotateState.Right;
                testInputs = new int[] { 0, 0,  1, 0,  -2, 0,  1, -2,  -2, 1 };
                break;
            case RotateState.Left:
                desiredRotation = RotateState.Two;
                testInputs = new int[] { 0, 0,  -2, 0,  1, 0,  -2, -1,  1, 2 };
                break;
        }
        Vector3 pos = transform.GetChild(4).position;
        transform.RotateAround(pos, Vector3.forward, 90.0f);
        if (TestPositions(testInputs))
        {
            currentRotation = desiredRotation;
        }
        else
        {
            transform.RotateAround(pos, Vector3.forward, -90.0f);
        }
    }

    void Update()
    {
        Vector2 axes = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        isDownKeyHeld = Input.GetKey(KeyCode.DownArrow);
        isLeftKeyHeld = Input.GetKey(KeyCode.LeftArrow);
        isRightKeyHeld = Input.GetKey(KeyCode.RightArrow);

        //Left or right movement
        if (
            (isLeftKeyHeld && !isRightKeyHeld || axes.x < -0.5f)
            && Time.time - lastLeft >= 0.1)
        {
            moveLeft();
            lastLeft = Time.time;
        }
        else if (
            (isRightKeyHeld && !isLeftKeyHeld || axes.x > 0.5)
            && Time.time - lastRight >= 0.1)
        {
            moveRight();
            lastRight = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.B) || CrossPlatformInputManager.GetButtonDown("Clockwise"))
        {
            ClockWise();
        }
        if (Input.GetKeyDown(KeyCode.V) || CrossPlatformInputManager.GetButtonDown("AntiClockwise"))
        {
            AntiClockWise();
        }

        //Move down the screen
        if ((isDownKeyHeld || axes.y < -0.5) && Time.time - lastFall >= 0.1)
        {
            moveDown();
            lastFall = Time.time;
        }
        else if (Time.time - lastFall >= 1)
        {
            moveDown();
            lastFall = Time.time;
        }
    }
}
