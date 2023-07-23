using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public List<Tile> Frontier(Vector2Int source, int range)
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        PriorityQueue<Vector2Int, float> frontier = new PriorityQueue<Vector2Int, float>();
        frontier.Enqueue(source, 0);

        cameFrom[source] = source;
        costSoFar[source] = 0;

        //BFS
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            Tile currentTile = GameManager.Instance.GridManager.GetTile(current);

            foreach (Tile next in GameManager.Instance.GridManager.GetNeighbors(currentTile))
            {
                //int newCost = costSoFar[current] + graph.Cost(current, next);
                float newCost;
                if (EstimateHeuristicCost(current, next._location) == 1)
                    newCost = costSoFar[current] + 1;
                else
                    newCost = costSoFar[current] + 1.3f;
                if (!costSoFar.ContainsKey(next._location) || newCost < costSoFar[next._location])
                {
                    costSoFar[next._location] = newCost;
                    next._distanceUI.text = newCost.ToString();
                    float priority = newCost + EstimateHeuristicCost(next._location, source);
                    cameFrom[next._location] = current;

                    if (newCost <= range)
                        frontier.Enqueue(next._location, priority);
                }
            }
        }

        List<Tile> finalFrontier = new List<Tile>();
        foreach (Vector2Int tileLocation in costSoFar.Keys)
        {
            if (costSoFar[tileLocation] <= range)
                finalFrontier.Add(GameManager.Instance.GridManager.GetTile(tileLocation));
        }

        return finalFrontier;
    }

    public List<Tile> FindPath(Vector2Int start, Vector2Int goal)
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        PriorityQueue<Vector2Int, float> frontier = new PriorityQueue<Vector2Int, float>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        //BFS
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            Tile currentTile = GameManager.Instance.GridManager.GetTile(current);

            if (current.Equals(goal))
            {
                break;
            }

            foreach (Tile next in GameManager.Instance.GridManager.GetNeighbors(currentTile))
            {
                float newCost;
                if (EstimateHeuristicCost(current, next._location) == 1)
                    newCost = costSoFar[current] + 1;
                else
                    newCost = costSoFar[current] + 1.3f;
                //int newCost = costSoFar[current] + graph.Cost(current, next);
                if (!costSoFar.ContainsKey(next._location) || newCost < costSoFar[next._location])
                {
                    costSoFar[next._location] = newCost;
                    float priority = newCost + EstimateHeuristicCost(next._location, goal);
                    frontier.Enqueue(next._location, priority);
                    cameFrom[next._location] = current;
                }
            }
        }

        //check if path exist
        if (costSoFar.ContainsKey(goal) == false || costSoFar[goal] == 0)
            return null;

        Tile currentParent = GameManager.Instance.GridManager.GetTile(goal);
        List<Tile> path = new List<Tile>();
        path.Add(GameManager.Instance.GridManager.GetTile(goal));

        while (currentParent._location != start)
        {
            Tile newParent = GameManager.Instance.GridManager.GetTile(cameFrom[currentParent._location]);
            path.Add(newParent);
            currentParent = newParent;
        }

        path.Reverse();
        return path;
    }

    public List<Tile> VisibleTiles(Tile source, int maxViewDistance)
    {
        List<Tile> visibleTiles = Frontier(source._location, maxViewDistance);

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

    /// Calculate the estimated Heuristic cost to the goal 

    private int EstimateHeuristicCost(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}