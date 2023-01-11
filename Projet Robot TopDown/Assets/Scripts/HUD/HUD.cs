using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject _action;

    public void DisplayActionsButtons()
    {
        _action.SetActive(true);
    }

    public void HideActionsButtons()
    {
        _action.SetActive(false);
    }


    //Button Callbacks
    public void MoveButton()
    {
        GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction = PlayerController.RobotActions.Move;
    }

    public void TurnWeaponButton()
    {
        GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction = PlayerController.RobotActions.TurnWeapon;
    }

    public void ShootIfPossibleButton()
    {
        GameManager.Instance.TurnManager.AddAttackIfPossibleAction(GameManager.Instance.TurnManager.CurrentSelectedPlayer, GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentWeaponSelected);
        GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction = PlayerController.RobotActions.ShootIfPossible;
    }

}
