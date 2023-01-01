using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int _autoCreationId;
    public Vector2Int _location;
    public bool _isWalkable = true;

    public float _g = 1;
    public float _h = 0;
    public float _f { get { return _g + _h; } }
    public NodeState _state { get; set; }
    public Tile _parentNode { get; set; }
    public enum NodeState { Untested, Open, Closed }

    public GameObject _groundSR;
    public GameObject _movementCellSR;
    public GameObject _pathCellSR;

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

    private void OnMouseUp()
    {
        if (_movementCellSR.activeSelf && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            GameManager.Instance.TurnManager.AddMovementAction(this);
        }
    }

    private void OnMouseEnter()
    {
        if (_movementCellSR.activeSelf && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            List<Tile> pathToThisTile = new List<Tile>();

            pathToThisTile = GameManager.Instance.Pathfinding.FindPath(GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentTile, this);

            if (pathToThisTile == null || pathToThisTile.Count == 0)
                return;

            foreach (Tile tile in pathToThisTile)
            {
                //Debug.Log(tile._location);
                tile._pathCellSR.SetActive(true);
            }

            GameManager.Instance.TurnManager._currentPath = pathToThisTile;
        }
    }

    private void OnMouseExit()
    {
        if (_movementCellSR.activeSelf && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            GameManager.Instance.GridManager.DeactivatePathCellSprite();
        }
    }

}
