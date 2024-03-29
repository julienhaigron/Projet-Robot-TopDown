using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public PriorityQueue closedList;
    public PriorityQueue openList;

    // Direction vectors
    static int[] dRow = { -1, 0, 1, 0 };
    static int[] dCol = { 0, 1, 0, -1 };

    // Function to check if a cell
    // is be visited or not
    private bool isValid(bool[,] vis, int row, int col)
    {
        // If cell lies out of bounds
        if (row < 0 || col < 0 || row >= GameManager.Instance.GridManager._height || col >= GameManager.Instance.GridManager._width)
            return false;

        // If cell is already visited
        if (vis[row, col])
            return false;

        // Otherwise
        return true;
    }

    public List<Tile> BFS(Vector2Int source)
    {
        // Stores indices of the matrix cells
        Queue<Tile> q = new Queue<Tile>();

        bool[,] visited = new bool[GameManager.Instance.GridManager._height, GameManager.Instance.GridManager._width];
        List<Tile> frontier = new List<Tile>();

        // Mark the starting cell as visited
        // and push it into the queue
        q.Enqueue(GameManager.Instance.GridManager.GetTile(source.x, source.y));
        visited[source.x, source.y] = true;

        // Iterate while the queue
        // is not empty
        while (q.Count != 0)
        {
            Tile tile = q.Peek();
            int x = tile._location.x;
            int y = tile._location.y;
            q.Dequeue();

            List<Tile> neighbors = new List<Tile>();
            neighbors = GameManager.Instance.GridManager.GetNeighbors(tile);

            // Go to the adjacent cells
            for (int i = 0; i < 4; i++)
            {
                int adjx = x + dRow[i];
                int adjy = y + dCol[i];
                if (isValid(visited, adjx, adjy))
                {
                    Tile neigbhor = GameManager.Instance.GridManager.GetTile(adjx, adjy);
                    q.Enqueue(neigbhor);
                    frontier.Add(neigbhor);
                    visited[adjx, adjy] = true;
                }
            }
        }

        return frontier;
    }

    public List<Tile> TilesInRange(Tile source, int range)
    {
        List<Tile> frontier = BFS(source._location);
        List<Tile> tilesInRange = new List<Tile>();

        foreach(Tile tile in frontier)
        {
            List<Tile> pathToThisTile = FindPath(source, tile);

            if(pathToThisTile != null && pathToThisTile.Count <= range)
            {
                tilesInRange.Add(tile);
            }
        }

        return tilesInRange;
    }

    public List<Tile> VisibleTiles(Tile source, int maxViewDistance)
    {
        List<Tile> visibleTiles = TilesInRange(source, maxViewDistance);

        foreach (Tile tile in visibleTiles)
        {
            List<Tile> lineToThisTile = Line(source, tile);
            //bool isVisible = true;
            foreach (Tile tileInLine in lineToThisTile)
            {
                if (!tileInLine._isWalkable)
                {
                    visibleTiles.Remove(tile);
                }
            }
        }

        return visibleTiles;
    }

    public List<Tile> Line(Tile from, Tile to)
    {
        List<Tile> line = new List<Tile>();
        float dist = DiagonalDistance(from._location, to._location);
        for (int i = 0; i < dist; i++)
        {
            float t = dist == 0 ? 0.0f : i / dist;
            line.Add(RoundTile(LerpTile(from._location, to._location, t)));
        }

        return line;
    }

    public float DiagonalDistance(Vector2 p0, Vector2 p1)
    {
        float dx = p1.x - p0.x;
        float dy = p1.y - p0.y;
        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
    }

    public Tile RoundTile(Vector2 loc)
    {
        return GameManager.Instance.GridManager.GetTile(Mathf.RoundToInt(loc.x), Mathf.RoundToInt(loc.y));
    }

    public Vector2 LerpTile(Vector2 l1, Vector2 l2, float pos)
    {
        //return new Vector2(Lerp(l1.x, l2.x, pos), Lerp(l1.y, l2.y, pos));
        return Vector2.Lerp(l1, l2, pos);
    }

    public float Lerp(float start, float end, int t)
    {
        return start * (1f - t) + t * end;
    }

    private List<Tile> CalculatePath(Tile node)
    {
        List<Tile> list = new List<Tile>();
        while (node != null)
        {
            list.Add(node);
            node = node._parentNode;
        }
        list.Reverse();
        return list;
    }


    /// Calculate the estimated Heuristic cost to the goal 
    private float EstimateHeuristicCost(Tile curNode, Tile goalNode)
    {
        Vector2Int vecCost = curNode._location - goalNode._location;
        return vecCost.magnitude;
    }

    public List<Tile> FindPath(Tile start, Tile goal)
    {
        openList = new PriorityQueue();
        openList.Push(start);
        start._g = 0.0f;
        start._h = EstimateHeuristicCost(start, goal);

        closedList = new PriorityQueue();
        Tile node = null;
        if (GameManager.Instance.GridManager == null)
        {
            return null;
        }

        while (openList.Length != 0)
        {
            node = openList.GetFirstNode();

            if (node._location == goal._location)
            {
                return CalculatePath(node);
            }

            List<Tile> neighbors = new List<Tile>();
            neighbors = GameManager.Instance.GridManager.GetNeighbors(node);

            if (neighbors.Count == 0)
            {
                Debug.Log("error here");
            }

            //Update the costs of each neighbor node.
            for (int i = 0; i < neighbors.Count; i++)
            {
                Tile neighborNode = neighbors[i];

                if (!closedList.Contains(neighborNode))
                {
                    //Cost from current node to this neighbor node
                    float cost = EstimateHeuristicCost(node, neighborNode);

                    //Total Cost So Far from start to this neighbor node
                    float totalCost = node._g + cost;

                    //Estimated cost for neighbor node to the goal
                    float neighborNodeEstCost = EstimateHeuristicCost(neighborNode, goal);

                    //Assign neighbor node properties
                    neighborNode._g = totalCost;
                    neighborNode._parentNode = node;
                    neighborNode._h = totalCost + neighborNodeEstCost;

                    //Add the neighbor node to the open list if we haven't already done so.
                    if (!openList.Contains(neighborNode))
                    {
                        openList.Push(neighborNode);
                    }
                }
            }
            closedList.Push(node);
            openList.Remove(node);
        }

        //We handle the scenario where no goal was found after looping thorugh the open list
        if (node._location != goal._location)
        {
            Debug.Log("Goal Not Found; nb parcourded found : " + closedList.Length);
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(node);
    }
}