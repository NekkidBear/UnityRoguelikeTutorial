using UnityEngine;
using System.Collections;
using System.Collections.Generic;        // Allows us to use Lists. 
using UnityEngine.SceneManagement;       // Allows us to use scene Loaded events
using UnityEngine.UIElements;            // Allows us to use UI Toolkit.
using Assets.Scripts;

public class GameManager : MonoBehaviour
{
    // Time to wait before starting level, int seconds.
    public float levelStartDelay = 2f;
    // Delay between each Player turn.
    public float turnDelay = 0.1f;
    // Starting value for Player food points.
    public int playerFoodPoints = 100;
    // Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager instance = null;
    // Boolean to check if it's players turn, hidden in inspector but public.
    [HideInInspector] public bool playersTurn = true;

    // Store a reference to our BoardManager which will set up the level.
    private BoardManager boardScript;
    // Current level number, expressed in game as "Day 1".
    private int level = 1;
    // List of all Enemy units, used to issue them move commands.
    private List<Enemy> enemies;
    // Boolean to check if enemies are moving.
    private bool enemiesMoving;
    // Boolean to check if we're setting up board, prevent Player from moving during setup.
    private bool doingSetup = true;

    public UIManager uiManager; // Reference to the UIManager

    // Awake is always called before any Start functions
    void Awake()
    {
        // Check if instance already exists
        if (instance == null)
            // If not, set instance to this
            instance = this;
        else if (instance != this)
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        // Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        // Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        // Call the InitGame function to initialize the first level 
        InitGame();
    }

    // This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Add one level to our level number
        level++;
        // Call InitGame to initialize our level
        InitGame();
    }

    void OnEnable()
    {
        // Tell our 'OnLevelFinishedLoading' function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        // Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change event as soon as this script is disabled.
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // Initializes the game for each level.
    void InitGame()
    {
        // While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        // Set the title card with the current level
        uiManager.ShowTitleCard("Day " + level);

        // Call HideTitleCard function with a delay in seconds of levelStartDelay.
        Invoke("HideTitleCard", levelStartDelay);

        // Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        // Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);
    }

    // Hides title card used between levels
    void HideTitleCard()
    {
        // Hide the title card
        uiManager.HideTitleCard();

        // Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    // Update is called every frame.
    void Update()
    {
        // Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving || doingSetup)
            // If any of these are true, return and do not start MoveEnemies.
            return;

        // Start moving enemies.
        StartCoroutine(MoveEnemies());
    }

    // Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        // Add Enemy to List enemies.
        enemies.Add(script);
    }

    // GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        // Set the game over message
        uiManager.ShowGameOverMessage("After " + level + " days, you starved.");

        // Disable this GameManager.
        enabled = false;
    }

    // Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        // While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        // Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        // If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            // Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        // Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            // Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            // Wait for Enemy's moveTime before moving next Enemy,
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        // Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        // Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }
}
