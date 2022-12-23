using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionQueueManager : MonoBehaviour
{
    private Queue<AIAction> _AIActions = new Queue<AIAction>();
    private bool _isActive = false;
    public bool IsActive { get => _isActive; }

    #region
    private static AIActionQueueManager instance = null;
    public static AIActionQueueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AIActionQueueManager();
            }
            return instance;
        }
    }
    #endregion Singleton

    public void AddAIActionToQueue(AIAction action)
    {
        _AIActions.Enqueue(action);
    }

    public void StartPerformAIActions()
    {
        if (!_isActive)
        {
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
        AIAction firstAction = _AIActions.Dequeue();

        firstAction.Perform();
    }

}
