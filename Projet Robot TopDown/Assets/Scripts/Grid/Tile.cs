using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int _autoCreationId;
    public Vector2Int _location;

    private TileVisibilityState _currentVisibilityState = TileVisibilityState.Undiscovered;
    public TileVisibilityState CurrentVisibilityState { get => _currentVisibilityState; set => _currentVisibilityState = value; }
    public enum TileVisibilityState
    {
        Undiscovered,
        NotVisible,
        Visible
    }

    #region Pathfinding
    public bool _isWalkable = true;
    public float _g = 1;
    public float _h = 0;
    public float _f { get { return _g + _h; } }
    public NodeState _state { get; set; }
    public Tile _parentNode { get; set; }
    public enum NodeState { Untested, Open, Closed }

    public int CompareTo(Tile tile)
    {
        if (_h < tile._h)
        {
            return -1;
        }
        if (_h > tile._h)
        {
            return 1;
        }
        return 0;
    }

    public void MarkAsObstacle()
    {
        _isWalkable = false;
    }

    #endregion

    public GameObject _groundSR;
    public GameObject _movementSprite;
    public GameObject _pathSprite;
    public GameObject _attackSprite;
    public GameObject _deadAttackSprite;

    public void SetTileVisibility(TileVisibilityState tileVisibilityState)
    {
        CurrentVisibilityState = tileVisibilityState;

        switch (CurrentVisibilityState)
        {
            case TileVisibilityState.Undiscovered:
                _groundSR.SetActive(false);
                break;
            case TileVisibilityState.NotVisible:
                _groundSR.SetActive(true);
                //TODO : hide enemy on this tile (if exist)
                break;
            case TileVisibilityState.Visible:
                _groundSR.SetActive(true);
                //TODO : show enemy on this tile (if exist)
                break;
        }
    }

    #region Controller

    private void OnMouseUp()
    {
        if (GameManager.Instance.TurnManager.CurrentSelectedPlayer != null && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            PlayerController robot = GameManager.Instance.TurnManager.CurrentSelectedPlayer;

            switch (GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction)
            {
                case PlayerController.RobotActions.Move:
                    GameManager.Instance.TurnManager.AddMovementAction(this);
                    break;
                case PlayerController.RobotActions.TurnWeapon:
                    GameManager.Instance.TurnManager.AddRotateWeaponAction(this, robot, robot.CurrentWeaponSelected);
                    break;
                case PlayerController.RobotActions.ShootIfPossible:
                    GameManager.Instance.TurnManager.AddAttackIfPossibleAction(robot, robot.CurrentWeaponSelected);
                    break;
            }
        }
    }

    private void OnMouseEnter()
    {
        
        if (GameManager.Instance.TurnManager.CurrentSelectedPlayer != null && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            switch (GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentRobotAction)
            {
                case PlayerController.RobotActions.Move:
                    if (_movementSprite.activeSelf)
                    {
                        List<Tile> pathToThisTile = new List<Tile>();

                        if (GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentSelectionState == PlayerController.RobotSelectionState.GhostActivated)
                            pathToThisTile = GameManager.Instance.Pathfinding.FindPath(GameManager.Instance.TurnManager.CurrentGhost.CurrentTile, this);
                        else
                            pathToThisTile = GameManager.Instance.Pathfinding.FindPath(GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentTile, this);

                        if (pathToThisTile == null || pathToThisTile.Count == 0)
                            return;

                        foreach (Tile tile in pathToThisTile)
                        {
                            //Debug.Log(tile._location);
                            tile._pathSprite.SetActive(true);
                        }

                        GameManager.Instance.TurnManager.CurrentPath = pathToThisTile;
                    }
                    break;

                case PlayerController.RobotActions.TurnWeapon:

                    if (GameManager.Instance.TurnManager.CurrentGhost != null)
                        GameManager.Instance.TurnManager.CurrentGhost.UpdateWeaponTarget(this);
                    else
                        GameManager.Instance.TurnManager.CurrentSelectedPlayer.UpdateWeaponTarget(this);
                    break;

                case PlayerController.RobotActions.ShootIfPossible:
                    //do nothing
                    break;
            }
        }


    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.TurnManager.CurrentSelectedPlayer != null && _movementSprite.activeSelf && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            GameManager.Instance.GridManager.DeactivatePathCellSprite();
        }
    }

    #endregion

}
