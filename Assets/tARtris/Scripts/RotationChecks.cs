using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Tetromino/Rotation Check", order = 1)]
public class RotationChecks : ScriptableObject
{
    public int[] ZeroClockwise;
    public int[] ZeroAntiClockwise;
    public int[] RightClockwise;
    public int[] RightAntiClockwise;
    public int[] TwoClockwise;
    public int[] TwoAntiClockwise; 
    public int[] LeftClockwise;
    public int[] LeftAntiClockwise;
}
