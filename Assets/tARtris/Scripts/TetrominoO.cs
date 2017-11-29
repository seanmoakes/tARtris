// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TetrominoO : Tetromino
{
    // Update is called once per frame
    void Update()
    {
        // setGhostTransform();
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
