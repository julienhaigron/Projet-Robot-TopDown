using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int _autoCreationId;
    public Vector2Int _location;
    public bool _isWalkable = true;

    public float _g { get; set; }
    public float _h { get; set; }
    public float _f { get { return _g + _h; } }
    public NodeState _state { get; set; }
    public Tile _parentNode { get; set; }
    public enum NodeState { Untested, Open, Closed }

    public GameObject _groundSR;
    public GameObject _movementCellSR;
    public GameObject _pathCellSR;

    private void OnMouseUp()
    {
        if(_movementCellSR.activeSelf && GameManager.Instance.TurnManager.CurrentTurnState == TurnManager.TurnState.RecordingPlayerActions)
        {
            Debug.Log("record movment action");

            List<Tile> pathToThisTile = GameManager.Instance.Pathfinding.FindPath(GameManager.Instance.TurnManager.CurrentSelectedPlayer.CurrentTile, this);

            foreach (Tile tile in pathToThisTile)
            {
                Debug.Log(tile._location);
                tile._movementCellSR.SetActive(false);
                tile._pathCellSR.SetActive(true);
            }

            MoveAction moveAction = new MoveAction(pathToThisTile);

            GameManager.Instance.TurnManager.AddAIActionToQueue(moveAction);
        }
    }

}
