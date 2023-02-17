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

    //weapons
    public GameObject _weaponVisionConePrefab;
    public List<GameObject> _weaponsVisionCone;
    public GameObject _weaponVisionConeGreyPrefab;
    public GameObject _weaponVisionConeGrey;
    public List<Tile> _weaponsTarget;

    public GameObject _ui;
    public TextMeshProUGUI _actionPointText;

    private void OnMouseUp()
    {
        //pop frontier
        switch (_currentSelectionState)
        {
            case PlayerController.RobotSelectionState.Unselected:
                //Debug.Log("activate movment sprite");
                GameManager.Instance.TurnManager.CurrentSelectedPlayer = _connectedPlayer;
                GameManager.Instance.GridManager.ActivateMovementCell(_currentTile, _connectedPlayer.RemainingActionPoints);
                _currentSelectionState = PlayerController.RobotSelectionState.Selected;
                break;
            case PlayerController.RobotSelectionState.Selected:
                //Debug.Log("deactivate movment sprite");
                GameManager.Instance.GridManager.DeactivateMovementCellSprite();
                _currentSelectionState = PlayerController.RobotSelectionState.Unselected;
                break;
        }
    }

    public void InitWeapons()
    {
        _weaponsVisionCone = new List<GameObject>();
        _weaponsTarget = new List<Tile>();

        int weaponId = 0;
        foreach (WeaponStats weaponStat in _connectedPlayer._robotStats._weapons)
        {
            GameObject weapon = Instantiate(_weaponVisionConePrefab, transform.position, Quaternion.identity, transform);
            weapon.transform.localScale = new Vector3((float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f, (float)(weaponStat._range + 0.5f) / 1.5f);
            weapon.transform.rotation = _connectedPlayer.Weapons[weaponId].transform.rotation;
            _weaponsVisionCone.Add(weapon);
            _weaponsTarget.Add(GameManager.Instance.GridManager.GetTile(_currentTile._location.x + 1, _currentTile._location.y));
            weaponId++;
        }
    }

    public void InitUnchaingedAngleWeapon()
    {
        int weaponId = _connectedPlayer.CurrentWeaponSelected;
        _weaponVisionConeGrey = Instantiate(_weaponVisionConeGreyPrefab, transform.position, Quaternion.identity, transform);
        _weaponVisionConeGrey.transform.localScale = new Vector3((float)(_connectedPlayer._robotStats._weapons[weaponId]._range + 0.5f) / 1.5f,
            (float)(_connectedPlayer._robotStats._weapons[weaponId]._range + 0.5f) / 1.5f, (float)(_connectedPlayer._robotStats._weapons[weaponId]._range + 0.5f) / 1.5f);

        Vector3 oldRotation = transform.rotation.eulerAngles;

        Vector2Int currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, _weaponsTarget[weaponId]._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        _weaponVisionConeGrey.transform.rotation = newRotationQUAT;
    }

    public void UpdateWeaponTarget(Tile target)
    {
        if (_weaponsVisionCone[_connectedPlayer.CurrentWeaponSelected] == null)
            return;

        Vector3 oldRotation = transform.rotation.eulerAngles;

        Vector2Int currentLocation = GameManager.Instance.TurnManager.CurrentGhost.CurrentTile._location;

        Vector3 newRotationV3 = new Vector3(oldRotation.x, GameManager.Instance.GridManager.GetTileAngle(currentLocation, target._location), oldRotation.z);
        Quaternion newRotationQUAT = new Quaternion();
        newRotationQUAT.eulerAngles = newRotationV3;
        _weaponsVisionCone[_connectedPlayer.CurrentWeaponSelected].transform.rotation = newRotationQUAT;

        GameManager.Instance.GridManager.DeactivateMovementCellSprite();
        GameManager.Instance.GridManager.DeactivateAttackCellSprite();
        GameManager.Instance.GridManager.ActivateWeaponConeTiles(_currentTile, target);
    }

    public void DepopGhost()
    {
        foreach(GameObject cone in _weaponsVisionCone)
        {
            Destroy(cone);
        }
        Destroy(_weaponVisionConeGrey);
        _weaponsVisionCone = null;
        Destroy(gameObject);
        GameManager.Instance.TurnManager.CurrentGhost = null;
    }

}
