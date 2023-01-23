using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public RobotStats _robotStats;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private BoxCollider _bc;


    //actions
    private int _currentActionPoints;
    public int CurrentActionPoints { get => _currentActionPoints; set => _currentActionPoints = value; }
    private Tile _currentTile;
    public Tile CurrentTile { get => _currentTile; }

    public List<Tile> _weaponsTarget;

    //movement
    private List<Tile> _currentPath;
    private int _posInPath;
    private Vector3 _destination;
    private bool _isMoving;

    private enum AIState
    {
        Patrol,
        Eliminate
    }
    private AIState _currentAIState = AIState.Patrol;

    //Patrol
    private List<Tile> _patrolPoints;
    private int _positionInPatrol;

    //Eliminate
    private PlayerController _aiTarget;


    public void InitRobot(List<Tile> patrolPoints)
    {
        //patrol
        _positionInPatrol = 0;
        _patrolPoints = patrolPoints;
    }


    #region IA

    public void DetermineTurnsAction()
    {
        switch (_currentAIState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Eliminate:
                Eliminate();
                break;
        }
    }


    public void Patrol()
    {
        //evaluate if need to change AI behavior
        //search players around robot
        List<Tile> visibleTiles = GameManager.Instance.Pathfinding.Frontier(_currentTile._location, _robotStats._viewDistance);
        List<PlayerController> players = GameManager.Instance.TurnManager.Players;

        List<PlayerController> playersInRange = GameManager.Instance.TurnManager.Players;
        for (int i = 0; i < visibleTiles.Count; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {
                if(visibleTiles[j]._location == players[j].CurrentTile._location)
                {
                    //found player in view range
                    playersInRange.Add(players[j]);
                }
            }
        }

        if(playersInRange.Count > 0)
        {
            _currentAIState = AIState.Eliminate;
            _aiTarget = playersInRange[playersInRange.Count - 1];
            DetermineTurnsAction();
            return;
        }

        //no players found so move to next patrol position
        Tile nextPatrolPosition = _patrolPoints[(_positionInPatrol + 1) % _patrolPoints.Count];

        //TODO : Rotate weapon toward next point
        RotateWeaponAction rotateAction = new RotateWeaponAction(nextPatrolPosition, this, 0);
        GameManager.Instance.TurnManager.AddAIActionToQueue(rotateAction);

        List<Tile> pathToNextPosition = GameManager.Instance.Pathfinding.FindPath(CurrentTile, nextPatrolPosition);
        int nbToDelete = pathToNextPosition.Count - _robotStats._actionPointsPerTurn;
        if (nbToDelete > 0)
        {
            for(int i = 0; i< nbToDelete; i++)
            {
                pathToNextPosition.RemoveAt(pathToNextPosition.Count - 1);
            }
        }

        MoveAction moveAction = new MoveAction(pathToNextPosition, this);
        GameManager.Instance.TurnManager.AddAIActionToQueue(moveAction);
    }

    public void Eliminate()
    {
        //evaluate if need to change AI behavior
        //search players around robot
        List<Tile> visibleTiles = GameManager.Instance.Pathfinding.Frontier(_currentTile._location, _robotStats._viewDistance);
        List<PlayerController> players = GameManager.Instance.TurnManager.Players;

        List<PlayerController> playersInRange = GameManager.Instance.TurnManager.Players;
        for (int i = 0; i < visibleTiles.Count; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {
                if (visibleTiles[j]._location == players[j].CurrentTile._location)
                {
                    //found player in view range
                    playersInRange.Add(players[j]);
                }
            }
        }

        if (playersInRange.Count == 0)
        {
            _currentAIState = AIState.Patrol;
            _aiTarget = null;
            DetermineTurnsAction();
            return;
        }
        else
        {
            _aiTarget = playersInRange[playersInRange.Count - 1];
        }

        //TODO : design rest of behavior and implement it

    }

    #endregion

    #region Movement

    public void SetPath(List<Tile> path)
    {
        _currentPath = path;
        _posInPath = 0;
        SetDestination(path[_posInPath++].transform.position + new Vector3(0, _bc.size.y / 2f, 0));
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

    public void SetWeaponAim(Tile aimedTile, int weaponId)
    {
        float angle = GameManager.Instance.GridManager.GetTileAngle(_currentTile._location, aimedTile._location);
        //_weaponsVisionCone[weaponId].transform.Rotate(Vector3.up, angle);
        //TODO : add enemy weapon range visuals

        GameManager.Instance.TurnManager.PerformNextAIAction();
    }

    #endregion
}
