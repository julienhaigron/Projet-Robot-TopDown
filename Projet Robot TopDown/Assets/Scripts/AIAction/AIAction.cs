using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : MonoBehaviour
{
    public abstract void Perform();

    public void EndPerform()
    {
        AIActionQueueManager.Instance.PerformNextAIAction();
    }

}
