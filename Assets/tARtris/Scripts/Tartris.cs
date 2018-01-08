using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;

using static ScoreType;
using static MoveType;

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
    private ScoreType scoreThisTurn = NoScore;
    private MoveType moveThisTurn = Normal;
    public Text HUDScore;
    public Text HUDLines;
    public Text HUDLevel;

    [Space]
    [Header("Starting Values")]
    private int currentScore = 0;
    private int startingHighScore;
    private int linesCleared = 0;
    public static int startingLevel = 1;
    public float horizontalDelay = 0.3f;

    [Space]
    [Header("In Game Values")]
    [SerializeField]
    private int currentLevel;
    [SerializeField]
    private float DropSpeed;

    private GameObject previewTartriminoGO;
    private GameObject tartriminoGO;
    private GameObject ghostTARtriminoGO;

    private Vector3 startingSpawnPosition = new Vector3(5f, 21f);
    private Vector3 previewTartriminoPosition = new Vector3(14f, 15f);

    private bool gameStarted = false;
    public bool updateGhost = true;
    private bool updateUINeeded = false;

    /// tartrimino variables
    List<int> tartriminoIndices;
    private Queue<int> nextTartriminos = new Queue<int>();
    public GameObject[] TARtriminos;
    public Color[] BlockColors;
    public Material[] materials;
    private MaterialPropertyBlock props;
    private int activeColour;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private float GetFallSpeed()
    {
        return Mathf.Pow((0.8f - (((float)currentLevel - 1f) * 0.007f)), (float)currentLevel - 1f);
    }
    private void Start()
    {
        // Difficulty - level based on selection in menu, and drop speed based on level
        currentLevel = (int)Mathf.Clamp((float)startingLevel, 1f, 15f);
        DropSpeed = GetFallSpeed();

        // Get high score from player prefs
        startingHighScore = PlayerPrefs.GetInt("HighScore");

        // Initialize Tartriminos and populate the queue
        props = new MaterialPropertyBlock();
        tartriminoIndices = new List<int>(Enumerable.Range(0, noTartriminos));
        FillQueue();
        FillQueue();

        // Spawn the first tartrimino
        SpawnNextTARtrimino();
    }
    public void Update()
    {
        UpdateScore();
        UpdateHighScore();
        UpdateLinesCleared();
        UpdateLevel();
        UpdateUI();
        ResetPerTurnVariables();
        if (CrossPlatformInputManager.GetButtonDown("ResetBtn"))
        {
            Reset();
        }
    }
    public void UpdateScore()
    {
        switch (noRowsThisTurn)
        {
            case 0:
                if (moveThisTurn == TSpin)
                {
                    updateUINeeded = true;
                    scoreThisTurn = TSpinNoLineClear;
                }
                else if (moveThisTurn == MiniTSpin)
                {
                    updateUINeeded = true;
                    scoreThisTurn = MiniTSpinNoLineClear;
                }
                else
                {
                    scoreThisTurn = NoScore;
                }
                break;
            case 1:
                updateUINeeded = true;
                if (moveThisTurn == TSpin)
                {
                    scoreThisTurn = TSpinSingle;
                }
                else if (moveThisTurn == MiniTSpin)
                {
                    scoreThisTurn = MiniTSpinSingle;
                }
                else
                {
                    scoreThisTurn = Single;
                }
                break;

            case 2:
                updateUINeeded = true;
                if (moveThisTurn == TSpin)
                {
                    scoreThisTurn = TSpinDouble;
                }
                else
                {
                    scoreThisTurn = Double;
                }
                break;
            case 3:
                updateUINeeded = true;
                if (moveThisTurn == TSpin)
                {
                    scoreThisTurn = TSpinTriple;
                }
                else
                {
                    scoreThisTurn = Triple;
                }
                break;
            case 4:
                {
                    scoreThisTurn = tARtris;
                }
                break;
            default:
                scoreThisTurn = NoScore;
                break;
        }
        currentScore += score[(int)scoreThisTurn];
    }
    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }
    }
    public void UpdateLinesCleared()
    {
        switch (scoreThisTurn)
        {
            case Single:
                linesCleared++;
                break;
            case MiniTSpinSingle:
                linesCleared += 2;
                break;
            case Double:
                linesCleared += 3;
                break;
            case TSpinNoLineClear:
                linesCleared += 4;
                break;
            case Triple:
                linesCleared += 5;
                break;
            case tARtris:
            case TSpinSingle:
                linesCleared += 8;
                break;
            case TSpinDouble:
                linesCleared += 12;
                break;
            case TSpinTriple:
                linesCleared += 16;
                break;
            case MiniTSpinNoLineClear:
            case NoScore:
                break;
        }
    }
    public void UpdateLevel()
    {
        var temp = currentLevel;
        currentLevel = (int)Mathf.Max((float)startingLevel, (float)(linesCleared / 10));
        if (temp != currentLevel)
        {
            UpdateSpeed();
        }
    }
    public void UpdateUI()
    {
        HUDScore.text = currentScore.ToString();
        HUDLevel.text = currentLevel.ToString();
        HUDLines.text = linesCleared.ToString();
    }
    public void UpdateSpeed()
    {
        DropSpeed = GetFallSpeed();
    }
    public void ResetPerTurnVariables()
    {
        scoreThisTurn = NoScore;
        moveThisTurn = Normal;
        noRowsThisTurn = 0;
        updateUINeeded = false;
    }
    public float GetDropSpeed()
    {
        return DropSpeed;
    }
    public bool CheckIsAboveGrid(Tetromino tetro)
    {
        for (int x = 0; x < m_GridWidth; ++x)
        {
            foreach (Transform mino in tetro.transform)
            {
                Vector2 pos = RoundVec2(mino.position);
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
                Vector2 pos = RoundVec2(mino.position);
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

            activeColour = GetRandomTartrimino();
            tartriminoGO = InstantiateTartrimino(activeColour, startingSpawnPosition);
            SpawnGhostTARtrimino(activeColour);

            activeColour = GetRandomTartrimino();

            previewTartriminoGO = InstantiateTartrimino(activeColour, previewTartriminoPosition);
            previewTartriminoGO.GetComponent<Tetromino>().enabled = false;
            tartriminoGO.tag = "currentActiveTARtrimino";

            //SpawnGhostTARtrimino(i);
        }
        else
        {
            tartriminoGO = previewTartriminoGO;
            tartriminoGO.GetComponent<Tetromino>().enabled = true;
            SpawnGhostTARtrimino(activeColour);
            updateGhost = true;

            activeColour = GetRandomTartrimino();
            previewTartriminoGO = InstantiateTartrimino(activeColour, previewTartriminoPosition);
            previewTartriminoGO.GetComponent<Tetromino>().enabled = false;
            tartriminoGO.tag = "currentActiveTARtrimino";
        }
        //updateGhost = true;
    }

    public void SpawnGhostTARtrimino(int i)
    {
        Destroy(GameObject.FindGameObjectWithTag("currentGhostTARtrimino"));

        ghostTARtriminoGO = (GameObject)Instantiate(tartriminoGO, tartriminoGO.transform.position, Quaternion.identity);

        Destroy(ghostTARtriminoGO.GetComponent<Tetromino>());
        ghostTARtriminoGO.AddComponent<GhostTARtrimino>();

        var renderers = ghostTARtriminoGO.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in renderers)
        {
            mr.material = materials[0];
            mr.material.color = BlockColors[i];
        }
    }

    public void DisableCurrentGhost()
    {
        ghostTARtriminoGO.transform.position += new Vector3(500, 500);
        ghostTARtriminoGO.GetComponent<GhostTARtrimino>().enabled = false;
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

    public Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public Text m_MessageText;                // Reference to the overlay Text to display winning text, etc.

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
