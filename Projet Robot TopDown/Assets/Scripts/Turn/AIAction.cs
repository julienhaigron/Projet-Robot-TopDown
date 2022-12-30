using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction
{
    public abstract void Perform();

    public void EndPerform()
    {
        GameManager.Instance.TurnManager.PerformNextAIAction();
    }

}
