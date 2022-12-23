using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int _height = 10;
    public int _width = 10;
    public float _gridSpaceSize = 1f;

    public GameObject _gridTilePrefab;
    public GameObject _gridTilePrefabParent;
    public GameObject[,] _grid;

    public void CreateGrid()
    {
        if (_gridTilePrefab == null)
        {
            Debug.LogError("Error : grid tile prefab is null");
            return;
        }

        Vector3 originPos = new Vector3(-_height / 2, 0, -_width/2);

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                //Instantiate(_gridTilePrefab, )
            }
        }

    }
}