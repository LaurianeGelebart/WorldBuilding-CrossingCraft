using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

class TileName
{
    public static string Ground = "ground";
    public static string GroundWallSide = "ground-wall-side";
    public static string GroundWallInnerCorner = "ground-wall-inner-corner";
    public static string GroundWallOuterCorner = "ground-wall-outer-corner";
    public static string Wall = "wall";
    public static string WallOuterSide = "wall-outer-side";
    public static string WallInnerSide = "wall-inner-side";
    public static string WallHole = "wall-hole";
    public static string WallBulge = "wall-bulge";
    public static string CliffSide = "cliff-side";
    public static string CliffCorner = "cliff-corner";
};

public class TestWFC : MonoBehaviour
{
    public Vector3Int min, max;
    public Vector3 tileSize = new(2, 2, 2);

    public TextMeshProUGUI stateText;

    public bool renderIterations = false;

    public List<GeneralWFCTile> tileset;
    readonly WaveFunctionCollapse wfc = new();

    float timer = 0;

    bool halted = false;
    int iteration = 0;

    // Start is called before the first frame update
    void Start()
    {
        List<WFCTile> tiles = new();
        foreach (var generalTile in tileset)
        {
            tiles.AddRange(generalTile.ToWFCTiles());
        }
        wfc.tileset = tiles;
        wfc.min = min;
        wfc.max = max;

        wfc.weightCallback = (pos, tile) =>
        {
            if (tile.Name == TileName.Ground)
                return 5;
            else if (tile.Name == TileName.Wall)
                return 0;
            else if (tile.sockets.IsAll("-1")) // external void (air)
                return 3;
            return 1;
        };

        wfc.Initialize();
        Reinitialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (halted) return;

        if (renderIterations)
            RenderWFC();

        timer += Time.deltaTime;
        Iterate();
        if (wfc.IsCollapsed())
        {
            stateText.text = "Collapsed in " + timer + " seconds";
            halted = true;
            RenderWFC();
        }
    }

    public void Iterate()
    {
        try
        {
            iteration++;
            stateText.text = $"Iteration {iteration} | Time: {timer}";

            wfc.Iterate();
        }
        catch (Exception err)
        {
            stateText.text = $"Error at iteration {iteration}";
            Debug.Log($"At iteration {iteration}");
            halted = true;
            Debug.LogError(err);
        }
    }

    public void Reinitialize()
    {
        timer = 0;
        wfc.Clear();
        for (int x = wfc.min.x; x <= wfc.max.x; x++)
        {
            for (int z = wfc.min.z; z <= wfc.max.z; z++)
            {
                wfc.SetAt(new(x, wfc.max.y, z), wfc.tileset.Find(t => t.sockets.IsAll("-1"))); // air tiles
                wfc.SetAt(new(x, wfc.min.y, z), wfc.tileset.Find(t => t.sockets.IsAll("-2"))); // underground tiles
            }
        }

        var width = max.x - min.x + 1;
        var height = max.y - min.y + 1;
        var depth = max.z - min.z + 1;
        wfc.SetAt(new(width / 2, 1, depth / 2), wfc.tileset.Find(t => t.Name == TileName.Ground));

        halted = false;
        iteration = 0;
    }

    void RenderWFC()
    {
        if (!wfc.updated) return;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = wfc.min.x; x <= wfc.max.x; x++)
        {
            for (int y = wfc.min.y; y <= wfc.max.y; y++)
            {
                for (int z = wfc.min.z; z <= wfc.max.z; z++)
                {
                    var pos = new Vector3Int(x, y, z);
                    var tile = wfc.GetTileAt(pos);
                    if (tile == null || tile.prefab == null) continue;
                    var go = tile.ToGameObject(transform);
                    go.transform.localPosition = new(
                        x * tileSize.x,
                        y * tileSize.y,
                        z * tileSize.z
                    );
                }
            }
        }
        wfc.updated = false;
    }
}
