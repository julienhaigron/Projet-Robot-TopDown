using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private Queue<AIAction> _AIActions = new Queue<AIAction>();
    private PlayerController _currentSelectedPlayer;
    public PlayerController CurrentSelectedPlayer { get => _currentSelectedPlayer; set => _currentSelectedPlayer = value; }

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
        }
    }

}
