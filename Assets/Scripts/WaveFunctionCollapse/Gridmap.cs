using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridmap<GridCell>
{
    public Vector3 cellSize;
    public Vector3 cellOffset;
    public List<GridCell> tileset;

    private readonly Dictionary<Vector3Int, GridCell> map = new();

    public Dictionary<Vector3Int, GridCell> Map { get { return map; } }

    public Gridmap(Vector3 cellSize, Vector3 cellOffset, List<GridCell> tileset)
    {
        this.cellSize = cellSize;
        this.cellOffset = cellOffset;
        this.tileset = tileset;
    }

    public void Clear()
    {
        map.Clear();
    }
    public GridCell Get(Vector3Int position)
    {
        if (map.TryGetValue(position, out var cell))
        {
            return cell;
        }
        return default;
    }
    public void Place(GridCell cell, Vector3Int position)
    {
        map[position] = cell;
    }
    public void Remove(Vector3Int position)
    {
        map.Remove(position);
    }
}
