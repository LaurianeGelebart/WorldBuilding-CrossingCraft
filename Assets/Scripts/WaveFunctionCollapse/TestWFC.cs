using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestWFC : MonoBehaviour
{
    public Vector3Int min, max;
    public Vector3 tileSize = new(2, 2, 2);

    public TextMeshProUGUI stateText;

    public bool renderIterations = false;
    public bool manualIterations = false;

    public List<GeneralWFCTile> tileset;
    readonly WaveFunctionCollapse wfc = new();

    float timer = 0;

    bool halted = false;
    int iteration = 0;

    // Start is called before the first frame update
    void Start()
    {
        List<WFCTile> tiles = new();
        // List<string> whiteList = new() {
        //     TileName.ForestGround,
        //     TileName.ForestGroundWallInnerCorner,
        //     TileName.ForestGroundWallOuterCorner,
        //     TileName.ForestCliffSide,
        //     TileName.ForestCliffCorner,
        //     TileName.ForestGroundWallSide,

        //     TileName.DesertGround,
        //     TileName.DesertGroundWallInnerCorner,
        //     TileName.DesertGroundWallOuterCorner,
        //     TileName.DesertCliffSide,
        //     TileName.DesertCliffCorner,
        //     TileName.DesertGroundWallSide,

        //     TileName.TransitionForestDesertLinear,
        //     TileName.TransitionForestDesertAngleDesert,
        //     TileName.TransitionForestDesertAngleForest
        // };
        foreach (var generalTile in tileset)
        {
            // if (
            //     generalTile.prefab != null &&
            //     !whiteList.Contains(generalTile.prefab.name)
            // ) continue;
            tiles.AddRange(generalTile.ToWFCTiles());
        }
        wfc.tileset = tiles;
        wfc.min = min;
        wfc.max = max;

        var width = max.x - min.x + 1;
        var height = max.y - min.y + 1;
        var depth = max.z - min.z + 1;

        wfc.weightCallback = (pos, tile) =>
        {
            var val = 10;
            if (TileName.HasWater(tile.Name))
                val = 500;
            if (TileName.IsGround(tile.Name) || TileName.HasProps(tile.Name))
                val = 500;
            if (TileName.HasWall(tile.Name))
                val = 1;

            float forest_weight = Mathf.InverseLerp(min.z, max.z, pos.z);
            float desert_weight = Mathf.InverseLerp(max.z, min.z, pos.z);
            // float transition_weight = (1 - Mathf.Abs(z - 0.5f)) * 2;
            if (TileName.IsForest(tile.Name))
                return (int)(val * forest_weight);
            else if (TileName.IsDesert(tile.Name))
                return (int)(val * desert_weight);
            // else if (TileName.IsTransition(tile.Name))
            //     return (int)(val * transition_weight);
            // if (tile.sockets.IsAll("-1"))
            //     return 8;
            // else if (TileName.IsGround(tile.Name))
            //     return 256;
            // // else if (TileName.IsWall(tile.Name))
            // return 0;
            // else if (TileName.IsTransition(tile.Name))
            //     return 16;
            // else if (TileName.HasWater(tile.Name))
            //     return 256;
            // return 2;
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
        if (!manualIterations)
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
        wfc.SetAt(new(0, 1, 0), wfc.tileset.Find(t => t.Name == TileName.DesertGround));
        wfc.SetAt(new(width / 2, 1, 0), wfc.tileset.Find(t => t.Name == TileName.DesertGround));
        wfc.SetAt(new(width - 1, 1, 0), wfc.tileset.Find(t => t.Name == TileName.DesertGround));
        wfc.SetAt(new(0, 1, depth - 1), wfc.tileset.Find(t => t.Name == TileName.ForestGround));
        wfc.SetAt(new(width / 2, 1, depth - 1), wfc.tileset.Find(t => t.Name == TileName.ForestGround));
        wfc.SetAt(new(width - 1, 1, depth - 1), wfc.tileset.Find(t => t.Name == TileName.ForestGround));
        wfc.SetAt(new(0, 1, depth / 2), wfc.tileset.Find(t => t.Name == TileName.TransitionForestDesertLinear && t.rotationY == 3));
        wfc.SetAt(new(width / 2, 1, depth / 2), wfc.tileset.Find(t => t.Name == TileName.TransitionForestDesertLinear && t.rotationY == 3));
        wfc.SetAt(new(width - 1, 1, depth / 2), wfc.tileset.Find(t => t.Name == TileName.TransitionForestDesertLinear && t.rotationY == 3));

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

class TileName
{
    public static string ForestGround = "f-ground";
    public static string ForestGroundWallSide = "f-ground-wall-side";
    public static string ForestGroundWallInnerCorner = "f-ground-wall-inner-corner";
    public static string ForestGroundWallOuterCorner = "f-ground-wall-outer-corner";
    public static string ForestWall = "f-wall";
    public static string ForestWallOuterSide = "f-wall-outer-side";
    public static string ForestWallInnerSide = "f-wall-inner-side";
    public static string ForestWallHole = "f-wall-hole";
    public static string ForestWallBulge = "f-wall-bulge";
    public static string ForestCliffSide = "f-cliff-side";
    public static string ForestCliffCorner = "f-cliff-corner";

    public static string DesertGround = "d-ground";
    public static string DesertGroundWallSide = "d-ground-wall-side";
    public static string DesertGroundWallInnerCorner = "d-ground-wall-inner-corner";
    public static string DesertGroundWallOuterCorner = "d-ground-wall-outer-corner";
    public static string DesertWall = "d-wall";
    public static string DesertWallOuterSide = "d-wall-outer-side";
    public static string DesertWallInnerSide = "d-wall-inner-side";
    public static string DesertWallHole = "d-wall-hole";
    public static string DesertWallBulge = "d-wall-bulge";
    public static string DesertCliffSide = "d-cliff-side";
    public static string DesertCliffCorner = "d-cliff-corner";

    public static string TransitionForestDesertLinear = "transition-linear";
    public static string TransitionForestDesertAngleDesert = "transition-angle-desert";
    public static string TransitionForestDesertAngleForest = "transition-angle-forest";

    public static bool IsForest(string name)
    {
        return name.StartsWith("f-");
    }
    public static bool IsDesert(string name)
    {
        return name.StartsWith("d-");
    }
    public static bool IsTransition(string name)
    {
        return name.StartsWith("transition-");
    }
    public static bool HasWater(string name)
    {
        return name.Contains("water");
    }
    public static bool HasProps(string name)
    {
        return name.Contains("trees") || name.Contains("rocks");
    }

    public static bool IsGround(string name)
    {
        return name.EndsWith("-ground");
    }
    public static bool IsWall(string name)
    {
        return name.EndsWith("-wall");
    }
    public static bool IsCliff(string name)
    {
        return name.EndsWith("-cliff");
    }
    public static bool HasGround(string name)
    {
        return name.Contains("ground");
    }
    public static bool HasWall(string name)
    {
        return name.Contains("wall");
    }
    public static bool HasCliff(string name)
    {
        return name.Contains("cliff");
    }
    public static bool HasHole(string name)
    {
        return name.Contains("hole");
    }
    public static bool HasBulge(string name)
    {
        return name.Contains("bulge");
    }
};
