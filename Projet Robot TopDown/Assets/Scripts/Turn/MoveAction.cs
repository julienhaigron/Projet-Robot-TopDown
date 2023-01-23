using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : AIAction
{
    public List<Tile> _movementPath;
    public PlayerController _playerRobot;
    public EnemyController _enemyRobot;
    
    public MoveAction(List<Tile> movemntPath, PlayerController robot)
    {
        _movementPath = movemntPath;
        _playerRobot = robot;
        _cost = _movementPath.Count-1;
    }


    public MoveAction(List<Tile> movemntPath, EnemyController robot)
    {
        _movementPath = movemntPath;
        _enemyRobot = robot;
        _cost = _movementPath.Count - 1;
    }

    public override void Perform()
    {
        SendPathToRobot();
    }

    public void SendPathToRobot()
    {
        if (_playerRobot != null)
            _playerRobot.SetPath(_movementPath);
        else
            _enemyRobot.SetPath(_movementPath);
    }
}
