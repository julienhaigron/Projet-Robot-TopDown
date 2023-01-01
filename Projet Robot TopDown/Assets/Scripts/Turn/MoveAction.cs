using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : AIAction
{
    public List<Tile> _movementPath;
    public PlayerController _robot;
    
    public MoveAction(List<Tile> movemntPath, PlayerController robot)
    {
        _movementPath = movemntPath;
        _robot = robot;
    }

    public override void Perform()
    {
        SendPathToRobot();
    }

    public void SendPathToRobot()
    {
        _robot.SetPath(_movementPath);
    }
}
