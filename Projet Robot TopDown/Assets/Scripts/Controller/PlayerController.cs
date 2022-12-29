using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //movement
    public int _gridMovementSpeed = 3;
    public Tile _currTile;

    //ref
    public GridManager _gridManager;

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
        _gridManager.LoadGridInScene();
        _currTile = _gridManager.GetTile(8, 8);
    }

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case RobotSelectionState.Unselected:
                Debug.Log("activate movment sprite");
                _gridManager.ActivateMovementCell(_currTile._location, _gridMovementSpeed);
                _currentSelectionState = RobotSelectionState.Selected;
                break;
            case RobotSelectionState.Selected:
                Debug.Log("deactivate movment sprite");
                _gridManager.DeactivateMovemtnCell();
                _currentSelectionState = RobotSelectionState.Unselected;
                break;
        }
    }
}
