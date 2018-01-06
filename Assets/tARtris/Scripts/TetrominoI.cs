//using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

//public class TetrominoI : Tetromino
//{
//    public RotationChecks rotationChecks;
//    public void ClockWise()
//    {
//        int[] testInputs = new int[10];
//        switch (currentRotation)
//        {
//            case RotateState.Zero:
//                desiredRotation = RotateState.Right;
//                testInputs = rotationChecks?.ZeroClockwise;
//                break;
//            case RotateState.Right:
//                desiredRotation = RotateState.Two;
//                testInputs = rotationChecks?.RightClockwise;
//                break;
//            case RotateState.Two:
//                desiredRotation = RotateState.Left;
//                testInputs = rotationChecks?.TwoClockwise;
//                break;
//            case RotateState.Left:
//                desiredRotation = RotateState.Zero;
//                testInputs = rotationChecks?.LeftClockwise;
//                break;
//        }
//        Vector3 pos = transform.GetChild(4).position;
//        transform.RotateAround(pos, Vector3.forward, -90.0f);

//        if (TestPositions(testInputs))
//        {
//            currentRotation = desiredRotation;
//            PlayRotateClkAudio();
//        }
//        else
//        {
//            transform.RotateAround(pos, Vector3.forward, 90.0f);
//            //PlayRotateErrorAudio();
//        }
//        FindObjectOfType<Tartris>().UpdateGrid(this);
//    }

//    public void AntiClockWise()
//    {
//        int[] testInputs = null;
//        switch (currentRotation)
//        {
//            case RotateState.Zero:
//                desiredRotation = RotateState.Left;
//                testInputs = rotationChecks?.ZeroAntiClockwise;
//                break;
//            case RotateState.Right:
//                desiredRotation = RotateState.Zero;
//                testInputs = rotationChecks?.RightAntiClockwise;
//                break;
//            case RotateState.Two:
//                desiredRotation = RotateState.Right;
//                testInputs = rotationChecks?.TwoAntiClockwise;
//                break;
//            case RotateState.Left:
//                desiredRotation = RotateState.Two;
//                testInputs = rotationChecks?.LeftAntiClockwise;
//                break;
//        }
//        Vector3 pos = transform.GetChild(4).position;
//        transform.RotateAround(pos, Vector3.forward, 90.0f);
//        if (TestPositions(testInputs))
//        {
//            currentRotation = desiredRotation;
//            PlayRotateAntiClkAudio();
//        }
//        else
//        {
//            transform.RotateAround(pos, Vector3.forward, -90.0f);
//        }
//        FindObjectOfType<Tartris>().UpdateGrid(this);
//    }

//    void Update()
//    {
//        UpdateMovement();
//        if (Input.GetKeyDown(KeyCode.B) || CrossPlatformInputManager.GetButtonDown("Clockwise"))
//        {
//            ClockWise();
//        }
//        if (Input.GetKeyDown(KeyCode.V) || CrossPlatformInputManager.GetButtonDown("AntiClockwise"))
//        {
//            AntiClockWise();
//        }
        
//    }
//}
