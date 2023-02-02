using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public RobotStats _robotStats;

    //actions
    private int _remainingActionPoints;
    public int RemainingActionPoints { get => _remainingActionPoints; set => _remainingActionPoints = value; }
    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; set => _currentTile = value; }

    //movement
    private Tile _currentTileDestination;
    private Vector3 _currentVectorDestination;
    private bool _isMoving;

    //weapons
    public GameObject _weaponVisionConePrefab;
    public List<GameObject> Weapons;
    public GameObject _weaponVisionConeGreyPrefab;
    private GameObject _weaponVisionConeGrey;
    private int _currentWeaponSelected;
    public int CurrentWeaponSelected { get => _currentWeaponSelected; set => _currentWeaponSelected = value; }
    public List<Tile> _weaponsTarget;

    //ref
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

    public void Init(Tile spawn)
    {
        _currentTile = spawn;

        NewTurn(); //reset stats

        GameManager.Instance.GridManager.LoadGridInScene();
        GameManager.Instance.GridManager.UpdateVisibleTiles();

        InitWeapons();
    }

    private void Start()
    {
        _isMoving = false;
        _currentSelectionState = RobotSelectionState.Unselected;
    }

    private void Update()
    {
        if (_isMoving && _currentVectorDestination != null)
        {
            MoveToDestination(_currentVectorDestination);
        }
    }

    public void NewTurn()
    {
        _remainingActionPoints = _robotStats._actionPointsPerTurn;
        _actionPointText.SetText(_remainingActionPoints.ToString());
        _currentRobotAction = RobotActions.Move;
    }

    public void SetDestination(Tile destination)
    {
        _currentTileDestination = destination;
        SetDestination(destination.transform.position + new Vector3(0, _bc.size.y / 2f, 0));
    }

    public void ActivateChost()
    {
        _currentSelectionState = PlayerController.RobotSelectionState.GhostActivated;

        foreach (GameObject cone in Weapons)
        {
            cone.SetActive(false);
        }
    }

    public void DeactivateGhost()
    {
        _currentSelectionState = PlayerController.RobotSelectionState.Unselected;

        foreach (GameObject cone in Weapons)
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
                GameManager.Instance.GridManager.ActivateMovementCell(_currentTile._location, _remainingActionPoints);
                GameManager.Instance.TurnManager.CurrentSelectedPlayer = this;
                _currentSelectionState = RobotSelectionState.Selected;
                GameManager.Instance.HUD.DisplayActionsButtons();
                break;
            case RobotSelectionState.Selected:
                GameManager.Instance.GridManager.DeactivateMovementCellSprite();
                _currentSelectionState = RobotSelectionState.Unselected;
                GameManager.Instance.TurnManager.CurrentSelectedPlayer = null;
                GameManager.Instance.HUD.HideActionsButtons();
                break;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        _isMoving = true;
        _currentVectorDestination = destination;
        MoveToDestination(destination);
    }

    public void MoveToDestination(Vector3 destination)
    {
        float distance = Vector3.Distance(transform.localPosition, _currentVectorDestination);

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

        //reached the end
        transform.localPosition = _currentVectorDestination;
        CurrentTile = _currentTileDestination;

        //reset physics
        _rb.velocity = new Vector3(0, 0, 0);
        _rb.angularVelocity = new Vector3(0, 0, 0);

        GameManager.Instance.GridManager.UpdateVisibleTiles();
        GameManager.Instance.TurnManager.PlayerRobotPerformedActionCallback(this);
    }

    #endregion

    #region Weapon


    public void InitWeapons()
    {
        Weapons = new List<GameObject>();
        _weaponsTarget = new List<Tile>();
        _currentWeaponSelected = 0;

        foreach (WeaponStats weaponStat in _robotStats._weapons)
        {
            GameObject weapon = Instantiate(_weaponVisionConePrefab, transform.position + new Vector3(0, -0.48f, 0), Quaternion.identity, transform);
            weapon.transform.localScale = new Vector3((float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f);
            Weapons.Add(weapon);
            _weaponsTarget.Add(GameManager.Instance.GridManager.GetTile(_currentTile._location.x + 1, _currentTile._location.y));
        }
    }

    public void InitUnchaingedAngleWeapon()
    {
        int weaponId = _currentWeaponSelected;
        if (_weaponVisionConeGrey == null)
        {
            _weaponVisionConeGrey = Instantiate(_weaponVisionConeGreyPrefab, transform.position + new Vector3(0, -0.48f, 0), Quaternion.identity, transform);
            _weaponVisionConeGrey.transform.localScale = new Vector3((float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f, (float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f, (float)(_robotStats._weapons[weaponId]._range + 0.5f) / 1.5f);
        }

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

    public void UpdateWeaponTarget(Tile target, int weaponId)
    {
        if (Weapons[weaponId] == null)
            return;

        Vector3 oldRotation = Weapons[weaponId].transform.rotation.eulerAngles;
        //Debug.Log("angle : " + GameManager.Instance.GridManager.GetTileAngle(_currentTile._location, target._location));

        Vector2Int currentLocation;
        if (GameManager.Instance.TurnManager.CurrentGhost == null)
            currentLocation = _currentTile._location;
        else
            currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, target._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        Weapons[weaponId].transform.rotation = newRotationQUAT;

        GameManager.Instance.GridManager.DeactivateMovementCellSprite();
        GameManager.Instance.GridManager.DeactivateAttackCellSprite();
        GameManager.Instance.GridManager.ActivateWeaponConeTiles(_currentTile, target);
    }

    public void ResetWeaponCones()
    {
        //TODO : save position before destroying and rotate actual to old
        Destroy(_weaponVisionConeGrey);


        Vector2Int currentLocation;
        if (GameManager.Instance.TurnManager.CurrentGhost == null)
            currentLocation = _currentTile._location;
        else
            currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        int counter = 0;
        foreach (GameObject weapon in Weapons)
        {
            Vector3 oldRotation = transform.rotation.eulerAngles;
            Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, _weaponsTarget[counter]._location), oldRotation.z);
            Quaternion newRotationQUAT = new Quaternion();
            newRotationQUAT.eulerAngles = newRotationV3;
            Weapons[_currentWeaponSelected].transform.rotation = newRotationQUAT;
            counter++;
        }
    }

    public void SetWeaponAim(Tile aimedTile, int weaponId)
    {
        Vector3 oldRotation = Weapons[weaponId].transform.rotation.eulerAngles;

        Vector2Int currentLocation = _currentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, aimedTile._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        Weapons[weaponId].transform.rotation = newRotationQUAT;

        GameManager.Instance.TurnManager.PlayerRobotPerformedActionCallback(this);
    }

    public void AttackIfPossible(int weaponId)
    {
        //perform attack

        GameManager.Instance.TurnManager.PlayerRobotPerformedActionCallback(this);
    }

    #endregion

}
