using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeaponAction : AIAction
{
    public float _from;
    public Tile _aimedTile;
    public int _weaponId;
    public PlayerController _robot;
    
    public RotateWeaponAction(Tile tileAimed, PlayerController robot, int weaponId)
    {
        _aimedTile = tileAimed;
        _robot = robot;
        _weaponId = weaponId;
        _cost = 0;
    }

    public override void Perform()
    {
        SendAimToRobot();
    }

    public void SendAimToRobot()
    {
        _robot.SetWeaponAim(_aimedTile, _weaponId);
    }
}
