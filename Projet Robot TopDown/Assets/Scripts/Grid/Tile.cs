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

}
