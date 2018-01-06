//using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

//public class TetrominoJLSTZ : Tetromino
//{
//    public void ClockWise()
//    {
//        int[] testInputs = new int[10];
//        switch (currentRotation)
//        {
//            case RotateState.Zero:
//                desiredRotation = RotateState.Right;
//                testInputs = new int[] { 0, 0, -1, 0, -1, 1, 0, -2, -1, -2 };
//                break;
//            case RotateState.Right:
//                desiredRotation = RotateState.Two;
//                testInputs = new int[] { 0, 0, 1, 0, 1, -1, 0, 2, 1, 2 };
//                break;
//            case RotateState.Two:
//                desiredRotation = RotateState.Left;
//                testInputs = new int[] { 0, 0, 1, 0, 1, 1, 0, -2, 1, -2 };
//                break;
//            case RotateState.Left:
//                desiredRotation = RotateState.Zero;
//                testInputs = new int[] { 0, 0, -1, 0, -1, -1, 0, 2, -1, 2 };
//                break;
//        }
//        Vector3 pos = transform.GetChild(4).position;
//        transform.RotateAround(pos, Vector3.forward, -90.0f);
//        //transform.Rotate(0,0,-90);
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
//        int[] testInputs = new int[10];
//        switch (currentRotation)
//        {
//            case RotateState.Zero:
//                desiredRotation = RotateState.Left;
//                testInputs = new int[] { 0, 0, 1, 0, 1, 1, 0, -2, 1, -2 };
//                break;
//            case RotateState.Right:
//                desiredRotation = RotateState.Zero;
//                testInputs = new int[] { 0, 0, 1, 0, 1, -1, 0, 2, 1, 2 };
//                break;
//            case RotateState.Two:
//                desiredRotation = RotateState.Right;
//                testInputs = new int[] { 0, 0, -1, 0, -1, 1, 0, -2, -1, -2 };
//                break;
//            case RotateState.Left:
//                desiredRotation = RotateState.Two;
//                testInputs = new int[] { 0, 0, -1, 0, -1, -1, 0, 2, -1, 2 };
//                break;
//        }
//        Vector3 pos = transform.GetChild(4).position;
//        transform.RotateAround(pos, Vector3.forward, 90.0f);
//        //transform.Rotate(0,0,-90);
//        if (TestPositions(testInputs))
//        {
//            currentRotation = desiredRotation;
//            PlayRotateAntiClkAudio();
//        }
//        else
//        {
//            transform.RotateAround(pos, Vector3.forward, -90.0f);
//            //PlayRotateErrorAudio();
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
