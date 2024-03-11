using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGame : MonoBehaviour
{
    public enum GameStates
    {
        Intro,
        StartArea,
        Challenge1,
        Challenge2,
        Challenge3,
        Challenge4,
        Challenge5,
        Challenge6,
        Challenge7,
        Challenge8,
        Ending,
        Credits
    }
    [SerializeField] public List<GameState> gameStates = new List<GameState>();

    [Header("Debug Info")]
    public GameStates currentGameState;
    
    public static ManagerGame instance = null;

    private bool gameOver;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentGameState = GameStates.Intro;
        gameStates[((int)currentGameState)].OnStart();
    }

    private void FixedUpdate()
    {
        if (gameOver)
            return;

        gameStates[((int)currentGameState)].TickGameState();
    }

    public void ResetOnDeath()
    {
       gameStates[((int)currentGameState)].ResetGameState();
    }
    public void AdvanceGameState(GameStates newGameState)
    {
        gameStates[((int)currentGameState)].OnExit();

        if (newGameState == GameStates.Credits)
        {
            gameOver = true;
            return;
        }

        currentGameState = newGameState;
        gameStates[((int)currentGameState)].OnStart();
    }
}

