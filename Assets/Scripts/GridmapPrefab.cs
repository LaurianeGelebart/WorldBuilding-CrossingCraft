using System;
using System.Collections.Generic;
using UnityEngine;

public class GridmapPrefab : MonoBehaviour
{
    // Inspector fields
    public Vector3 gridSize;
    public Vector3 offset;
    public List<GridTilePF> tileset;

    // Private fields
    private readonly List<GridCellPF> cellTileset = new();
    private readonly Dictionary<Vector3Int, GridCellPF> map = new();
    private readonly List<GameObject> gameObjectsTiles = new();

    // Accessors
    public List<GridCellPF> CellTileset { get { return cellTileset; } }
    public Dictionary<Vector3Int, GridCellPF> Map { get { return map; } }
    public List<GameObject> GameObjectsTiles { get { return gameObjectsTiles; } }

    void Start()
    {
        GenerateCells();
    }

    public void GenerateCells()
    {
        cellTileset.Clear();
        foreach (var tile in tileset)
        {
            for (int k = 0; k < 4; k++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int i = 0; i < tile.size.x * tile.size.y * tile.size.z; i++)
                    {
                        cellTileset.Add(new GridCellPF
                        {
                            tile = tile,
                            positionInTile = new(
                                i % tile.size.x,
                                i / tile.size.x % tile.size.y,
                                i / (tile.size.x * tile.size.y)
                            ),
                            flip = new(j & 1, j >> 1 & 1, j >> 2 & 1),
                            rotationY = (GridOrientationPF)k,
                        });
                    }
                }
            }
        }
    }

    public void ClearGameObjects()
    {
        foreach (var go in gameObjectsTiles)
        {
            Destroy(go);
        }
        gameObjectsTiles.Clear();
    }

    public void RefreshGameObjects()
    {
        ClearGameObjects();
        foreach (var kvp in map)
        {
            var unitTile = kvp.Value;
            var position = kvp.Key;

            if (unitTile.positionInTile != Vector3Int.zero) continue;

            var rotation = Quaternion.Euler(0, 90 * (int)unitTile.rotationY, 0);
            var scale = new Vector3(unitTile.flip.x == 1 ? -1 : 1, unitTile.flip.y == 1 ? -1 : 1, unitTile.flip.z == 1 ? -1 : 1);
            var positionWorld = new Vector3(position.x * gridSize.x, position.y * gridSize.y, position.z * gridSize.z);
            var go = Instantiate(unitTile.tile.prefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(transform);
            go.transform.localPosition = positionWorld + offset;
            go.transform.localRotation = rotation;
            go.transform.localScale = scale;
            gameObjectsTiles.Add(go);
        }
    }

    public GridTilePF FindTileByName(string name)
    {
        return tileset.Find(t => t.Name == name);
    }
    public GridCellPF FindCellOfTile(GridTilePF tile, Vector3Int positionInTile, Vector3Int flip, GridOrientationPF rotationY)
    {
        return cellTileset.Find(ut => ut.tile == tile && ut.positionInTile == positionInTile && ut.flip == flip && ut.rotationY == rotationY);
    }

    public Vector3 GetWorldPosition(Vector3Int position)
    {
        return new Vector3(
            position.x * gridSize.x + offset.x,
            position.y * gridSize.y + offset.y,
            position.z * gridSize.z + offset.z
        );
    }
    public Vector3Int GetGridPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.RoundToInt((position.x - offset.x) / gridSize.x),
            Mathf.RoundToInt((position.y - offset.y) / gridSize.y),
            Mathf.RoundToInt((position.z - offset.z) / gridSize.z)
        );
    }

    public Tuple<GridTilePF, Vector3Int> GetTileAndLocation(Vector3Int position)
    {
        if (map.ContainsKey(position))
        {
            return new(map[position].tile, position - map[position].positionInTile);
        }
        return null;
    }
    public bool PlaceTile(GridTilePF tile, Vector3Int position, Vector3Int flip, GridOrientationPF rotationY, bool replaceExisting = false)
    {
        List<GridCellPF> units = cellTileset.FindAll(ut => ut.tile == tile && ut.flip == flip && ut.rotationY == rotationY);
        foreach (GridCellPF unit in units)
        {
            var swizzledPosition = position + unit.PositionInTileSwizzled;
            if (!replaceExisting && map.ContainsKey(swizzledPosition))
            {
                return false;
            }
        }
        foreach (GridCellPF unit in units)
        {
            var swizzledPosition = position + unit.PositionInTileSwizzled;
            map[swizzledPosition] = unit;
        }
        return true;
    }
    public bool PlaceTile(string tileName, Vector3Int position, Vector3Int flip, GridOrientationPF rotationY, bool replaceExisting = false)
    {
        GridTilePF tile = FindTileByName(tileName);
        return PlaceTile(tile, position, flip, rotationY, replaceExisting);
    }
    public void RemoveTile(Vector3Int position)
    {
        var located = GetTileAndLocation(position);
        if (located == null) return;
        var (tile, origin) = located;
        List<GridCellPF> units = cellTileset.FindAll(ut => ut.tile == tile);
        foreach (GridCellPF unit in units)
        {
            var swizzledPosition = origin + unit.PositionInTileSwizzled;
            map.Remove(swizzledPosition);
        }
    }

    public GridCellPF GetCell(Vector3Int position)
    {
        if (map.ContainsKey(position))
        {
            return map[position];
        }
        return null;
    }
    public void PlaceCell(GridCellPF tile, Vector3Int position, bool replaceExisting = false)
    {
        if (!replaceExisting && map.ContainsKey(position))
        {
            return;
        }
        map[position] = tile;
    }
    public void PlaceCell(int idx, Vector3Int position, bool replaceExisting = false)
    {
        PlaceCell(cellTileset[idx], position, replaceExisting);
    }

    public void RemoveCell(Vector3Int position)
    {
        map.Remove(position);
    }
}

public enum GridOrientationPF
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}
[Serializable]
public class GridTilePF
{
    public GameObject prefab;
    public Vector3Int size;

    public string Name { get { return prefab.name; } }
}
public class GridCellPF
{
    public GridTilePF tile;
    public Vector3Int positionInTile;
    public Vector3Int flip;
    public GridOrientationPF rotationY;

    public Vector3Int PositionInTileSwizzled
    {
        get { return SwizzleTile(positionInTile, tile.size, flip, rotationY); }
    }
    public static Vector3Int SwizzleTile(Vector3Int position, Vector3Int size, Vector3Int flip, GridOrientationPF rotationY)
    {
        var x = -(flip.x * 2 - 1) * position.x + (flip.x == 1 ? size.x - 1 : 0);
        var y = -(flip.y * 2 - 1) * position.y + (flip.y == 1 ? size.y - 1 : 0);
        var z = -(flip.z * 2 - 1) * position.z + (flip.z == 1 ? size.z - 1 : 0);
        return rotationY switch
        {
            GridOrientationPF.Right => new Vector3Int(z, y, size.x - 1 - x),
            GridOrientationPF.Down => new Vector3Int(size.x - 1 - x, y, size.z - 1 - z),
            GridOrientationPF.Left => new Vector3Int(size.z - 1 - z, y, x),
            _ => new Vector3Int(x, y, z),
        };
    }
}
