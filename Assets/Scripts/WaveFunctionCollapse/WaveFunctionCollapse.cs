using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WaveFunctionCollapse
{
    public List<WFCTile> tileset;
    public Vector3Int min, max;
    public Func<Vector3Int, WFCTile, float> weightCallback = (pos, tile) => 1;

    private WFCCell[,,] cellGrid;
    public bool updated = false;

    public WFCCell GetAt(Vector3Int pos)
    {
        return cellGrid[pos.x - min.x, pos.y - min.y, pos.z - min.z];
    }
    public void SetAt(Vector3Int pos, WFCTile tile)
    {
        GetAt(pos).CollapseTo(tile);
        Propagate(pos);
        updated = true;
    }
    public void Constrain(Vector3Int pos, WFCTile tile)
    {
        WFCCell cell = GetAt(pos);
        if (cell == null) return;
        if (cell.Collapsed)
        {
            throw new Exception($"Attempted to constrain a collapsed cell at Vector3{pos}");
        }
        if (!cell.possibleTiles.Remove(tile))
        {
            throw new Exception($"Failed to constrain to {tile} at Vector3{pos}");
        }
        updated = true;
    }
    public void Clear()
    {
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    var pos = new Vector3Int(x, y, z);
                    cellGrid[pos.x - min.x, pos.y - min.y, pos.z - min.z] = new WFCCell(tileset);
                }
            }
        }
        updated = true;
    }
    public WFCTile GetTileAt(Vector3Int pos)
    {
        var cell = GetAt(pos);
        if (!cell.Collapsed) return null;
        return cell.Tile;
    }

    public void Initialize()
    {
        foreach (var tile in tileset)
        {
            tile.FetchValidNeighbors(tileset);
        }
        cellGrid = new WFCCell[max.x - min.x + 1, max.y - min.y + 1, max.z - min.z + 1];
    }

    public void DoTheThing()
    {
        while (!IsCollapsed())
        {
            Iterate();
        }
    }

    public bool IsCollapsed()
    {
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    var pos = new Vector3Int(x, y, z);
                    var cell = GetAt(pos);
                    if (!cell.Collapsed) return false;
                    if (cell.Entropy == 0) throw new System.Exception($"No possible tiles at {pos}");
                }
            }
        }
        return true;
    }

    public void Iterate()
    {
        var pos = FindMinimumEntropyPosition();
        CollapseAt(pos);
        Propagate(pos);
    }

    public Vector3Int FindMinimumEntropyPosition()
    {
        int minEntropy = int.MaxValue;
        List<Vector3Int> possiblePositions = new();
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                for (int z = min.z; z <= max.z; z++)
                {
                    var pos = new Vector3Int(x, y, z);
                    MinimumEntropyAt(pos, ref minEntropy, ref possiblePositions);
                }
            }
        }
        int index = UnityEngine.Random.Range(0, possiblePositions.Count);
        return possiblePositions[index];
    }

    public void MinimumEntropyAt(Vector3Int position, ref int minEntropy, ref List<Vector3Int> possiblePositions)
    {
        var cell = GetAt(position);
        if (cell.Collapsed) return;
        if (cell.Entropy == 0)
        {
            throw new System.Exception($"No possible tiles at {position}");
        }
        if (cell.Entropy < minEntropy)
        {
            minEntropy = cell.Entropy;
            possiblePositions = new() { position };
        }
        else if (cell.Entropy == minEntropy)
        {
            possiblePositions.Add(position);
        }
    }

    public int GetRandomIndexFromWeight(Vector3Int pos)
    {
        WFCCell cell = GetAt(pos);
        if (cell.Collapsed) return 0;
        if (cell.Entropy == 0)
            throw new System.Exception($"No possible tiles at Vector3{pos}");

        float[] weights = new float[cell.possibleTiles.Count];
        for (int i = 0; i < cell.possibleTiles.Count; i++)
        {
            weights[i] = weightCallback(pos, cell.possibleTiles[i]);
        }
        float totalWeight = weights.Sum();
        float randomWeight = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            randomWeight -= weights[i];
            if (randomWeight <= 0)
            {
                return i;
            }
        }
        return 0;
    }

    public void CollapseAt(Vector3Int pos)
    {
        int index = GetRandomIndexFromWeight(pos);
        var cell = GetAt(pos);
        cell.CollapseTo(cell.possibleTiles[index]);
        updated = true;
    }

    public void Propagate(Vector3Int pos)
    {
        Stack<Vector3Int> stack = new();
        Stack<Vector3Int> stackTmp = new();
        bool crash = false;
        stack.Push(pos);

        int iterationCount = 0;

        while (stack.Count > 0)
        {
            if (crash || iterationCount++ > 100000)
            {
                throw new Exception($"Infinite loop detected with stack size of {stack.Count} at Vector3{pos}");
            }
            PropagateStep(stack);
        }
    }
    public void PropagateStep(Stack<Vector3Int> stack)
    {
        Vector3Int currentPos = stack.Pop();
        foreach (var dir in ValidDirs(currentPos))
        {
            Vector3Int neighborPos = currentPos + dir;
            List<WFCTile> neighborPossibleTiles = new(GetAt(neighborPos).possibleTiles);

            List<WFCTile> possibleNeighbors = PossibleNeighbors(currentPos, dir);

            if (neighborPossibleTiles.Count == 0)
                continue;

            foreach (var neighborTile in neighborPossibleTiles)
            {
                if (!possibleNeighbors.Contains(neighborTile))
                {
                    Constrain(neighborPos, neighborTile);
                    if (!stack.Contains(neighborPos))
                        stack.Push(neighborPos);
                }
            }
        }
    }

    public List<Vector3Int> ValidDirs(Vector3Int pos)
    {
        List<Vector3Int> dirs = new();
        if (pos.x > min.x) dirs.Add(Vector3Int.left);
        if (pos.x < max.x) dirs.Add(Vector3Int.right);
        if (pos.y > min.y) dirs.Add(Vector3Int.down);
        if (pos.y < max.y) dirs.Add(Vector3Int.up);
        if (pos.z > min.z) dirs.Add(Vector3Int.back);
        if (pos.z < max.z) dirs.Add(Vector3Int.forward);
        return dirs;
    }
    public bool IsInRange(Vector3Int pos)
    {
        return pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y && pos.z >= min.z && pos.z <= max.z;
    }
    public Vector3Int GetBorderDirection(Vector3Int pos)
    {
        var dir = Vector3Int.zero;
        if (pos.x == min.x) dir += Vector3Int.left;
        if (pos.x == max.x) dir += Vector3Int.right;
        if (pos.y == min.y) dir += Vector3Int.down;
        if (pos.y == max.y) dir += Vector3Int.up;
        if (pos.z == min.z) dir += Vector3Int.back;
        if (pos.z == max.z) dir += Vector3Int.forward;
        return dir;
    }
    public bool IsInBorder(Vector3Int pos)
    {
        return GetBorderDirection(pos) != Vector3Int.zero;
    }

    // TODO:
    public List<WFCTile> PossibleNeighbors(Vector3Int pos, Vector3Int dir)
    {
        WFCCell cell = GetAt(pos);
        if (cell == null) return new();
        var neighbor = GetAt(pos + dir);
        List<WFCTile> possibleNeighbors = new();
        switch (dir)
        {
            case Vector3Int left when left == Vector3Int.left:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.XNegNeighbors));
                break;
            case Vector3Int right when right == Vector3Int.right:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.XPosNeighbors));
                break;
            case Vector3Int down when down == Vector3Int.down:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.YNegNeighbors));
                break;
            case Vector3Int up when up == Vector3Int.up:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.YPosNeighbors));
                break;
            case Vector3Int back when back == Vector3Int.back:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.ZNegNeighbors));
                break;
            case Vector3Int forward when forward == Vector3Int.forward:
                possibleNeighbors.AddRange(cell.possibleTiles.SelectMany(tile => tile.ZPosNeighbors));
                break;
        }
        possibleNeighbors = possibleNeighbors.Distinct().ToList();
        return possibleNeighbors;
    }
}

public class WFCCell
{
    public List<WFCTile> tiles;
    public List<WFCTile> possibleTiles;

    public int Entropy { get { return possibleTiles.Count; } }
    public bool Collapsed { get { return Entropy == 1; } }
    public WFCTile Tile { get { return Collapsed ? possibleTiles[0] : null; } }

    public WFCCell(List<WFCTile> tiles)
    {
        this.tiles = new(tiles);
        possibleTiles = new(tiles);
    }

    public void Reset()
    {
        possibleTiles = new(tiles);
    }

    public void CollapseTo(WFCTile tile)
    {
        if (!tiles.Contains(tile)) throw new Exception("Invalid tile");
        possibleTiles = new() { tile };
    }
}

[System.Serializable]
public struct WFCSockets
{
    public string xPos;
    public string xNeg;
    public string yPos;
    public string yNeg;
    public string zPos;
    public string zNeg;

    public WFCSockets Rotated(int rotation)
    {
        return rotation switch
        {
            1 => new()
            {
                xPos = zPos,
                zNeg = xPos,
                xNeg = zNeg,
                zPos = xNeg,
                yPos = VerticalRotated(yPos, 1),
                yNeg = VerticalRotated(yNeg, 1),
            },
            2 => new()
            {
                xPos = xNeg,
                zNeg = zPos,
                xNeg = xPos,
                zPos = zNeg,
                yPos = VerticalRotated(yPos, 2),
                yNeg = VerticalRotated(yNeg, 2),
            },
            3 => new()
            {
                xPos = zNeg,
                zNeg = xNeg,
                xNeg = zPos,
                zPos = xPos,
                yPos = VerticalRotated(yPos, 3),
                yNeg = VerticalRotated(yNeg, 3),
            },
            _ => new()
            {
                xPos = xPos,
                zNeg = zNeg,
                xNeg = xNeg,
                zPos = zPos,
                yPos = yPos,
                yNeg = yNeg,
            },
        };
    }
    public WFCSockets Flipped(string yPosFlipped, string yNegFlipped)
    {
        return new()
        {
            xPos = HorizontalFlipped(xNeg),
            xNeg = HorizontalFlipped(xPos),
            zPos = HorizontalFlipped(zPos),
            zNeg = HorizontalFlipped(zNeg),
            yPos = yPosFlipped,
            yNeg = yNegFlipped,
        };
    }

    public readonly bool IsAll(string socket)
    {
        return xPos == socket && xNeg == socket && yPos == socket && yNeg == socket && zPos == socket && zNeg == socket;
    }

    public static bool IsSymmetric(string socket)
    {
        return socket.EndsWith("s");
    }
    public static bool IsVertical(string socket)
    {
        return socket.StartsWith("v");
    }
    public static bool IsVoid(string socket)
    {
        return socket.StartsWith("-");
    }
    public static string VerticalRotated(string socket, int rotation) // v[...]_0 -> v[...]_1
    {
        if (IsVoid(socket)) return socket;
        if (!IsVertical(socket)) throw new System.Exception($"{socket} is not a vertical socket");
        string rotation_str = socket.Substring(socket.Length - 1, 1);
        int rotation_int = int.Parse(rotation_str);
        rotation_int = (rotation_int + rotation) % 4;
        return $"{socket[..^2]}_{rotation_int}";
    }
    public static string HorizontalFlipped(string socket) // x -> xf, xf -> x
    {
        if (IsVoid(socket)) return socket;
        if (IsVertical(socket)) throw new System.Exception($"{socket} is not a horizontal socket");
        if (socket.EndsWith("f"))
        {
            return socket[..^1];
        }
        else
        {
            return socket + "f";
        }
    }

    public static bool IsCompatible(string socket1, string socket2)
    {
        if (IsSymmetric(socket1) || IsVertical(socket1) || IsVoid(socket1))
            return socket1 == socket2;
        return socket1 == socket2 + "f" || socket1 + "f" == socket2;
    }
}

[System.Serializable]
public class WFCTile
{
    public GameObject prefab;
    public WFCSockets sockets;
    public int rotationY;
    public bool flipX;
    public string Name { get { return prefab == null ? $"void({sockets.xPos})" : prefab.name; } }

    readonly List<WFCTile> xPosNeighbors = new();
    readonly List<WFCTile> xNegNeighbors = new();
    readonly List<WFCTile> yPosNeighbors = new();
    readonly List<WFCTile> yNegNeighbors = new();
    readonly List<WFCTile> zPosNeighbors = new();
    readonly List<WFCTile> zNegNeighbors = new();

    public List<WFCTile> XPosNeighbors { get { return xPosNeighbors; } }
    public List<WFCTile> XNegNeighbors { get { return xNegNeighbors; } }
    public List<WFCTile> YPosNeighbors { get { return yPosNeighbors; } }
    public List<WFCTile> YNegNeighbors { get { return yNegNeighbors; } }
    public List<WFCTile> ZPosNeighbors { get { return zPosNeighbors; } }
    public List<WFCTile> ZNegNeighbors { get { return zNegNeighbors; } }

    public GameObject ToGameObject(Transform parent = null)
    {
        if (prefab == null) return null;
        GameObject go = GameObject.Instantiate(prefab);
        if (parent) go.transform.parent = parent;
        go.transform.localPosition = new(0, 0, 0);
        go.transform.localEulerAngles = new(0, 90 * rotationY, 0);
        if (flipX)
            go.transform.localScale = new(-1, 1, 1);
        else
            go.transform.localScale = new(1, 1, 1);
        return go;
    }

    public void FetchValidNeighbors(List<WFCTile> tiles)
    {
        xPosNeighbors.Clear();
        xNegNeighbors.Clear();
        yPosNeighbors.Clear();
        yNegNeighbors.Clear();
        zPosNeighbors.Clear();
        zNegNeighbors.Clear();
        foreach (var tile in tiles)
        {
            if (WFCSockets.IsCompatible(sockets.xPos, tile.sockets.xNeg))
            {
                xPosNeighbors.Add(tile);
            }
            if (WFCSockets.IsCompatible(sockets.xNeg, tile.sockets.xPos))
            {
                xNegNeighbors.Add(tile);
            }
            if (WFCSockets.IsCompatible(sockets.yPos, tile.sockets.yNeg))
            {
                yPosNeighbors.Add(tile);
            }
            if (WFCSockets.IsCompatible(sockets.yNeg, tile.sockets.yPos))
            {
                yNegNeighbors.Add(tile);
            }
            if (WFCSockets.IsCompatible(sockets.zPos, tile.sockets.zNeg))
            {
                zPosNeighbors.Add(tile);
            }
            if (WFCSockets.IsCompatible(sockets.zNeg, tile.sockets.zPos))
            {
                zNegNeighbors.Add(tile);
            }
        }
    }

    public override string ToString()
    {
        return $"{Name} - {sockets.xPos} {sockets.xNeg} {sockets.yPos} {sockets.yNeg} {sockets.zPos} {sockets.zNeg}";
    }
}

public enum Flip { None, X, Z }
[System.Serializable]
public class GeneralWFCTile
{

    public GameObject prefab;
    public WFCSockets sockets;
    public bool rotates;
    public bool flip;
    public string yPosFlipped;
    public string yNegFlipped;

    public List<WFCTile> ToWFCTiles()
    {
        List<WFCTile> tiles = new()
        {
            new WFCTile
            {
                prefab = prefab,
                sockets = sockets,
                rotationY = 0,
                flipX = false,
            }
        };
        if (rotates)
        {
            for (int i = 1; i <= 3; i++)
            {
                tiles.Add(new()
                {
                    prefab = prefab,
                    sockets = sockets.Rotated(i),
                    rotationY = i,
                    flipX = false,
                });
            }
        }
        if (flip)
        {
            var flipped = sockets.Flipped(yPosFlipped, yNegFlipped);
            tiles.Add(new()
            {
                prefab = prefab,
                sockets = flipped,
                rotationY = 0,
                flipX = true,
            });
            for (int i = 1; i <= 3; i++)
            {
                tiles.Add(new()
                {
                    prefab = prefab,
                    sockets = flipped.Rotated(i),
                    rotationY = i,
                    flipX = true,
                });
            }
        }
        return tiles;
    }
}
