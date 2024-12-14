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

    public TextMeshProUGUI stateText;

    public List<GeneralWFCTile> tileset;
    readonly WaveFunctionCollapse wfc = new();

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
                return 2;
            return 1;
        };

        Reinitialize();
    }

    // Update is called once per frame
    void Update()
    {
        RenderWFC();
        if (halted)
        {
            return;
        }
        if (wfc.IsCollapsed())
        {
            stateText.text = "Collapsed";
            halted = true;
        }
        else
        {
            Iterate();
        }
    }

    public void Iterate()
    {
        try
        {
            iteration++;
            stateText.text = $"Iteration {iteration}";

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
        wfc.Initialize();
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
        wfc.SetAt(new(width / 2, 2, depth / 2), wfc.tileset.Find(t => t.Name == TileName.Ground));

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

        foreach (var (pos, tile) in wfc.Result)
        {
            if (tile == null || tile.prefab == null) continue;
            var go = tile.ToGameObject(transform);
            go.transform.localPosition = pos * 2;
        }
        wfc.updated = false;

    }
}
