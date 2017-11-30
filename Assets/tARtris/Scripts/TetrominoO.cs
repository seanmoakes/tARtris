using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TetrominoO : Tetromino
{
    public void ClockWise()
    {
        PlayRotateClkAudio();
    }

    public void AntiClockWise()
    {
        PlayRotateAntiClkAudio();
    }

    void Update()
    {
        UpdateMovement();
        if (Input.GetKeyDown(KeyCode.B) || CrossPlatformInputManager.GetButtonDown("Clockwise"))
        {
            ClockWise();
        }
        if (Input.GetKeyDown(KeyCode.V) || CrossPlatformInputManager.GetButtonDown("AntiClockwise"))
        {
            AntiClockWise();
        }      
    }
}
