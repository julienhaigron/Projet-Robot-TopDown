using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public RobotStats _robotStats;
    //movement
    public int _currentActionPoints;
    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; }
    private List<Tile> _currentPath;
    public int _posInPath;
    public Vector3 _destination;
    public bool _isMoving;

    //ref
    public GameManager _gameManager;
    public Rigidbody _rb;
    public BoxCollider _bc;

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

        NewTurn();
        _isMoving = false;
        _posInPath = 0;

        //debug
        _gameManager.GridManager.LoadGridInScene();
        _currentTile = _gameManager.GridManager.GetTile(8, 8);
    }

    private void Update()
    {
        if (_isMoving && _destination != null)
        {
            MoveToDestination(_destination);
        }
    }

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case RobotSelectionState.Unselected:
                Debug.Log("activate movment sprite");
                _gameManager.GridManager.ActivateMovementCell(_currentTile._location, _robotStats._actionPointsPerTurn);
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

    public void NewTurn()
    {
        _currentActionPoints = _robotStats._actionPointsPerTurn;
    }

    public void SetPath(List<Tile> path)
    {
        Debug.Log("set path");
        _currentPath = path;
        _posInPath = 0;
        SetDestination(path[_posInPath++].transform.position + new Vector3(0, _bc.size.y/2f, 0));
    }

    public void SetDestination(Vector3 destination)
    {
        _isMoving = true;
        _destination = destination;
        MoveToDestination(destination);
    }

    public void MoveToDestination(Vector3 destination)
    {
        float distance = Vector3.Distance(transform.localPosition, _destination);

        //3) apply velocity
        //_rb.velocity = _cardsMovementSpeed * difference;

        var step = distance * 10 * Time.deltaTime; // calculate distance to move
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, step);

        //4) check if reached destination
        if (distance <= 0.01f)
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        _isMoving = false;
        if (_posInPath == _currentPath.Count)
        {
            //reached the end
            GameManager.Instance.TurnManager.PerformNextAIAction();
            
            transform.localPosition = _destination;

            //reset physics
            _rb.velocity = new Vector3(0, 0, 0);
            _rb.angularVelocity = new Vector3(0, 0, 0);
        }
        else
        {
            //go to next tile
            SetDestination(_currentPath[_posInPath++].transform.position + new Vector3(0, _bc.size.y / 2f, 0));

            _currentTile = _currentPath[_posInPath];
        }
    }

}
