using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GridManager _gridManager;
    public GridManager GridManager { get => _gridManager; }
    [SerializeField]
    private Pathfinding _pathfinding;
    public Pathfinding Pathfinding { get => _pathfinding; }

    [SerializeField]
    private TurnManager _turnManager;
    public TurnManager TurnManager { get => _turnManager; }

    [SerializeField]
    private HUD _hud;
    public HUD HUD { get => _hud; }

    //spawn
    [Header("Spawn")]
    public GameObject _robotPrefab;
    public List<RobotStats> PlayerRobots;
    public List<Tile> PlayerRobotsSpawn;

    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        //instantiate ally robot
        TurnManager.Players = new List<PlayerController>();

        for (int i = 0; i < PlayerRobots.Count; i++)
        {
            Vector3 playersPos = PlayerRobotsSpawn[i].transform.position + new Vector3(0, 1 / 2f, 0);
            GameObject playerGO = Instantiate(_robotPrefab, playersPos, Quaternion.identity, transform);
            PlayerController controller = playerGO.GetComponent<PlayerController>();
            controller._robotStats = PlayerRobots[i];
            TurnManager.Players.Add(controller);

            controller.Init(PlayerRobotsSpawn[i]);
        }

        /*TurnManager.Players = new List<PlayerController>();
        GameObject[] playersGO = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playersGO)
        {
            TurnManager.Players.Add(player.GetComponent<PlayerController>());
        }*/

        //select first player
        TurnManager.CurrentSelectedPlayer = TurnManager.Players[0];
        TurnManager.CurrentSelectedPlayer.CurrentSelectionState = PlayerController.RobotSelectionState.Selected;
        GridManager.ActivateMovementCell(TurnManager.CurrentSelectedPlayer);

        //instantiate enemy robot


        //setup HUD
        HUD.DisplayActionsButtons();

        //init turn manager
        TurnManager.Init();

        //start record player's robots actions
        TurnManager.CurrentTurnState = TurnManager.TurnState.RecordingPlayerActions;
    }
}
