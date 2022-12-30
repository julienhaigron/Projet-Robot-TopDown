using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : AIAction
{
    public List<Tile> _movementPath;
    public MoveAction(List<Tile> movemntPath)
    {
        _movementPath = movemntPath;
    }

    public override void Perform()
    {
        throw new System.NotImplementedException();
    }
}
