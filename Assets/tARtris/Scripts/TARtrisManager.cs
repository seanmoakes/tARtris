using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class TARtrisManager : MonoBehaviour
{
    static public TARtrisManager s_Instance;

    // The difficulty level the player has reached
    
    public float m_StartDelay = 3f;           // The delay between the start of new game setup and playing phases.
    public float m_EndDelay = 3f;             // The delay between the end of playing and game over phases.
    public Text m_MessageText;                // Reference to the overlay Text to display winning text, etc.

    [HideInInspector]
    public bool m_GameIsOver = false;

    //Various UI references to hide the screen between rounds.
    [Space]
    [Header("UI")]
    public CanvasGroup m_FadingScreen;
    public CanvasGroup m_EndRoundScreen;

    private int m_Level;                    // Which level the game is currently on.
    private WaitForSeconds m_StartWait;     // Used to have a delay whilst the game starts.
    private WaitForSeconds m_EndWait;       // Used to have a delay before the game is over.

    void Awake()
    {
        s_Instance = this;
    }

    
    private void Start ()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

    //    StartCoroutine(GameLoop());
	}

    static public void GameOver(bool isOver)
    {
        s_Instance.m_GameIsOver = isOver;
    }

    static public bool IsGameOver()
    {
        return s_Instance.m_GameIsOver;
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("ResetBtn"))
            Reset();
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStarting());
    }

    private IEnumerator GameStarting()
    {

    }

    void NewGame()
    {
        TetrisGrid.grid = new Transform[TetrisGrid.w, TetrisGrid.h];
    }
	// Update is called once per frame
	void Update () {
    */
}
