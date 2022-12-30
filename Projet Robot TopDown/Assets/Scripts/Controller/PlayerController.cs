using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //movement
    public int _gridMovementSpeed = 3;
    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; }

    //ref
    public GameManager _gameManager;

    //selection
    public enum RobotSelectionState
    {
        Unselected,
        Selected
    }
    private RobotSelectionState _currentSelectionState;

    void Start()
    {
        //_gridManager = GridManager.Instance;
        _currentSelectionState = RobotSelectionState.Unselected;

        //debug
        _gameManager.GridManager.LoadGridInScene();
        _currentTile = _gameManager.GridManager.GetTile(8, 8);
    }

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case RobotSelectionState.Unselected:
                Debug.Log("activate movment sprite");
                _gameManager.GridManager.ActivateMovementCell(_currentTile._location, _gridMovementSpeed);
                _gameManager.TurnManager.CurrentSelectedPlayer = this;
                _currentSelectionState = RobotSelectionState.Selected;
                break;
            case RobotSelectionState.Selected:
                Debug.Log("deactivate movment sprite");
                _gameManager.GridManager.DeactivateMovemtnCellSprite();
                _currentSelectionState = RobotSelectionState.Unselected;
                break;
        }
    }
}
