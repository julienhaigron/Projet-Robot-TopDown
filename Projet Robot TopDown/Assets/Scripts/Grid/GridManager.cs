using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    public int _height = 10;
    public int _width = 10;
    public float _gridSpaceSize = 1f;

    public GameObject _gridTilePrefab;
    public GameObject _gridTilePrefabParent;
    private Tile[,] _grid;

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

        if(_grid != null)
        {
            foreach(Tile tile in _grid)
            {
                DestroyImmediate(tile.gameObject);
            }
        }

        Vector3 originPos = new Vector3(-_height / 2, 0, -_width/2);
        _grid = new Tile[_height, _width];

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                GameObject tile = Instantiate(_gridTilePrefab, originPos + new Vector3(i * _gridSpaceSize, 0, j * _gridSpaceSize), Quaternion.identity, _gridTilePrefabParent.transform);
                Tile tileScript = tile.GetComponent<Tile>();
                tileScript._location = new Vector2Int(i, j);
                _grid[i, j] = tileScript;
            }
        }

    }

    public Tile GetTile(int x, int y)
    {
        return _grid[x, y].GetComponent<Tile>();
    }

    public void DisplayMovementCell(int x, int y, int speed)
    {

    }
}