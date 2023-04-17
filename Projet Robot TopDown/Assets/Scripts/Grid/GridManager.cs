using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public int _height = 10;
    public int _width = 10;
    public float _gridSpaceSize = 1f;

    public GameObject _gridTilePrefab;
    public GameObject _gridTilePrefabParent;
    private Tile[,] _grid;
    private List<Tile> _obstacleList = new();

    //robot movment
    private List<Tile> _activeMovmentTile = new();

    //robot attack
    public List<Tile> _activeAttackTile = new();
    public List<Tile> _activeDeadAttackTile = new();

    //visible tiles
    private List<Tile> _visibleTiles = new();

    #region Singleton
    private static GridManager instance = null;
    public static GridManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GridManager();
            }
            return instance;
        }
    }
    #endregion

    [ContextMenu("CreateGrid")]
    public void CreateGrid()
    {
        if (_gridTilePrefab == null)
        {
            Debug.LogError("Error : grid tile prefab is null");
            return;
        }

        if (_grid != null)
        {
            foreach (Tile tile in _grid)
            {
                if (tile != null)
                    DestroyImmediate(tile.gameObject);
            }
        }

        int autoCreationId = 0;
        Vector3 originPos = new Vector3(-_height / 2, 0, -_width / 2);
        _grid = new Tile[_height, _width];

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                GameObject tile = Instantiate(_gridTilePrefab, originPos + new Vector3(i * _gridSpaceSize, 0, j * _gridSpaceSize), Quaternion.identity, _gridTilePrefabParent.transform);
                Tile tileScript = tile.GetComponent<Tile>();
                tileScript._location = new Vector2Int(i, j);
                tileScript._autoCreationId = autoCreationId++;
                _grid[i, j] = tileScript;
            }
        }
    }

    public void LoadGridInScene()
    {
        GameObject[] obstaclesGO = GameObject.FindGameObjectsWithTag("Obstacle");
        List<GameObject> newObstaclesListGO = new List<GameObject>(obstaclesGO);
        _obstacleList = new List<Tile>();

        for (int i = 0; i < newObstaclesListGO.Count; i++)
        {
            _obstacleList.Add(newObstaclesListGO[i].GetComponent<Tile>());
        }
        CalculateObstacles();

        GameObject[] tilesGO = GameObject.FindGameObjectsWithTag("Tile");
        List<GameObject> newTileListGO = new List<GameObject>(tilesGO);
        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < newTileListGO.Count; i++)
        {
            tiles.Add(newTileListGO[i].GetComponent<Tile>());
        }

        List<Tile> orderedTile = tiles.OrderBy(tile => tile._autoCreationId).ToList();

        _grid = new Tile[_height, _width];
        int cursor = 0;

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _grid[i, j] = orderedTile[cursor++];
            }
        }

    }

    private void CalculateObstacles()
    {
        if (_obstacleList != null && _obstacleList.Count > 0)
        {
            foreach (Tile obstacle in _obstacleList)
            {

                GetTile(obstacle._location.x, obstacle._location.y).MarkAsObstacle();
            }
        }
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int neighborPosition = tile._location;

        int row = neighborPosition.x;
        int column = neighborPosition.y;

        //Bottom
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        Tile leftNeibhbor = AssignNeighbor(leftNodeRow, leftNodeColumn);
        if (leftNeibhbor != null)
            neighbors.Add(leftNeibhbor);

        //Top
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        leftNeibhbor = AssignNeighbor(leftNodeRow, leftNodeColumn);
        if (leftNeibhbor != null)
            neighbors.Add(leftNeibhbor);

        //Right
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        leftNeibhbor = AssignNeighbor(leftNodeRow, leftNodeColumn);
        if (leftNeibhbor != null)
            neighbors.Add(leftNeibhbor);

        //Left
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        leftNeibhbor = AssignNeighbor(leftNodeRow, leftNodeColumn);
        if (leftNeibhbor != null)
            neighbors.Add(leftNeibhbor);

        return neighbors;
    }

    // Check the neighbor. If it's not an obstacle, assign the neighbor.
    private Tile AssignNeighbor(int row, int column)
    {
        if (row != -1 && column != -1 && row < _height && column < _width)
        {
            Tile nodeToAdd = _grid[row, column];
            if (nodeToAdd._isWalkable)
            {
                return nodeToAdd;
            }
        }

        return null;
    }

    public Tile GetTile(int x, int y)
    {
        return _grid[x, y];
    }

    public void UpdateVisibleTiles()
    {

        foreach (Tile oldVisibleTile in _visibleTiles)
        {
            oldVisibleTile.SetTileVisibility(Tile.TileVisibilityState.NotVisible);
        }

        //make visible new tiles
        List<Tile> newVisibleTiles = new List<Tile>();
        foreach (PlayerController player in GameManager.Instance.TurnManager.Players)
        {
            List<Tile> thisPlayerVisibleTiles = GameManager.Instance.Pathfinding.VisibleTiles(player.CurrentTile, player._robotStats._viewDistance);
            newVisibleTiles.AddRange(thisPlayerVisibleTiles);
        }

        foreach (Tile tile in newVisibleTiles)
        {
            tile.SetTileVisibility(Tile.TileVisibilityState.Visible);
        }

        _visibleTiles = newVisibleTiles;
    }

    public void ActivateMovementCell(PlayerController robot)
    {
        ActivateMovementCell(robot.CurrentTile, robot.RemainingActionPoints);
    }

    public void ActivateMovementCell(Tile source, int speed)
    {
        _activeMovmentTile = new List<Tile>();

        _activeMovmentTile = GameManager.Instance.Pathfinding.TilesInRange(source, speed);

        foreach (Tile tile in _activeMovmentTile)
        {
            tile._movementSprite.SetActive(true);
        }
    }

    public void DeactivateMovementCellSprite()
    {
        foreach (Tile activeTile in _activeMovmentTile)
        {
            activeTile._movementSprite.SetActive(false);
            activeTile._pathSprite.SetActive(false);
        }
    }
    public void DeactivatePathCellSprite()
    {
        foreach (Tile activeTile in _activeMovmentTile)
        {
            activeTile._pathSprite.SetActive(false);
        }
    }
    public void DeactivateAttackCellSprite()
    {
        foreach (Tile activeTile in _activeAttackTile)
        {
            activeTile._attackSprite.SetActive(false);
        }
    }

    public void DeactivateDeadAttackCellSprite()
    {
        foreach (Tile activeTile in _activeDeadAttackTile)
        {
            activeTile._deadAttackSprite.SetActive(false);
        }
    }

    //weapon
    public void ActivateWeaponConeTiles(Tile origin, Tile target)
    {
        PlayerController robot = GameManager.Instance.TurnManager.CurrentSelectedPlayer;
        WeaponStats weapon = robot._robotStats._weapons[robot.CurrentWeaponSelected];

        //get tiles in cirle around weapon._range (frontier)
        List<Tile> frontier = GameManager.Instance.Pathfinding.VisibleTiles(origin, weapon._range);

        //check if tiles angle is in weaponConeAngleRange
        //if true then turn on tile's "attack" sprite in cone
        float targetAngle = GetTileAngle(origin._location, target._location);
        float minAngle = targetAngle - weapon._coneAngle / 2;
        float maxAngle = targetAngle + weapon._coneAngle / 2;

        foreach (Tile tile in frontier)
        {
            float thisTileAngle = GetTileAngle(origin._location, tile._location);
            if (thisTileAngle >= minAngle && thisTileAngle <= maxAngle)
            {
                tile._attackSprite.SetActive(true);
                tile._deadAttackSprite.SetActive(false);
                _activeAttackTile.Add(tile);
            }
            else
            {
                tile._deadAttackSprite.SetActive(true);
                _activeDeadAttackTile.Add(tile);
            }
        }
    }

    /// <summary>
    /// Gives tile rotation on grid towards player
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public float GetTileAngle(Vector2Int origin, Vector2Int destination)
    {
        float x1 = origin.x + 1 - origin.x; //Vector 1 - x
        float y1 = origin.y - origin.y; //Vector 1 - y

        float x2 = destination.x - origin.x; //Vector 2 - x
        float y2 = destination.y - origin.y; //Vector 2 - y

        float angle = Mathf.Atan2(y1, x1) - Mathf.Atan2(y2, x2);
        angle = angle * 360 / (2 * Mathf.PI);

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }
}