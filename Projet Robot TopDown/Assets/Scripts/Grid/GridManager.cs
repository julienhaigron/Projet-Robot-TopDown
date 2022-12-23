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
    private GameObject[,] _grid;

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
            foreach(GameObject tile in _grid)
            {
                DestroyImmediate(tile);
            }
        }

        Vector3 originPos = new Vector3(-_height / 2, 0, -_width/2);
        _grid = new GameObject[_height, _width];

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                GameObject tile = Instantiate(_gridTilePrefab, originPos + new Vector3(i * _gridSpaceSize, 0, j * _gridSpaceSize), Quaternion.identity, _gridTilePrefabParent.transform);
                tile.GetComponent<Tile>()._x = i;
                tile.GetComponent<Tile>()._y = j;
                _grid[i, j] = tile;
            }
        }

    }

    public GameObject GetTileGameObject(int x, int y)
    {
        return _grid[x, y];
    }

    public Tile GetTile(int x, int y)
    {
        return _grid[x, y].GetComponent<Tile>();
    }

    public void DisplayMovementCell(int x, int y, int speed)
    {

    }
}