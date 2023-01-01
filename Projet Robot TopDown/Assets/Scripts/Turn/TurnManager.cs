using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private Queue<AIAction> _AIActions = new Queue<AIAction>();
    private PlayerController _currentSelectedPlayer;
    public PlayerController CurrentSelectedPlayer { get => _currentSelectedPlayer; set => _currentSelectedPlayer = value; }

    public List<Tile> _currentPath;

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
    }

    public void StartPerformAIActions()
    {
        if (_currentTurnState != TurnState.PerformingPlayerActions)
        {
            GameManager.Instance.GridManager.DeactivateMovemtnCellSprite();

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
        }
    }

    #region Actions

    public void AddMovementAction(Tile destination)
    {
        //add action to queue
        MoveAction moveAction = new MoveAction(GameManager.Instance.TurnManager._currentPath, CurrentSelectedPlayer);
        AddAIActionToQueue(moveAction);

        //pay action cost
        Debug.Log("movment cost : " + destination._f/2);
        CurrentSelectedPlayer._currentActionPoints -= (int)destination._f/2;

        if (CurrentSelectedPlayer._currentActionPoints <= 0)
            StartPerformAIActions();
    }

    #endregion

}
