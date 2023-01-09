using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeaponAction : AIAction
{
    public float _from;
    public float _rotation;
    public int _weaponId;
    public PlayerController _robot;
    
    public RotateWeaponAction(float rotation, PlayerController robot, int weaponId)
    {
        _rotation = rotation;
        _robot = robot;
        _weaponId = weaponId;
        _cost = 0;
    }

    public override void Perform()
    {
        SendRotationToRobot();
    }

    public void SendRotationToRobot()
    {
        _robot.SetWeaponRotation(_rotation, _weaponId);
    }
}
