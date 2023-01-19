using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public RobotStats _robotStats;

    //actions
    private int _currentActionPoints;
    public int CurrentActionPoints { get => _currentActionPoints; set => _currentActionPoints = value; }
    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; }

    //movement
    private List<Tile> _currentPath;
    private int _posInPath;
    private Vector3 _destination;
    private bool _isMoving;

    //weapons
    public GameObject _weaponVisionConePrefab;
    private List<GameObject> _weaponsVisionCone;
    public GameObject _weaponVisionConeGreyPrefab;
    private GameObject _weaponVisionConeGrey;
    private int _currentWeaponSelected;
    public int CurrentWeaponSelected { get => _currentWeaponSelected; set => _currentWeaponSelected = value; }
    public List<Tile> _weaponsTarget;

    //ref
    public GameManager _gameManager;
    public Rigidbody _rb;
    public BoxCollider _bc;
    public GameObject _ui;
    public TextMeshProUGUI _actionPointText;

    //selection
    public enum RobotSelectionState
    {
        Unselected,
        Selected,
        GhostActivated
    }
    private RobotSelectionState _currentSelectionState;
    public RobotSelectionState CurrentSelectionState { get => _currentSelectionState; set => _currentSelectionState = value; }

    //actions
    public enum RobotActions
    {
        Move,
        TurnWeapon,
        ShootIfPossible
    }
    private RobotActions _currentRobotAction;
    public RobotActions CurrentRobotAction { get => _currentRobotAction; set => _currentRobotAction = value; }

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
        GameManager.Instance.GridManager.UpdateVisibleTiles();

        InitWeapons();
    }

    private void Update()
    {
        if (_isMoving && _destination != null)
        {
            MoveToDestination(_destination);
        }
    }

    public void NewTurn()
    {
        _currentActionPoints = _robotStats._actionPointsPerTurn;
        _actionPointText.SetText(_currentActionPoints.ToString());
        _currentRobotAction = RobotActions.Move;
    }

    public void SetPath(List<Tile> path)
    {
        _currentPath = path;
        _posInPath = 0;
        SetDestination(path[_posInPath++].transform.position + new Vector3(0, _bc.size.y / 2f, 0));
    }

    public void ActivateChost()
    {
        _currentSelectionState = PlayerController.RobotSelectionState.GhostActivated;

        foreach(GameObject cone in _weaponsVisionCone)
        {
            cone.SetActive(false);
        }
    }

    public void DeactivateGhost()
    {
        _currentSelectionState = PlayerController.RobotSelectionState.Unselected;

        foreach (GameObject cone in _weaponsVisionCone)
        {
            cone.SetActive(true);
        }
    }

    #region Movement

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case RobotSelectionState.Unselected:
                _gameManager.GridManager.ActivateMovementCell(_currentTile._location, _currentActionPoints);
                _gameManager.TurnManager.CurrentSelectedPlayer = this;
                _currentSelectionState = RobotSelectionState.Selected;
                GameManager.Instance.HUD.DisplayActionsButtons();
                break;
            case RobotSelectionState.Selected:
                _gameManager.GridManager.DeactivateMovementCellSprite();
                _currentSelectionState = RobotSelectionState.Unselected;
                _gameManager.TurnManager.CurrentSelectedPlayer = null;
                GameManager.Instance.HUD.HideActionsButtons();
                break;
        }
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
            _currentTile = _currentPath[_posInPath];
            SetDestination(_currentPath[_posInPath++].transform.position + new Vector3(0, _bc.size.y / 2f, 0));
        }

        GameManager.Instance.GridManager.UpdateVisibleTiles();
    }

    #endregion

    #region Weapon


    public void InitWeapons()
    {
        _weaponsVisionCone = new List<GameObject>();
        _weaponsTarget = new List<Tile>();
        _currentWeaponSelected = 0;

        foreach (WeaponStats weaponStat in _robotStats._weapons)
        {
            GameObject weapon = Instantiate(_weaponVisionConePrefab, transform.position, Quaternion.identity, transform);
            weapon.transform.localScale = new Vector3((float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f);
            _weaponsVisionCone.Add(weapon);
            _weaponsTarget.Add(GameManager.Instance.GridManager.GetTile(_currentTile._location.x + 1, _currentTile._location.y));
        }
    }

    public void InitUnchaingedAngleWeapon()
    {
        int weaponId = _currentWeaponSelected;
        _weaponVisionConeGrey = Instantiate(_weaponVisionConeGreyPrefab, transform.position, Quaternion.identity, transform);
        _weaponVisionConeGrey.transform.localScale = new Vector3((float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f, (float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f, (float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f);
        
        Vector3 oldRotation = transform.rotation.eulerAngles;

        Vector2Int currentLocation;
        if (GameManager.Instance.TurnManager.CurrentGhost == null)
            currentLocation = _currentTile._location;
        else
            currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, _weaponsTarget[weaponId]._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        _weaponVisionConeGrey.transform.rotation = newRotationQUAT;
    }

    public void UpdateWeaponTarget(Tile target)
    {
        if (_weaponsVisionCone[_currentWeaponSelected] == null)
            return;

        Vector3 oldRotation = transform.rotation.eulerAngles;
        //Debug.Log("angle : " + GameManager.Instance.GridManager.GetTileAngle(_currentTile._location, target._location));
        //Debug.Log("angle : " + GameManager.Instance.GridManager.GetTileAngle(_currentTile._location, target._location));

        Vector2Int currentLocation;
        if (GameManager.Instance.TurnManager.CurrentGhost == null)
            currentLocation = _currentTile._location;
        else
            currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, target._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        _weaponsVisionCone[_currentWeaponSelected].transform.rotation = newRotationQUAT;

        GameManager.Instance.GridManager.DeactivateMovementCellSprite();
        GameManager.Instance.GridManager.DeactivateAttackCellSprite();
        GameManager.Instance.GridManager.ActivateWeaponConeTiles(_currentTile, target);
    }

    public void DestroyWeaponCones()
    {
        //TODO : save position before destroying and rotate actual to old
        //Destroy(_weaponVisionConeGrey);
        foreach(GameObject weapon in _weaponsVisionCone)
        {
            Destroy(weapon);
        }
    }

    public void SetWeaponRotation(float angle, int weaponId)
    {
        _weaponsVisionCone[weaponId].transform.Rotate(Vector3.up, angle);

        GameManager.Instance.TurnManager.PerformNextAIAction();
    }

    public void AttackIfPossible(int weaponId)
    {
        //perform attack

        GameManager.Instance.TurnManager.PerformNextAIAction();
    }

    #endregion

}
