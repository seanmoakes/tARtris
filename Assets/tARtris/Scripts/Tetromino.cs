using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum RotateState
{           /// Names for the 4 possible rotations of a tetromino.
    Zero,   /// Starting position
    Right,  /// Start +90 degrees
    Two,    /// Start + 180 degrees
    Left    /// Start + 270 degrees
}

public class Tetromino : MonoBehaviour
{
    /* Input variables */
    protected bool isDownKeyHeld = false;
    protected bool isLeftKeyHeld = false;
    protected bool isRightKeyHeld = false;
    bool hardDrop = false;
    protected Vector2 axes = new Vector2(0.0f, 0.0f);

    /* Current and desired states used in checking for viable position after a rotation
     * and to employ SRS style rotation for wall kicks etc */
    public RotateState currentRotation = RotateState.Zero;
    public RotateState desiredRotation = RotateState.Zero;
    public RotationChecks rotationChecks;


    /* Audio clips for the different moves, and a source for playing them
     * Source is destroyed 1s after block is locked in place */
    public bool gameOver = false;
    public Vector3 SpawnPosition;
    public AudioClip moveLeftSound;
    public AudioClip moveRightSound;
    public AudioClip rotateClockwiseSound;
    public AudioClip rotateAntiClockwiseSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    /* Movement variables */
    private float lateralDelay;
    private float LeftDelay;
    private float rightDelay;
    private float lDelayRemaining;
    private float rDelayRemaining;
    private float leftHeldTime = 0.0f;
    private float rightHeldTime = 0.0f;
    private float lateralSpeed = 0.05f;
    private float dropInterval;
    private float dropDelta = 0.0f;
    private float softDropInterval;
    bool leftRepeat = false;
    bool rightRepeat = false;

    public void Start()
    {
        transform.position = SpawnPosition;
        audioSource = GetComponent<AudioSource>();
        dropInterval = FindObjectOfType<Tartris>().GetDropSpeed();
        softDropInterval = dropInterval / 20.0f;
        lateralDelay = FindObjectOfType<Tartris>().horizontalDelay;
        rightDelay = lateralDelay;
        LeftDelay = lateralDelay;
    }

    void Update()
    {
        UpdateMovement();
        UpdateRotation();

    }
    private void UpdateRotation()
    {
        if (Input.GetKeyDown(KeyCode.B) || CrossPlatformInputManager.GetButtonDown("Clockwise"))
        {
            ClockWise();
        }
        if (Input.GetKeyDown(KeyCode.V) || CrossPlatformInputManager.GetButtonDown("AntiClockwise"))
        {
            AntiClockWise();
        }
    }
    /*****************/
    /* Movement Loop */
    /*****************/
    protected void UpdateMovement()
    {
        UpdateMovementInputs();

        UpdateLateralMovement();

        UpdateVerticalMovement();
    }
    private void UpdateLateralMovement()
    {
        if (isLeftKeyHeld && isRightKeyHeld)
        {

        }
        else if (isLeftKeyHeld || axes.x < -0.5)
        {
            rightHeldTime = 0.0f;
            rightDelay = lateralDelay;
            rightRepeat = true;
            if (!leftRepeat)
            {
                moveLeft();
                leftRepeat = true;
            }
            else
            {
                if (LeftDelay > 0)
                {
                    LeftDelay -= Time.deltaTime;
                }
                else
                {
                    leftHeldTime += Time.deltaTime;
                    if (leftHeldTime >= lateralSpeed)
                    {
                        moveLeft();
                        leftHeldTime -= lateralSpeed;
                    }
                }
            }
        }
        else if (isRightKeyHeld || axes.x > 0.5)
        {
            leftHeldTime = 0.0f;
            LeftDelay = lateralDelay;
            leftRepeat = false;
            if (!rightRepeat)
            {
                moveRight();
                rightRepeat = true;
            }
            else
            {
                if (rightDelay > 0)
                {
                    rightDelay -= Time.deltaTime;
                }
                else
                {
                    rightHeldTime += Time.deltaTime;
                    if (rightHeldTime >= lateralSpeed)
                    {
                        moveRight();
                        rightHeldTime -= lateralSpeed;
                    }
                }
            }
        }
        else
        {
            leftHeldTime = 0;
            LeftDelay = lateralDelay;
            leftRepeat = false;
            rightHeldTime = 0;
            rightDelay = lateralDelay;
            rightRepeat = false;
        }
    }
    private void UpdateVerticalMovement()
    {
        if(!hardDrop)
        {
            float dropSpeed = isDownKeyHeld || axes.y < -0.5 ? softDropInterval : dropInterval;
            
            dropDelta += Time.deltaTime;
            if (dropDelta >= dropSpeed)
            {
                moveDown();
                dropDelta -= dropSpeed;
            }
        }
        else
        {
            moveDown();
            moveDown();
        }
    }
    private void UpdateMovementInputs()
    {
        axes = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        isDownKeyHeld = Input.GetKey(KeyCode.DownArrow);
        isLeftKeyHeld = Input.GetKey(KeyCode.LeftArrow);
        isRightKeyHeld = Input.GetKey(KeyCode.RightArrow);
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.DownArrow))
        {
            ResetDownVariables();
        }
    }


    public void ClockWise()
    {
        int[] testInputs = new int[10];
        switch (currentRotation)
        {
            case RotateState.Zero:
                desiredRotation = RotateState.Right;
                testInputs = rotationChecks?.ZeroClockwise;
                break;
            case RotateState.Right:
                desiredRotation = RotateState.Two;
                testInputs = rotationChecks?.RightClockwise;
                break;
            case RotateState.Two:
                desiredRotation = RotateState.Left;
                testInputs = rotationChecks?.TwoClockwise;
                break;
            case RotateState.Left:
                desiredRotation = RotateState.Zero;
                testInputs = rotationChecks?.LeftClockwise;
                break;
        }
        Vector3 pos = transform.GetChild(4).position;
        transform.RotateAround(pos, Vector3.forward, -90.0f);

        if (TestPositions(testInputs))
        {
            currentRotation = desiredRotation;
            PlayRotateClkAudio();
        }
        else
        {
            transform.RotateAround(pos, Vector3.forward, 90.0f);
            //PlayRotateErrorAudio();
        }
        FindObjectOfType<Tartris>().UpdateGrid(this);
    }

    public void AntiClockWise()
    {
        int[] testInputs = null;
        switch (currentRotation)
        {
            case RotateState.Zero:
                desiredRotation = RotateState.Left;
                testInputs = rotationChecks?.ZeroAntiClockwise;
                break;
            case RotateState.Right:
                desiredRotation = RotateState.Zero;
                testInputs = rotationChecks?.RightAntiClockwise;
                break;
            case RotateState.Two:
                desiredRotation = RotateState.Right;
                testInputs = rotationChecks?.TwoAntiClockwise;
                break;
            case RotateState.Left:
                desiredRotation = RotateState.Two;
                testInputs = rotationChecks?.LeftAntiClockwise;
                break;
        }
        Vector3 pos = transform.GetChild(4).position;
        transform.RotateAround(pos, Vector3.forward, 90.0f);
        if (TestPositions(testInputs))
        {
            currentRotation = desiredRotation;
            PlayRotateAntiClkAudio();
        }
        else
        {
            transform.RotateAround(pos, Vector3.forward, -90.0f);
        }
        FindObjectOfType<Tartris>().UpdateGrid(this);
    }

    /******************/
    /* Movement Tests */
    /******************/
    public bool TestPosition(int x, int y)
    {
        transform.position += new Vector3(x, y, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<Tartris>().UpdateGrid(this);
            return true;
        }
        transform.position -= new Vector3(x, y, 0);
        return false;
    }
    public bool TestPositions(int[] testInputs)
    {
        if (testInputs != null)
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
        }
        return false;
    }
    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            if (mino != transform.GetChild(4))
            {
                Vector2 pos = FindObjectOfType<Tartris>().roundVec2(mino.position);
                if (FindObjectOfType<Tartris>().CheckIsInsideGrid(pos) == false)
                {
                    return false;
                }

                if (FindObjectOfType<Tartris>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Tartris>().GetTransformAtGridPosition(pos).parent != transform)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /**********************/
    /* Movement Utilities */
    /**********************/
    public void ResetDownVariables()
    {
        dropDelta = 0.0f;
    }
    public void moveDown()
    {
        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Tartris>().UpdateGrid(this);
        }
        else
        {
            PlayLandAudio();
            Destroy(audioSource, 1f);

            transform.position += new Vector3(0, 1, 0);

            Destroy(transform.GetChild(4).gameObject);

            FindObjectOfType<Tartris>().DeleteRow();

            if (FindObjectOfType<Tartris>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Tartris>().GameOver();
            }

            enabled = false;
            hardDrop = false;
            FindObjectOfType<Tartris>().SpawnNextTARtrimino();
        }
    }
    public void moveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Tartris>().UpdateGrid(this);
            PlayMoveLeftAudio();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }
    public void moveRight()
    {
        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Tartris>().UpdateGrid(this);
            PlayMoveRightAudio();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    /*****************/
    /* Audio Methods */
    /*****************/
    public void PlayMoveLeftAudio()
    {
        audioSource.PlayOneShot(moveLeftSound);
    }
    public void PlayMoveRightAudio()
    {
        audioSource.PlayOneShot(moveRightSound);
    }
    public void PlayRotateClkAudio()
    {
        audioSource.PlayOneShot(rotateClockwiseSound);
    }
    public void PlayRotateAntiClkAudio()
    {
        audioSource.PlayOneShot(rotateAntiClockwiseSound);
    }
    public void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }
}

