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
        GameObject[] playersGO = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playersGO)
        {
            TurnManager.Players.Add(player.GetComponent<PlayerController>());
        }

        //select first platyers
        TurnManager.CurrentSelectedPlayer = TurnManager.Players[0];
        TurnManager.CurrentSelectedPlayer.CurrentSelectionState = PlayerController.RobotSelectionState.Selected;

        //instantiate enemy robot


        //setup HUD
        GameManager.Instance.HUD.DisplayActionsButtons();

        //init turn manager
        GameManager.Instance.TurnManager.Init();

        //start record player's robots actions
        TurnManager.CurrentTurnState = TurnManager.TurnState.RecordingPlayerActions;
    }
}
