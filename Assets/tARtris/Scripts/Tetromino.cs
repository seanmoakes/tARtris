using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateState
{           /// Names for the 4 possible rotations of a tetromino.
    Zero,   /// Starting position
    Right,  /// Start +90 degrees
    Two,    /// Start + 180 degrees
    Left    /// Start + 270 degrees
}

public class Tetromino : MonoBehaviour
{
    protected float lastFall = 0;
    protected float lastLeft = 0;
    protected float lastRight = 0;
    protected bool isDownKeyHeld = false;
    protected bool isLeftKeyHeld = false;
    protected bool isRightKeyHeld = false;
    public RotateState currentRotation = RotateState.Zero;
    public RotateState desiredRotation = RotateState.Zero;
    public bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            if (child != transform.GetChild(4))
            {
                Vector2 v = TetrisGrid.roundVec2(child.position);
                if (!TetrisGrid.insideBorder(v))
                {
                    return false;
                }

                if (TetrisGrid.grid[(int)v.x, (int)v.y] != null &&
                   TetrisGrid.grid[(int)v.x, (int)v.y].parent != transform)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void updateGrid()
    {
        for (int y = 0; y < TetrisGrid.h; y++)
        {
            for (int x = 0; x < TetrisGrid.w; x++)
            {
                if (TetrisGrid.grid[x, y] != null)
                {
                    if (TetrisGrid.grid[x, y].parent == transform)
                    {
                        TetrisGrid.grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform child in transform)
        {
            if (child != transform.GetChild(4))
            {
                Vector2 v = TetrisGrid.roundVec2(child.position);
                TetrisGrid.grid[(int)v.x, (int)v.y] = child;
            }
        }
    }
    // Use this for initialization
    public void Start()
    {
        if (!isValidGridPos())
        {
            Debug.Log("Game Over");
            Destroy(gameObject);
        }
    }

    public bool TestPosition(int x, int y)
    {
        transform.position += new Vector3(x, y, 0);
        if (isValidGridPos())
        {
            updateGrid();
            return true;
        }
        transform.position -= new Vector3(x, y, 0);
        return false;
    }
    public bool TestPositions(int[] testInputs)
    {
        if (TestPosition(testInputs[0], testInputs[1]))
        {
            return true;
        }
        else if (TestPosition(testInputs[2], testInputs[3]))
        {
            return true;
        }
        else if (TestPosition(testInputs[4], testInputs[5]))
        {
            return true;
        }
        else if (TestPosition(testInputs[6], testInputs[7]))
        {
            return true;
        }
        else if (TestPosition(testInputs[8], testInputs[9]))
        {
            return true;
        }
        return false;
    }
    
    // Update is called once per frame
    void Update()
    {
//         isDownKeyHeld = Input.GetKey(KeyCode.DownArrow);
//         isLeftKeyHeld = Input.GetKey(KeyCode.LeftArrow);
//         isRightKeyHeld = Input.GetKey(KeyCode.RightArrow);
// 
//         //Left or right movement
//         if (isLeftKeyHeld && !isRightKeyHeld && Time.time - lastLeft >= 0.1)
//         {
//             moveLeft();
//             lastLeft = Time.time;
//         }
//         else if (isRightKeyHeld && !isLeftKeyHeld && Time.time - lastRight >= 0.1)
//         {
//             moveRight();
//             lastRight = Time.time;
//         }
// 
//         //Move down the screen
//         if (isDownKeyHeld && Time.time - lastFall >= 0.1)
//         {
//             moveDown();
//             lastFall = Time.time;
//         }
//         else if (Time.time - lastFall >= 1)
//         {
//             moveDown();
//             lastFall = Time.time;
//         }
    }

    public void moveDown()
    {
        transform.position += new Vector3(0, -1, 0);

        if (isValidGridPos())
        {
            updateGrid();
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            TetrisGrid.deleteFullRows();

            FindObjectOfType<Spawner>().spawnNext();
            enabled = false;
            Destroy(transform.GetChild(4).gameObject);
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }
    public void moveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);

        if (isValidGridPos())
        {
            updateGrid();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    public void moveRight()
    {
        transform.position += new Vector3(1, 0, 0);

        if (isValidGridPos())
        {
            updateGrid();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }
}
