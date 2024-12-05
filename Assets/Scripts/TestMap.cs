using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    Gridmap<UnitTile> gridmap;

    public List<GenericTile> tileset;

    // Start is called before the first frame update
    void Start()
    {
        ParseTileset();
        for (int idx = gridmap.tileset.Count - 1; idx >= 0; idx--)
        {
            var unitTile = gridmap.tileset[idx];
            var pos = new Vector3(idx % 8, 0, idx / 8) * 2;
            var go = unitTile.Instantiate(pos);
            go.transform.parent = transform;
        }
    }

    void ParseTileset()
    {
        List<UnitTile> unitTiles = new();
        foreach (var tile in tileset)
        {
            unitTiles.AddRange(CreateUnitTiles(tile));
        }
        gridmap = new Gridmap<UnitTile>(new Vector3(2, 2, 2), new Vector3(0, 0, 0), unitTiles);
    }
    List<UnitTile> CreateUnitTiles(GenericTile tile)
    {
        List<UnitTile> units = new();
        for (int k = 0; k < 4; k++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (!tile.rotatableY && k != 0) continue;
                if (!tile.flippableX && (j & 1) == 1) continue;
                if (!tile.flippableZ && (j >> 1 & 1) == 1) continue;
                units.Add(new UnitTile
                {
                    tile = tile,
                    rotationY = (Rotation2D)k,
                    flipX = (j & 1) == 1,
                    flipZ = (j >> 1 & 1) == 1,
                });
            }
        }
        return units;
    }
}

public enum Rotation2D
{
    None = 0,
    Right = 1,
    Half = 2,
    Left = 3,
}

[System.Serializable]
public class GenericTile
{
    public GameObject prefab;
    public bool rotatableY;
    public bool flippableX;
    public bool flippableZ;
}

public class UnitTile
{
    public GenericTile tile;
    public Rotation2D rotationY;
    public bool flipX;
    public bool flipZ;

    public GameObject Instantiate(Vector3 position)
    {
        var go = Object.Instantiate(tile.prefab, position, Quaternion.identity);
        go.transform.Rotate(0, 90 * (int)rotationY, 0);
        go.transform.localScale = new Vector3(flipX ? -1 : 1, 1, flipZ ? -1 : 1);
        return go;
    }
}
