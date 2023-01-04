using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject _ghostPrefab;
    private GhostController _currentGhost;
    public GhostController CurrentGhost { get => _currentGhost; set => _currentGhost = value; }
    private Queue<AIAction> _AIActions = new Queue<AIAction>();
    private PlayerController _currentSelectedPlayer;
    public PlayerController CurrentSelectedPlayer { get => _currentSelectedPlayer; set => _currentSelectedPlayer = value; }

    private List<Tile> _currentPath;
    public List<Tile> CurrentPath { get => _currentPath; set => _currentPath = value; }

    private TurnState _currentTurnState;
    public TurnState CurrentTurnState { get => _currentTurnState; set => _currentTurnState = value; }
    public enum TurnState
    {
        RecordingPlayerActions,
        PerformingPlayerActions,
        PerformingEnemyActions
    }

    private void Start()
    {
        _currentTurnState = TurnState.RecordingPlayerActions;
    }

    public void AddAIActionToQueue(AIAction action)
    {
        _AIActions.Enqueue(action);

        //pay action cost
        Debug.Log("movment cost : " + action._cost);
        CurrentSelectedPlayer.CurrentActionPoints -= action._cost;
        CurrentSelectedPlayer._actionPointText.SetText(CurrentSelectedPlayer.CurrentActionPoints.ToString());

        //start perform player turn if all action used
        if (CurrentSelectedPlayer.CurrentActionPoints <= 0)
            StartPerformAIActions();
    }

    public void StartPerformAIActions()
    {
        if (_currentTurnState != TurnState.PerformingPlayerActions)
        {
            _currentSelectedPlayer.CurrentSelectionState = PlayerController.RobotSelectionState.Unselected;
            GameManager.Instance.GridManager.DeactivateMovementCellSprite();

            _currentTurnState = TurnState.PerformingPlayerActions;
            AIAction firstAction = _AIActions.Dequeue();

            firstAction.Perform();
        }
        else
        {
            Debug.LogError("Error : AIActionQueueManager is already active");
        }
    }

    public void PerformNextAIAction()
    {
        if(_AIActions.Count > 0)
        {
            AIAction action = _AIActions.Dequeue();
            action.Perform();
        }
        else
        {
            //no more actions
            _currentTurnState = TurnState.RecordingPlayerActions;
            Debug.Log("player turn fully performed");
            GameManager.Instance.GridManager.DeactivateMovementCellSprite();
            GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentSelectionState = PlayerController.RobotSelectionState.Unselected;
        }
    }

    #region Actions

    public void AddMovementAction(Tile destination)
    {
        GameManager.Instance.GridManager.DeactivateMovementCellSprite();

        //add action to queue
        MoveAction moveAction = new MoveAction(GameManager.Instance.TurnManager._currentPath, CurrentSelectedPlayer);
        AddAIActionToQueue(moveAction);

        PopGhost(destination);
    }

    public void PopGhost(Tile ghostTile)
    {
        _currentSelectedPlayer.CurrentSelectionState = PlayerController.RobotSelectionState.GhostActivated;

        Vector3 position = ghostTile.transform.position + new Vector3(0, 1 / 2f, 0);
        GameObject currentGhostGO = Instantiate(_ghostPrefab, position, Quaternion.identity);

        GhostController ghostController = currentGhostGO.GetComponent<GhostController>();
        ghostController._connectedPlayer = CurrentSelectedPlayer;
        ghostController.CurrentTile = ghostTile;
        _currentGhost = ghostController;
    }

    #endregion

}
