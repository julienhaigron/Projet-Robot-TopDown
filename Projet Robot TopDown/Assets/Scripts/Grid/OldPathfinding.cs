using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPathfinding : MonoBehaviour
{
    public Tile _startLocation { get; set; }
    public Tile _endLocation { get; set; }

    public List<Tile> FindPath(Tile startTile, Tile endTile)
    {
        _startLocation = startTile;
        _endLocation = endTile;

        List<Tile> path = new List<Tile>();
        bool success = Search(startTile);
        if (success)
        {
            Tile node = endTile;
            while (node._parentNode != null)
            {
                path.Add(node);
                node = node._parentNode;
            }
            path.Reverse();
        }
        else
        {
            Debug.Log("Error : Cant find a path to destination");
        }
        return path;
    }

    public List<Tile> Frontier(Vector2Int source, int limit)
    {
        List<Tile> frontierTile = new List<Tile>();

        for (int i = 0; i < GameManager.Instance.GridManager._width; i++)
        {
            for (int j = 0; j < GameManager.Instance.GridManager._height; j++)
            {
                Tile tile = GameManager.Instance.GridManager.GetTile(i, j);
                tile._g = Vector2Int.Distance(source, tile._location);

                if (tile._g <= limit)
                {
                    //activate movement cell
                    tile._movementCellSR.SetActive(true);
                    frontierTile.Add(tile);
                }
            }
        }

        return frontierTile;
    }

    private bool Search(Tile currentNode)
    {
        currentNode._state = Tile.NodeState.Closed;
        List<Tile> nextNodes = GetAdjacentWalkableNodes(currentNode);
        nextNodes.Sort((node1, node2) => node1._f.CompareTo(node2._f));
        foreach (var nextNode in nextNodes)
        {
            if (nextNode._location == _endLocation._location)
            {
                return true;
            }
            else
            {
                if (Search(nextNode)) // Note: Recurses back into Search(Node)
                    return true;
            }
        }
        return false;
    }

    private List<Tile> GetAdjacentWalkableNodes(Tile fromNode)
    {
        List<Tile> walkableNodes = new List<Tile>();
        List<Vector2Int> nextLocations = GetAdjacentLocations(fromNode._location);

        foreach (var location in nextLocations)
        {
            int x = location.x;
            int y = location.y;

            // Stay within the grid's boundaries
            if (x < 0 || x >= GameManager.Instance.GridManager._width || y < 0 || y >= GameManager.Instance.GridManager._height)
                continue;

            Tile node = GameManager.Instance.GridManager.GetTile(x, y);
            // Ignore non-walkable nodes
            if (!node._isWalkable)
                continue;

            // Ignore already-closed nodes
            if (node._state == Tile.NodeState.Closed)
                continue;

            // Already-open nodes are only added to the list if their G-value is lower going via this route.
            if (node._state == Tile.NodeState.Open)
            {
                float traversalCost = GetTraversalCost(node._location, node._parentNode._location);
                float gTemp = fromNode._g + traversalCost;
                if (gTemp < node._g)
                {
                    node._parentNode = fromNode;
                    walkableNodes.Add(node);
                }
            }
            else
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                node._parentNode = fromNode;
                node._state = Tile.NodeState.Open;
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    public float GetTraversalCost(Vector2Int from, Vector2Int to)
    {
        return Vector2Int.Distance(from, to);
    }

    public List<Vector2Int> GetAdjacentLocations(Vector2Int location)
    {
        List<Vector2Int> adjacentLocation = new List<Vector2Int>();

        //West
        if (location.x - 1 >= 0)
        {
            adjacentLocation.Add(location + new Vector2Int(-1, 0));
        }

        //East
        if (location.x + 1 < GameManager.Instance.GridManager._width)
        {
            adjacentLocation.Add(location + new Vector2Int(1, 0));
        }

        //North
        if (location.y + 1 < GameManager.Instance.GridManager._height)
        {
            adjacentLocation.Add(location + new Vector2Int(0, 1));
        }

        //South
        if (location.y - 1 >= 0)
        {
            adjacentLocation.Add(location + new Vector2Int(0, -1));
        }

        //North West
        if (location.x - 1 >= 0 && location.y + 1 < GameManager.Instance.GridManager._height)
        {
            adjacentLocation.Add(location + new Vector2Int(-1, 1));
        }

        //North East
        if (location.x + 1 < GameManager.Instance.GridManager._width && location.y + 1 < GameManager.Instance.GridManager._height)
        {
            adjacentLocation.Add(location + new Vector2Int(1, 1));
        }

        //South West
        if (location.x - 1 >= 0 && location.y - 1 >= 0)
        {
            adjacentLocation.Add(location + new Vector2Int(-1, -1));
        }

        //South East
        if (location.x + 1 < GameManager.Instance.GridManager._width && location.y - 1 >= 0)
        {
            adjacentLocation.Add(location + new Vector2Int(1, -1));
        }

        return adjacentLocation;
    }
}