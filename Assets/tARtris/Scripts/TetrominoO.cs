using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoO : Tetromino
{
    // float lastFall = 0;
    // float lastLeft = 0;
    // float lastRight = 0;
    // bool isDownKeyHeld = false;
    // bool isLeftKeyHeld = false;
    // bool isRightKeyHeld = false;

    // Update is called once per frame
    void Update()
    {
        isDownKeyHeld = Input.GetKey(KeyCode.DownArrow);
        isLeftKeyHeld = Input.GetKey(KeyCode.LeftArrow);
        isRightKeyHeld = Input.GetKey(KeyCode.RightArrow);

        //Left or right movement
        if (isLeftKeyHeld && !isRightKeyHeld && Time.time - lastLeft >= 0.1)
        {
            moveLeft();
            lastLeft = Time.time;
        }
        else if (isRightKeyHeld && !isLeftKeyHeld && Time.time - lastRight >= 0.1)
        {
            moveRight();
            lastRight = Time.time;
        }

        //Move down the screen
        if (isDownKeyHeld && Time.time - lastFall >= 0.1)
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
