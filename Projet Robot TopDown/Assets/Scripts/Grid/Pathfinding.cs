using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public PriorityQueue closedList;
    public PriorityQueue openList;

    public List<Tile> Frontier(Vector2Int source, int limit)
    {
        List<Tile> frontierTile = new List<Tile>();

        for (int i = 0; i < GameManager.Instance.GridManager._width; i++)
        {
            for (int j = 0; j < GameManager.Instance.GridManager._height; j++)
            {
                Tile tile = GameManager.Instance.GridManager.GetTile(i, j);

                if (Vector2Int.Distance(source, tile._location) <= limit)
                {
                    //activate movement cell
                    tile._movementCellSR.SetActive(true);
                    frontierTile.Add(tile);
                }
            }
        }

        return frontierTile;
    }

    private static List<Tile> CalculatePath(Tile node)
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
    private static float EstimateHeuristicCost(Tile curNode, Tile goalNode)
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

            if(neighbors.Count == 0)
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