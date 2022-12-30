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

    //robot movment
    private List<Tile> _activeMovmentTile;

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

    public Tile GetTile(int x, int y)
    {
        return _grid[x, y];
    }

    public void ActivateMovementCell(Vector2Int source, int speed)
    {
        _activeMovmentTile = new List<Tile>();

        _activeMovmentTile = GameManager.Instance.Pathfinding.Frontier(source, speed);
    }

    public void DeactivateMovemtnCellSprite()
    {
        foreach (Tile activeTile in _activeMovmentTile)
        {
            activeTile._movementCellSR.SetActive(false);
            activeTile._pathCellSR.SetActive(false);
        }
    }
}