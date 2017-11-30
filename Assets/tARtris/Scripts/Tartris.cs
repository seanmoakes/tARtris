using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;

/// <summary>
/// An enum to represent the different score types that can be achieved in a single turn
/// </summary>
enum ScoreType
{
    Single          = 0,
    Double          = 1,
    Triple          = 2,
    tARtris         = 3,
    mTSpin          = 4,
    mTSpinSingle    = 5,
    TSpin           = 6,
    TSpinSingle     = 7,
    TSpinDouble     = 8,
    TSpinTriple     = 9,
    NoScore         = 10
}

/// <summary>
/// The types of moves that are possible
/// </summary>
enum MoveType
{
    Normal          = 0,
    MiniTSpin       = 1,
    TSpin           = 2
}

/// <summary>
/// The Tartris manager class
/// </summary>
public class Tartris : MonoBehaviour
{
    /// Grid Variables
    public static int m_GridWidth = 10;
    public static int m_GridHeight = 20;
    public static Transform[,] grid = new Transform[m_GridWidth, m_GridHeight];

    /// Score Variables
    public List<int> score = new List<int>
    {
        100,    // Single
        300,    // Double
        500,    // Triple
        800,    // tARtris
        100,    // mini T-Spin
        200,    // mini T-Spin Single
        400,    // T-Spin
        800,    // T-Spin Single
        1200,   // T-Spin Double
        1600,   // T-Spin Triple
        0       // No-Score
    };
    private int noRowsThisTurn = 0;
    private int noTartriminos = 7;
    private ScoreType scoreThisTurn = ScoreType.NoScore;
    private MoveType moveThisTurn = MoveType.Normal;
    public Text HUDScore;
    private int currentScore = 0;

    public int startingLevel;
    //public float startingSpeed;
    [SerializeField]
    private float DropSpeed;
    public float horizontalDelay;

    private GameObject previewTartriminoGO;
    private GameObject tartriminoGO;
    private Vector3 startingSpawnPosition = new Vector3(5f, 21f);
    private Vector3 previewTartriminoPosition = new Vector3(14f, 15f);

    private bool gameStarted = false;

    /// tartrimino variables
    List<int> tartriminoIndices;
    private Queue<int> nextTartriminos = new Queue<int>();
    public GameObject[] TARtriminos;
    public Color[] BlockColors;
    private MaterialPropertyBlock props;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private float GetFallSpeed(int level)
    {
        return Mathf.Pow((0.8f - (((float)level - 1f) * 0.007f)), (float)level - 1f);
    }
    private void Start()
    {
        props = new MaterialPropertyBlock();
        startingLevel = (int)Mathf.Clamp((float)startingLevel, 1f, 15f);
        tartriminoIndices = new List<int>(Enumerable.Range(0, noTartriminos));
        FillQueue();
        FillQueue();
        SpawnNextTARtrimino();
        DropSpeed = GetFallSpeed (startingLevel);
    }
    public void Update()
    {
        UpdateScore();
        if (CrossPlatformInputManager.GetButtonDown("ResetBtn"))
        {
            Reset();
        }
    }

    public float GetDropSpeed() { return DropSpeed; }

    public void UpdateScore()
    {
        switch (noRowsThisTurn)
        {
            case 0:
                if (moveThisTurn == MoveType.TSpin)
                {
                    scoreThisTurn = ScoreType.TSpin;
                }
                else if (moveThisTurn == MoveType.MiniTSpin)
                {
                    scoreThisTurn = ScoreType.mTSpin;
                }
                else
                {
                    scoreThisTurn = ScoreType.NoScore;
                }
                break;
            case 1:
                if (moveThisTurn == MoveType.TSpin)
                {
                    scoreThisTurn = ScoreType.TSpinSingle;
                }
                else if (moveThisTurn == MoveType.MiniTSpin)
                {
                    scoreThisTurn = ScoreType.mTSpinSingle;
                }
                else
                {
                    scoreThisTurn = ScoreType.Single;
                }
                break;
            case 2:
                if (moveThisTurn == MoveType.TSpin)
                {
                    scoreThisTurn = ScoreType.TSpinDouble;
                }
                else
                {
                    scoreThisTurn = ScoreType.Double;
                }
                break;
            case 3:
                if (moveThisTurn == MoveType.TSpin)
                {
                    scoreThisTurn = ScoreType.TSpinTriple;
                }
                else
                {
                    scoreThisTurn = ScoreType.Triple;
                }
                break;
            case 4:
                {
                    scoreThisTurn = ScoreType.tARtris;
                }
                break;
            default:
                scoreThisTurn = ScoreType.NoScore;
                break;
        }
        currentScore += score[(int)scoreThisTurn];
        HUDScore.text = currentScore.ToString();
        scoreThisTurn = ScoreType.NoScore;
        moveThisTurn = MoveType.Normal;
        noRowsThisTurn = 0;
    }

    public bool CheckIsAboveGrid(Tetromino tetro)
    {
        for (int x = 0; x < m_GridWidth; ++x)
        {
            foreach (Transform mino in tetro.transform)
            {
                Vector2 pos = roundVec2(mino.position);
                if (pos.y > m_GridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        for (int x = 0; x < m_GridWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        ++noRowsThisTurn;
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < m_GridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < m_GridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < m_GridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < m_GridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tet)
    {
        for (int y = 0; y < m_GridHeight; ++y)
        {
            for (int x = 0; x < m_GridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tet.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tet.transform)
        {
            if (mino != tet.transform.GetChild(4))
            {
                Vector2 pos = roundVec2(mino.position);
                if (pos.y < m_GridHeight)
                {
                    grid[(int)pos.x, (int)pos.y] = mino;
                }
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > m_GridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnNextTARtrimino()
    {
        // Check and fill queue to ensure we have the next load of tartriminos lined up
        if (nextTartriminos.Count <= TARtriminos.Length)
        {
            FillQueue();
        }
        // The first time we need to instatiate both the active tartrimino and the preview one;
        // After that we only need to assign the preview Tartrimino to be active and then replace it
        if (!gameStarted)   
        {
            gameStarted = true;

            int i = GetRandomTartrimino();
            tartriminoGO = InstantiateTartrimino(i, startingSpawnPosition);

            i = GetRandomTartrimino();
            previewTartriminoGO = InstantiateTartrimino(i, previewTartriminoPosition);
            previewTartriminoGO.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            tartriminoGO = previewTartriminoGO;
            tartriminoGO.GetComponent<Tetromino>().enabled = true;

            int i = GetRandomTartrimino();
            previewTartriminoGO = InstantiateTartrimino(i, previewTartriminoPosition);
            previewTartriminoGO.GetComponent<Tetromino>().enabled = false;
        }
    }

    public GameObject InstantiateTartrimino(int tartriminoIndex, Vector3 tartriminoPosition)
    {
        GameObject retTartrimino = Instantiate(TARtriminos[tartriminoIndex], tartriminoPosition, Quaternion.identity) as GameObject;
        props.SetColor("_InstanceColor", BlockColors[tartriminoIndex]);
        var renderers = retTartrimino.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in renderers)
        {
            mr.SetPropertyBlock(props);
        }
        
        return retTartrimino;
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < m_GridWidth && (int)pos.y >= 0;
    }

    public Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public Text m_MessageText;                // Reference to the overlay Text to display winning text, etc.

//     [HideInInspector]
//     public bool m_GameIsOver = false;
// 
//     private int m_Level;                    // Which level the game is currently on.
//  
//     static public void GameOver(bool isOver)
//     {
//        m_GameIsOver = isOver;
//     }
// 
//     static public bool IsGameOver()
//     {
//         return s_Instance.m_GameIsOver;
//     }

    private void CheckQueue()
    {
        if (nextTartriminos.Count <= tartriminoIndices.Count)
            FillQueue();
    }

    private void FillQueue()
    {
        tartriminoIndices.Shuffle();
        foreach (int i in tartriminoIndices)
        {
            nextTartriminos.Enqueue(i);
        }
    }

    public int GetRandomTartrimino()
    {
        return nextTartriminos.Dequeue();
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}

public static class ThreadSafeRandom
{
    [System.ThreadStatic] private static System.Random Local;

    public static System.Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new System.Random(unchecked(System.Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
