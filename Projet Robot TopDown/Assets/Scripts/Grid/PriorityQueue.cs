using System.Collections;
using System.Collections.Generic;

public class PriorityQueue
{
    private List<Tile> tiles;

    public PriorityQueue()
    {
        tiles = new List<Tile>();
    }

    public int Length
    {
        get { return tiles.Count; }
    }

    public bool Contains(Tile tile)
    {
        return tiles.Contains(tile);
    }

    public Tile GetFirstNode()
    {
        if (tiles.Count > 0)
        {
            return tiles[0];
        }
        return null;
    }

    public void Push(Tile node)
    {
        tiles.Add(node);
    }

    public void Remove(Tile node)
    {
        tiles.Remove(node);
    }
}
