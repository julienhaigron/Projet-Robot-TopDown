using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GhostController : MonoBehaviour
{
    public PlayerController _connectedPlayer;

    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; set => _currentTile = value; }

    private PlayerController.RobotSelectionState _currentSelectionState;
    public PlayerController.RobotSelectionState CurrentSelectionState { get => _currentSelectionState; set => _currentSelectionState = value; }

    public GameObject _ui;
    public TextMeshProUGUI _actionPointText;

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case PlayerController.RobotSelectionState.Unselected:
                Debug.Log("activate movment sprite");
                GameManager.Instance.TurnManager.CurrentSelectedPlayer = _connectedPlayer;
                GameManager.Instance.GridManager.ActivateMovementCell(_currentTile._location, _connectedPlayer.CurrentActionPoints);
                _currentSelectionState = PlayerController.RobotSelectionState.Selected;
                break;
            case PlayerController.RobotSelectionState.Selected:
                Debug.Log("deactivate movment sprite");
                GameManager.Instance.GridManager.DeactivateMovementCellSprite();
                _currentSelectionState = PlayerController.RobotSelectionState.Unselected;
                break;
        }
    }
}
