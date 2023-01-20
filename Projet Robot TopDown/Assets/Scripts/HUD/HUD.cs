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
        GameManager.Instance.TurnManager.CurrentSelectedPlayer.ResetWeaponCones();
        GameManager.Instance.GridManager.DeactivateAttackCellSprite();
        GameManager.Instance.GridManager.DeactivateDeadAttackCellSprite();

        if (GameManager.Instance.TurnManager.CurrentGhost == null)
            GameManager.Instance.GridManager.ActivateMovementCell(GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentTile._location, GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentActionPoints);
        else
            GameManager.Instance.GridManager.ActivateMovementCell(GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location, GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentActionPoints);
    }

    public void TurnWeaponButton()
    {
        if (GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction != PlayerController.RobotActions.TurnWeapon)
        {
            GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction = PlayerController.RobotActions.TurnWeapon;

            //instantiate cone prefab
            if (GameManager.Instance.TurnManager.CurrentGhost != null)
                GameManager.Instance.TurnManager.CurrentGhost.InitUnchaingedAngleWeapon();
            else
                GameManager.Instance.TurnManager.CurrentSelectedPlayer.InitUnchaingedAngleWeapon();
        }
    }

    public void ShootIfPossibleButton()
    {
        GameManager.Instance.TurnManager.AddAttackIfPossibleAction(GameManager.Instance.TurnManager.CurrentSelectedPlayer, GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentWeaponSelected);
        GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction = PlayerController.RobotActions.ShootIfPossible;
    }

}
